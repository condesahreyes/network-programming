using LogicaNegocio;
using Protocolo;
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

        public  void EnvioEncabezado(Usuario usuario)
        {
            int largoMensaje = usuario.NombreUsuario.Length;
            string mensajeAEnviar = usuario.NombreUsuario;

            EnvioDeEncabezado(Accion.Login, largoMensaje);

            //Console.ReadLine();
            //EnvioDeMensaje(mensajeAEnviar);
            
            //sender.Shutdown(SocketShutdown.Both);
            //sender.Close();
        }

        public void EnvioDeMensaje(string mensaje)
        {
            byte[] message = Encoding.ASCII.GetBytes(mensaje);
            int largoo = message.Length;
            // enviar mensaje al server
            int bytesSent = sender.Send(message);
            //sender.Shutdown(SocketShutdown.Both);
            //sender.Close();
        }

        public void EnvioDeEncabezado(string accion, int largoMensaje)
        {
            string mensaje = ""+largoMensaje;
            byte[] message = Encoding.ASCII.GetBytes(mensaje);

            // enviar mensaje al server
            int bytesSent = sender.Send(message);

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
            
        }

    }
}
