using LogicaNegocio;
using Protocolo;
using Servidor.Exception;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Servidor
{
    public class SocketServidor
    {
        private readonly int numeroPuerto = 9000;
        private readonly int cantConexionesEnEspera = 10;
        private FuncionalidadesServidor funcionalidadesServidor;
        Socket handler;
        Transferencia transferencia;
        Usuario usuario;

        public SocketServidor()
        {
        }

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
                transferencia = new Transferencia(handler);
                Thread threadProcessor = new Thread(() => EscucharPorUsuario(handler));
                threadProcessor.Start();
            }
        }

        private void EscucharPorUsuario(Socket handler)
        {
            funcionalidadesServidor = new FuncionalidadesServidor(transferencia);
            while (true) {
                Encabezado encabezado = ControladorDeTransferencia.RecibirEncabezado(transferencia);

                EjecutarAccion(encabezado);
            }
        }

        private void EjecutarAccion(Encabezado encabezado)
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
