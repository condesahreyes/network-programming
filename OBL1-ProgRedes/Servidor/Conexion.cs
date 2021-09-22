using System.Net.Sockets;
using System.Threading;
using LogicaNegocio;
using System.Net;
using Protocolo;
using System;
using System.Collections.Generic;

namespace Servidor
{
    public class Conexion
    {
        public static readonly List<Socket> ConnectedClients = new List<Socket>();
        public static bool exit = false;
        private readonly int numeroPuerto = 9000;
        private readonly int cantConexionesEnEspera = 10;
        private Funcionalidad funcionalidadesServidor;
        Socket handler;

        public Conexion() { }

        public void StartListening()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, numeroPuerto);

            Socket listener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(endPoint);
            // escuchar por conexiones entrantes
            listener.Listen(cantConexionesEnEspera);

            Thread threadProcessor = new Thread(() => EscucharPorUsuario(listener));
            threadProcessor.Start();

            Console.WriteLine("Bienvenido al server, presione enter para salir....");
            Console.ReadLine();
            exit = true;

            listener.Close(0);


            foreach (var socketClient in ConnectedClients)
            {
                    socketClient.Shutdown(SocketShutdown.Both);
                    socketClient.Close(1); 
            }
        }

        private void EscucharPorUsuario(Socket listener)
        {
            try
            {
                while (!exit)
                {
                    Console.WriteLine("Esperando por conexiones....");
                    handler = listener.Accept();


                    Transferencia transferencia = new Transferencia(handler);
                    Usuario usuario = null;

                    funcionalidadesServidor = new Funcionalidad(transferencia);

                    while (!exit)
                    {
                        Encabezado encabezado = Controlador.RecibirEncabezado(transferencia);


                        EjecutarAccion(encabezado, ref usuario);
                    }
                }
            }

            catch (SocketException e)
            {
                Console.WriteLine("Se perdió la conexión con el servidor: " + e.Message);
                Console.WriteLine("Presione enter para salir");
                Console.ReadLine();
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

            }
        }
    }
}
