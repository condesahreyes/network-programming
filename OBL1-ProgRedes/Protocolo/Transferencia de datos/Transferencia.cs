using System.Net.Sockets;
using System.Text;

namespace Protocolo
{
    public class Transferencia
    {
        private Socket socket;

        public Transferencia(Socket socket)
        {
            this.socket = socket;
        }

        public byte[] RecibirDatos(int largoMensaje)
        {
            int recibidoTotal = 0;

            byte[] datos = new byte[largoMensaje];

            while (recibidoTotal < largoMensaje)
            {
                var recibido = socket.Receive(datos, recibidoTotal,
                    largoMensaje - recibidoTotal, SocketFlags.None);

                recibidoTotal += recibido;
                
                if (recibido == 0)
                {
                    throw new SocketException();
                }
            }
            return datos;
        }

        public void EnvioDeDatos(string datos)
        {
            byte[] mensaje = Encoding.ASCII.GetBytes(datos);

            int largoMensaje = mensaje.Length;
            int enviados = 0;

            while (enviados < largoMensaje)
                enviados += socket.Send(mensaje, enviados, largoMensaje - enviados, SocketFlags.None);
        }

        public void Desconectar()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }


    }
}