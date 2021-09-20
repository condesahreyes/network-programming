using System.Net.Sockets;
using System.Threading;
using LogicaNegocio;
using System.Net;
using Protocolo;
using System;

namespace Servidor
{
    public class Conexion
    {
        private readonly int numeroPuerto = 9000;
        private readonly int cantConexionesEnEspera = 10;
        private Funcionalidad funcionalidadesServidor;
        Socket handler;

        public Conexion() { }

        public string StartListening()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, numeroPuerto);

            Socket listener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(endPoint);
            // escuchar por conexiones entrantes
            listener.Listen(cantConexionesEnEspera);

            while (true)
            {
                Console.WriteLine("Esperando por conexiones....");
                handler = listener.Accept();

                Thread threadProcessor = new Thread(() => EscucharPorUsuario(handler));
                threadProcessor.Start();
            }
        }

        private void EscucharPorUsuario(Socket handler)
        {
            Transferencia transferencia = new Transferencia(handler);
            Usuario usuario = null;

            funcionalidadesServidor = new Funcionalidad(transferencia);

            while (true) {
                Encabezado encabezado = Controlador.RecibirEncabezado(transferencia);

                EjecutarAccion(encabezado, ref usuario);
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
