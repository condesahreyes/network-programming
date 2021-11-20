using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using Servidor.Logica;
using LogicaNegocio;
using System.Net;
using Protocolo;
using System.IO;
using IServices;
using Servicios;
using System;

namespace Servidor
{
    public class Conexion
    {
        List<TcpClient> clientesConectados = new List<TcpClient>();

        private FuncionalidadServidor funcionalidadesServidor = new FuncionalidadServidor();
        private FuncionalidadCliente funcionalidadesCliente;

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
        }
        
        public async Task Escuchar()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipServidor), puerto);

            TcpListener listener = new TcpListener(endPoint);

            listener.Start(cantConexionesEnEspera);

            Task hiloDeEscucha = new Task(async () => await EscucharPorUsuario(listener));
            hiloDeEscucha.Start();

            await MenuServidor(listener);
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
                await MenuServidor(listener);
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
                    await funcionalidadesServidor.VerCatalogoJuegos();
                    await MenuServidor(listener);
                    break;
                case "2":
                    Console.Clear();
                    await usuarioService.ObtenerUsuario(Usuario.CrearUsuario());
                    await MenuServidor (listener);
                    break;
                case "3":
                    Console.Clear();
                    await funcionalidadesServidor.VerListaUsuario();
                    await MenuServidor(listener);
                    break;
                case "4":
                    Console.Clear();
                    string nombreUsuario = Usuario.CrearUsuario().NombreUsuario;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Ingrese un nuevo nombre");
                    Console.ForegroundColor = ConsoleColor.White;
                    string nuevoNombreUsuario = Console.ReadLine();
                    bool seModifico = await usuarioService.ModificarUsuario(nombreUsuario, nuevoNombreUsuario);
                    Console.WriteLine((!seModifico) ? "Usuario invalido o activo" : "Usuario Modificado");
                    await MenuServidor(listener);
                    break;
                case "5":
                    Console.Clear();
                    bool seElimino = await usuarioService.EliminarUsuario(Usuario.CrearUsuario().NombreUsuario);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine((!seElimino) ? "Usuario invalido o activo" : "Usuario Eliminado");

                    await MenuServidor(listener);
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
                    await usuarioService.ActualizarAUsuarioInactivo(usuario.NombreUsuario);
                    return;
                }
            }
        }

        private async Task<Usuario> EjecutarAccion(Usuario usuario, TcpClient handler)
        {
                Encabezado encabezado = await Controlador.RecibirEncabezadoAsync(new Transferencia(handler));
                Transferencia transferencia = new Transferencia(handler);

                funcionalidadesCliente = new FuncionalidadCliente(transferencia);

                string accion = encabezado.accion;
                int largoMensajeARecibir = encabezado.largoMensaje;

                switch (accion)
                {
                    case Accion.Login:
                        return await funcionalidadesCliente.InicioSesionCliente(usuario, largoMensajeARecibir);
                    case Accion.AdquirirJuego:
                        await funcionalidadesCliente.AdquirirJuego(largoMensajeARecibir, usuario);
                        break;
                    case Accion.VerJuegosAdquiridos:
                        await funcionalidadesCliente.VerJuegosAdquiridos(largoMensajeARecibir, usuario);
                        break;
                    case Accion.PublicarJuego:
                        await funcionalidadesCliente.CrearJuego(largoMensajeARecibir);
                        break;
                    case Accion.ListaJuegos:
                        await funcionalidadesCliente.EnviarListaJuegos();
                        break;
                    case Accion.PedirDetalleJuego:
                        await funcionalidadesCliente.EnviarDetalleDeUnJuego(largoMensajeARecibir);
                        break;
                    case Accion.PublicarCalificacion:
                        await funcionalidadesCliente.CrearCalificacion(largoMensajeARecibir);
                        break;
                    case Accion.BuscarTitulo:
                        await funcionalidadesCliente.BuscarJuegoPorTitulo(largoMensajeARecibir);
                        break;
                    case Accion.BuscarGenero:
                        await funcionalidadesCliente.BuscarJuegoPorGenero(largoMensajeARecibir);
                        break;
                    case Accion.BuscarCalificacion:
                        await funcionalidadesCliente.BuscarJuegoPorCalificacion(largoMensajeARecibir);
                        break;
                    case Accion.EliminarJuego:
                        await funcionalidadesCliente.EliminarJuego(largoMensajeARecibir);
                        break;
                    case Accion.ModificarJuego:
                        await funcionalidadesCliente.ModificarJuego(largoMensajeARecibir);
                        break;
                }
            return usuario;
        }
        
    }
}
