using LogicaNegocio;
using Protocolo;
using Servidor.Exception;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

        public SocketServidor(FuncionalidadesServidor servidor)
        {
            this.funcionalidadesServidor = servidor;
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
            while (true) {
                Encabezado encabezado = TransferenciaDatos.RecibirEncabezado(transferencia);

                EjecutarAccion(encabezado);
            }
        }

        private void EjecutarAccion(Encabezado encabezado)
        {
            string accion = encabezado.accion;
            int largoMensajeARecibir = encabezado.largoMensaje;
            
            switch (accion)
            {
                case AccionesConstantes.Login:
                    usuario = TransferenciaDatos.RecibirUsuario(transferencia, largoMensajeARecibir);
                    funcionalidadesServidor.InicioSesionCliente(usuario);
                    break;

                case AccionesConstantes.PublicarJuego:
                    Juego juego = TransferenciaDatos.PublicarJuego(transferencia, largoMensajeARecibir);
                    try
                    {
                        bool creo = funcionalidadesServidor.CrearJuego(juego);

                        if(creo)
                            TransferenciaDatos.EnviarMensajeClienteOk(transferencia);
                        else
                            TransferenciaDatos.EnviarMensajeClienteError(transferencia);

                    }
                    catch (JuegoExistente){
                        TransferenciaDatos.EnviarMensajeClienteError(transferencia);
                    }

                    break;
            }
        }
    }
}
