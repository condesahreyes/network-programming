using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using LogicaNegocio;
using System.Net;
using Protocolo;
using System.IO;
using System;
using IServices;
using Servicios;

namespace Servidor
{
    public class Conexion
    {
        List<TcpClient> clientesConectados = new List<TcpClient>();

        private Funcionalidad funcionalidadesServidor;
        //private LogicaUsuario logicaUsuario;
        private IUsuarioService usuarioService = new UsuarioService();
        private IJuegoService juegoService = new JuegoService();

        private TcpClient handler;

        private int cantConexionesEnEspera;
        private int puerto;

        private bool salir = false;

        private string ipServidor;

        public Conexion() 
        {
            IConfiguration configuracion = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json", optional: false).Build();

            puerto = int.Parse(configuracion["port"]);
            cantConexionesEnEspera = int.Parse(configuracion["backLog"]);
            ipServidor = configuracion["ip"];
            this.usuarioService = new UsuarioService();
            Escuchar();
        }
        
        public void Escuchar()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipServidor), puerto);

            TcpListener listener = new TcpListener(endPoint);

            listener.Start(cantConexionesEnEspera);

            Task hiloDeEscucha = new Task(async () => await EscucharPorUsuario(listener));
            hiloDeEscucha.Start();

            MenuServidor(listener);
        }

        private async Task MenuServidor(TcpListener listener)
        {

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("****************** Menú servidor ******************" +
                "\n0. Terminar la conexion. \n1. Ver catalogo de juegos \n2. Crear usuario " +
                "\n3. Ver lista usuarios \n4. Modificar usuario \n5. Dar de baja un usuario \n\nSeleccione una opción:");
            Console.ForegroundColor = ConsoleColor.White;

            string accion = Console.ReadLine();
            if (!Regex.IsMatch(accion, "^[" + 0 + "-" + 5 + "]$"))
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ingrese una opcion valida entre 0 y 1\n");
                MenuServidor(listener);
            }

            switch (accion)
            {
                case "0":
                    salir = true;

                    listener.Stop(); 

                    foreach (var socketCliente in clientesConectados)
                    {
                        socketCliente.GetStream().Close(); 
                        socketCliente.Close();
                    }
                    break;
                case "1":
                    Console.Clear();
                    juegoService.VerCatalogoJuegos();
                    MenuServidor(listener);
                    break;
                case "2":
                    Console.Clear();
                    usuarioService.ObtenerUsuario(Usuario.CrearUsuario());
                    MenuServidor(listener);
                    break;
                case "3":
                    Console.Clear();
                    usuarioService.VerListaUsuario();
                    MenuServidor(listener);
                    break;
                case "4":
                    Console.Clear();
                    string nombreUsuario = Usuario.CrearUsuario().NombreUsuario;
                    Console.WriteLine("Ingrese un nuevo nombre");
                    string nuevoNombreUsuario = Console.ReadLine();
                    Task<bool> seModifico = usuarioService.ModificarUsuario(nombreUsuario, nuevoNombreUsuario);
                    if (!seModifico.Result)
                    {
                        Console.WriteLine("Usuario invalido");
                    }
                    else
                    {
                        Console.WriteLine("Usuario Modificado");
                    }
                    MenuServidor(listener);
                    break;
                case "5":
                    Console.Clear();
                    Task<bool> seElimino = usuarioService.EliminarUsuario(Usuario.CrearUsuario().NombreUsuario); //esto deberia ser await 
                    if (!seElimino.Result)
                    {
                        Console.WriteLine("Usuario invalido");
                    }
                    else
                    {
                        Console.WriteLine("Usuario Eliminado");
                    }
                    MenuServidor(listener);
                    break;
            }
        }

        private async Task EscucharPorUsuario(TcpListener listener)
        {
            try
            {
                while (!salir)
                {
                    handler = await listener.AcceptTcpClientAsync();
                    Task hiloPorUsuario = new Task(async () => await ConexionUsuario(handler));
                    hiloPorUsuario.Start();
                    clientesConectados.Add(handler);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Conexion finalizada por una maquina sin usuario");

                Console.WriteLine("Presione enter si desea finalizar la conexion del servidor....");
            }
        }

        private async Task ConexionUsuario(TcpClient socket)
        {
            Usuario usuario = null;

            while (!salir)
            {
                try
                {
                    usuario = await EjecutarAccion(usuario, socket);
                }
                catch (SocketException)
                {
                    usuarioService.ActualizarAUsuarioInactivo(usuario.NombreUsuario);
                    return;
                }
            }
        }

        private async Task<Usuario> EjecutarAccion(Usuario usuario, TcpClient handler)
        {
                Encabezado encabezado = await Controlador.RecibirEncabezadoAsync(new Transferencia(handler));
                Transferencia transferencia = new Transferencia(handler);

                funcionalidadesServidor = new Funcionalidad(transferencia);

                string accion = encabezado.accion;
                int largoMensajeARecibir = encabezado.largoMensaje;

                switch (accion)
                {
                    case Accion.Login:
                        return await funcionalidadesServidor.InicioSesionCliente(usuario, largoMensajeARecibir);
                    case Accion.AdquirirJuego:
                        await funcionalidadesServidor.AdquirirJuego(largoMensajeARecibir, usuario);
                        break;
                    case Accion.VerJuegosAdquiridos:
                        await funcionalidadesServidor.VerJuegosAdquiridos(largoMensajeARecibir, usuario);
                        break;
                    case Accion.PublicarJuego:
                        await funcionalidadesServidor.CrearJuego(largoMensajeARecibir);
                        break;
                    case Accion.ListaJuegos:
                        await funcionalidadesServidor.EnviarListaJuegos();
                        break;
                    case Accion.PedirDetalleJuego:
                        await funcionalidadesServidor.EnviarDetalleDeUnJuego(largoMensajeARecibir);
                        break;
                    case Accion.PublicarCalificacion:
                        await funcionalidadesServidor.CrearCalificacion(largoMensajeARecibir);
                        break;
                    case Accion.BuscarTitulo:
                        await funcionalidadesServidor.BuscarJuegoPorTitulo(largoMensajeARecibir);
                        break;
                    case Accion.BuscarGenero:
                        await funcionalidadesServidor.BuscarJuegoPorGenero(largoMensajeARecibir);
                        break;
                    case Accion.BuscarCalificacion:
                        await funcionalidadesServidor.BuscarJuegoPorCalificacion(largoMensajeARecibir);
                        break;
                    case Accion.EliminarJuego:
                        await funcionalidadesServidor.EliminarJuego(largoMensajeARecibir);
                        break;
                    case Accion.ModificarJuego:
                        await funcionalidadesServidor.ModificarJuego(largoMensajeARecibir);
                        break;
                }
            return usuario;
        }
        
    }
}
