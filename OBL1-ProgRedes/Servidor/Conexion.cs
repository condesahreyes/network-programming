using Microsoft.Extensions.Configuration;
using Servidor.FuncionalidadesPorEntidad;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using LogicaNegocio;
using System.Net;
using Protocolo;
using System.IO;
using System;

namespace Servidor
{
    public class Conexion
    {
        List<TcpClient> clientesConectados = new List<TcpClient>();
        //private List<Socket> clientesConectados = new List<Socket>();
        private Funcionalidad funcionalidadesServidor;
        //private Socket handler;
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
            //Escuchar(); //Esto esta bien? ver prox comentario 
        }

        public async Task Escuchar() //Esto deberia ser async?????
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipServidor), puerto);

            TcpListener listener = new TcpListener(endPoint);

            //listener.Bind(endPoint);

            //Se cambia Listen por Start debido al cambio de socket -> TcpListener
            listener.Start(cantConexionesEnEspera);


            Thread hiloDeEscucha = new Thread(async () => await EscucharPorUsuario(listener));
            hiloDeEscucha.Start();

            MenuServidor(listener);
        }
        

        private void MenuServidor(TcpListener listener)
        {
            LogicaJuego _logicaJuegos = new LogicaJuego();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("****************** Menú servidor ******************" +
                "\n0. Terminar la conexion. \n1. Ver catalogo de juegos \n\nSeleccione una opción:");
            Console.ForegroundColor = ConsoleColor.White;

            string accion = Console.ReadLine();
            if (!Regex.IsMatch(accion, "^[" + 0 + "-" + 1 + "]$"))
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

                    //listener.Close(0);
                    listener.Stop(); // esto esta bien????

                    foreach (var socketCliente in clientesConectados)
                    {
                        //Arreglar esto para tcpClient
                        //socketCliente.EndConnect();
                        socketCliente.GetStream().Close(); //Ta bien delia?
                        socketCliente.Close();//Ta bien delia?
                        //socketCliente.Shutdown(SocketShutdown.Both); //Entiendo esto se va
                        //socketCliente.Close(1); // Entiendo esto se va
                    }
                    break;
                case "1":
                    Console.Clear();
                    _logicaJuegos.VerCatalogoJuegos();
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
                    Thread hiloPorUsuario = new Thread(async () => await ConexionUsuario(handler));
                    hiloPorUsuario.Start();
                    clientesConectados.Add(handler);
                }
            }
            catch (SocketException)
            {
                /*
                if(usuario != null)
                    Console.WriteLine("Conexion finalizada por usuario: " + usuario.NombreUsuario);
                else
                    Console.WriteLine("Conexion finalizada por una maquina sin usuario");
                */
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
                    return;
                }
            }

        }

        private async Task<Usuario> EjecutarAccion(Usuario usuario, TcpClient handler)
        {
                Encabezado encabezado = await Controlador.RecibirEncabezado(new Transferencia(handler));
                Transferencia transferencia = new Transferencia(handler);

                funcionalidadesServidor = new Funcionalidad(transferencia);

                string accion = encabezado.accion;
                int largoMensajeARecibir = encabezado.largoMensaje;

                switch (accion)
                {
                    case Accion.Login:
                        return await funcionalidadesServidor.InicioSesionCliente(usuario, largoMensajeARecibir);
                        break;
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
