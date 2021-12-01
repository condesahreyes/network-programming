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
        
        public async Task EscucharAsync()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipServidor), puerto);

            TcpListener listener = new TcpListener(endPoint);

            listener.Start(cantConexionesEnEspera);

            Task hiloDeEscucha = new Task(async () => await EscucharPorUsuarioAsync(listener));
            hiloDeEscucha.Start();

            await MenuServidorAsync(listener);
        }

        private async Task MenuServidorAsync(TcpListener listener)
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
                await MenuServidorAsync(listener);
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
                    await funcionalidadesServidor.VerCatalogoJuegosAsync();
                    await MenuServidorAsync(listener);
                    break;
                case "2":
                    Console.Clear();
                    await usuarioService.ObtenerUsuarioAsync(Usuario.CrearUsuario());
                    await MenuServidorAsync (listener);
                    break;
                case "3":
                    Console.Clear();
                    await funcionalidadesServidor.VerListaUsuarioAsync();
                    await MenuServidorAsync(listener);
                    break;
                case "4":
                    Console.Clear();
                    string nombreUsuario = Usuario.CrearUsuario().NombreUsuario;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Ingrese un nuevo nombre");
                    Console.ForegroundColor = ConsoleColor.White;
                    string nuevoNombreUsuario = Console.ReadLine();
                    bool seModifico = await usuarioService.ModificarUsuarioAsync(nombreUsuario, nuevoNombreUsuario);
                    Console.WriteLine((!seModifico) ? "Usuario invalido o activo" : "Usuario Modificado");
                    await MenuServidorAsync(listener);
                    break;
                case "5":
                    Console.Clear();
                    bool seElimino = await usuarioService.EliminarUsuarioAsync(Usuario.CrearUsuario().NombreUsuario);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine((!seElimino) ? "Usuario invalido o activo" : "Usuario Eliminado");

                    await MenuServidorAsync(listener);
                    break;
            }
        }

        private async Task EscucharPorUsuarioAsync(TcpListener listener)
        {
            try
            {
                while (!salir)
                {
                    handler = await listener.AcceptTcpClientAsync();
                    Task hiloPorUsuario = new Task(async () => await ConexionUsuarioAsync(handler));
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

        private async Task ConexionUsuarioAsync(TcpClient socket)
        {
            Usuario usuario = null;

            while (!salir)
            {
                try
                {
                    usuario = await EjecutarAccionAsync(usuario, socket);
                }
                catch (SocketException)
                {
                    await usuarioService.ActualizarAUsuarioInactivoAsync(usuario.NombreUsuario);
                    return;
                }
            }
        }

        private async Task<Usuario> EjecutarAccionAsync(Usuario usuario, TcpClient handler)
        {
                Encabezado encabezado = await Controlador.RecibirEncabezadoAsync(new Transferencia(handler));
                Transferencia transferencia = new Transferencia(handler);

                funcionalidadesCliente = new FuncionalidadCliente(transferencia);

                string accion = encabezado.accion;
                int largoMensajeARecibir = encabezado.largoMensaje;

                switch (accion)
                {
                    case Accion.Login:
                        return await funcionalidadesCliente.InicioSesionClienteAsync(usuario, largoMensajeARecibir);
                    case Accion.AdquirirJuego:
                        await funcionalidadesCliente.AdquirirJuegoAsync(largoMensajeARecibir, usuario);
                        break;
                    case Accion.VerJuegosAdquiridos:
                        await funcionalidadesCliente.VerJuegosAdquiridosAysnc(largoMensajeARecibir, usuario);
                        break;
                    case Accion.PublicarJuego:
                        await funcionalidadesCliente.CrearJuegoAysnc(largoMensajeARecibir);
                        break;
                    case Accion.ListaJuegos:
                        await funcionalidadesCliente.EnviarListaJuegosAysnc();
                        break;
                    case Accion.PedirDetalleJuego:
                        await funcionalidadesCliente.EnviarDetalleDeUnJuegoAsync(largoMensajeARecibir);
                        break;
                    case Accion.PublicarCalificacion:
                        await funcionalidadesCliente.CrearCalificacionAsync(largoMensajeARecibir);
                        break;
                    case Accion.BuscarTitulo:
                        await funcionalidadesCliente.BuscarJuegoPorTituloAsync(largoMensajeARecibir);
                        break;
                    case Accion.BuscarGenero:
                        await funcionalidadesCliente.BuscarJuegoPorGeneroAsync(largoMensajeARecibir);
                        break;
                    case Accion.BuscarCalificacion:
                        await funcionalidadesCliente.BuscarJuegoPorCalificacionAsync(largoMensajeARecibir);
                        break;
                    case Accion.EliminarJuego:
                        await funcionalidadesCliente.EliminarJuegoAsync(largoMensajeARecibir);
                        break;
                    case Accion.ModificarJuego:
                        await funcionalidadesCliente.ModificarJuegoAsync(largoMensajeARecibir);
                        break;
                }
            return usuario;
        }
        
    }
}
