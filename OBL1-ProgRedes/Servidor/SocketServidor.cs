using Protocolo;
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
        private FuncionalidadesServidor servidor;
        Socket handler;


        public SocketServidor(FuncionalidadesServidor servidor)
        {
            this.servidor = servidor;
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
                Thread threadProcessor = new Thread(() => EscucharPorUsuario(handler));
                threadProcessor.Start();
            }
        }

        private void EscucharPorUsuario(Socket handler)
        {
            int mensajeAccionLargo = TransferenciaDatos.EscucharPorEncabezado(handler);

            TransferenciaDatos.EscucharPorMensajes(handler, mensajeAccionLargo);
        }

        public void ValidarAccionDelCliente(string accion, byte[] bytes)
        {
            String[] text = accion.Split(" ");
            Console.WriteLine(text[0]);

            if (text[0]== "conectar")
                EnvioConfirmacionCliente(text[1]);            
            if (text[0]== "Desconectar")
                Desconectar(text[1]);
            if (text[0] == "crearJuego")
                servidor.CrearJuego(bytes);
        }

        private void Desconectar(string usuarioDesconectado)
        {
            Console.WriteLine("El usuairo " + usuarioDesconectado+" se ha desconectado");
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        public void EnvioConfirmacionCliente(string data)
        {
            servidor.InicioSesionCliente(data);
            byte[] msg = Encoding.ASCII.GetBytes("Usted inicio sesion en el servidor con " + data);
            handler.Send(msg);
            Console.ReadLine();
        }
    }
}
