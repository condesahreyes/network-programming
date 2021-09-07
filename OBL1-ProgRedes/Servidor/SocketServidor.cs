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

        public void StartListening()
        {
            // 127.0.0.1:9000
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, numeroPuerto);

            // Crear Socket TCP/IP
            Socket listener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Asociar el socket con la direccion ip y el puerto
            listener.Bind(endPoint);
            // escuchar por conexiones entrantes
            listener.Listen(cantConexionesEnEspera);

            while (true)
            {
                Console.WriteLine("Esperando por conexiones....");
                var handler = listener.Accept();
                Thread threadProcessor = new Thread(() => HandleReceivedClients(handler));
                threadProcessor.Start();
            }

        }

        private void HandleReceivedClients(Socket handler)
        {

            byte[] bytes = new byte[1024];

            string data = null;

            while (true)
            {
                int bytesRec = handler.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                if (data.IndexOf("<EOF>") > -1)
                {
                    break;
                }
            }
            Console.WriteLine("Texto recibido : {0}", data);

            // Echo the data back to the client.  
            byte[] msg = Encoding.ASCII.GetBytes("mensaje desde el server...");
            handler.Send(msg);

            handler.Shutdown(SocketShutdown.Both);

            handler.Close();
        }
    }
}
