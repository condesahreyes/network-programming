using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
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
        private List<Socket> ConnectedClients = new List<Socket>();
        private Funcionalidad funcionalidadesServidor;
        private Socket handler;

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
        }

        public void StartListening()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipServidor), puerto);

            Socket listener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(endPoint);

            // escuchar por conexiones entrantes
            listener.Listen(cantConexionesEnEspera);

            Thread threadProcessor = new Thread(() => EscucharPorUsuario(listener));
            threadProcessor.Start();

            Console.WriteLine("Bienvenido al server, presione enter para terminar la conexion....");
            Console.ReadLine();
            salir = true;

            listener.Close(0);

            foreach (var socketClient in ConnectedClients)
            {
                    socketClient.Shutdown(SocketShutdown.Both);
                    socketClient.Close(1); 
            }
        }

        private void EscucharPorUsuario(Socket listener)
        {
            Usuario usuario = null;
            try
            {
                while (!salir)
                {
                    Console.WriteLine("Esperando por conexiones....");
                    handler = listener.Accept();

                    Transferencia transferencia = new Transferencia(handler);
                    ConnectedClients.Add(handler);

                    funcionalidadesServidor = new Funcionalidad(transferencia);

                    while (!salir)
                    {
                        Encabezado encabezado = Controlador.RecibirEncabezado(transferencia);
                        EjecutarAccion(encabezado, ref usuario);
                    }
                }
            }

            catch (SocketException)
            {
                if(usuario != null)
                    Console.WriteLine("Conexion finalizada por usuario: " + usuario.NombreUsuario);
                else
                    Console.WriteLine("Conexion finalizada por una maquina sin usuario");

                Console.WriteLine("Presione enter si desea finalizar la conexion del servidor....");
            }
        }

        private void EjecutarAccion(Encabezado encabezado, ref Usuario usuario)
        {
            string accion = encabezado.accion;
            int largoMensajeARecibir = encabezado.largoMensaje;
            
            switch (accion)
            {
                case Accion.Login:
                    usuario = funcionalidadesServidor.InicioSesionCliente(usuario, largoMensajeARecibir);
                    break;
                case Accion.PublicarJuego:
                    funcionalidadesServidor.CrearJuego(largoMensajeARecibir);
                    break;
                case Accion.ListaJuegos:
                    funcionalidadesServidor.EnviarListaJuegos();
                    break;
                case Accion.PedirDetalleJuego:
                    funcionalidadesServidor.EnviarDetalleDeUnJuego(largoMensajeARecibir);
                    break;
                case Accion.PublicarCalificacion:
                    funcionalidadesServidor.CrearCalificacion(largoMensajeARecibir);
                    break;
                case Accion.BuscarTitulo:
                    funcionalidadesServidor.BuscarJuegoPorTitulo(largoMensajeARecibir);
                    break;
                case Accion.BuscarGenero:
                    funcionalidadesServidor.BuscarJuegoPorGenero(largoMensajeARecibir);
                    break;
                case Accion.BuscarCalificacion:
                    funcionalidadesServidor.BuscarJuegoPorCalificacion(largoMensajeARecibir);
                    break;
                case Accion.EliminarJuego:
                    funcionalidadesServidor.EliminarJuego(largoMensajeARecibir);
                    break;
                case Accion.ModificarJuego:
                    funcionalidadesServidor.ModificarJuego(largoMensajeARecibir);
                    break;
            }
        }
    }
}
