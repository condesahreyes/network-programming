using LogicaNegocio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Cliente
{
    public class ConexionCliente
    {
        Socket sender;
        int port = 9000;
        IPEndPoint ipEndPoint;

        public ConexionCliente()
        {
            int port = 9000;

            ipEndPoint = new IPEndPoint(IPAddress.Loopback, port);
            sender = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


            // Conectarse desde un socket client
            sender.Connect(ipEndPoint);
            Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());

        }

        public  void EnvioDeUsuarioAServidor(Usuario usuario)
        {
            byte[] message = Encoding.ASCII.GetBytes(usuario.NombreUsuario + " <EOF>");

            // enviar mensaje al server
            int bytesSent = sender.Send(message);

            // recibir respuesta del server
            byte[] bytes = new byte[1024];
            int bytesReceived = sender.Receive(bytes);

            // Parsear texto recibido
            if (bytesReceived > 0)
            {
                Console.WriteLine("Mensaje recibido = {0}",
                Encoding.ASCII.GetString(bytes, 0, bytesReceived));
            }

            //sender.Shutdown(SocketShutdown.Both);
            //sender.Close();
        }

        public void EnvioDeTextoGenerico(string mensaje)
        {
            byte[] mensajeByte = Encoding.ASCII.GetBytes(mensaje + " <EOF>");

            //sender = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // enviar mensaje al server
            int bytesSent = sender.Send(mensajeByte);

        }

        public void EnvioDeObjetos(byte[] objeto)
        {
            byte[] mensajeByte = Encoding.ASCII.GetBytes("crearJuego" + objeto + " <EOF>");

            //sender = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // enviar mensaje al server
            int bytesSent = sender.Send(mensajeByte);
            Console.WriteLine("llegue");
            Console.ReadLine();

        }

        public void DesconectarUsuario(Usuario usuario)
        {
            EnvioDeTextoGenerico("Desconectar " + usuario.NombreUsuario);
            sender.Shutdown(SocketShutdown.Both);
            Console.WriteLine("Se desconectoooo");
            Console.ReadLine();
            sender.Close();
            //sender.Disconnect(true);
        }

        public void MandarNuevoJuego(Juego juego)
        {
            EnvioDeObjetos(ObjetoEnByte(juego));
        }

        private byte[] ObjetoEnByte(Object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

    }
}
