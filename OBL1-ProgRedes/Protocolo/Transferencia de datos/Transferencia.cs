using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;
using System;

namespace Protocolo
{
    public class Transferencia
    {
        public TcpClient socket;
        private NetworkStream networkStream ;

        public Transferencia(TcpClient socket)
        {
            this.socket = socket;
            this.networkStream = socket.GetStream();
        }

        public async Task<byte[]> RecibirDatosAsync(int largoMensaje)
        {
            int recibidoTotal = 0;

            byte[] datos = new byte[largoMensaje];

            while (recibidoTotal < largoMensaje)
            {
                var recibido =  await networkStream.ReadAsync(datos, recibidoTotal,
                    largoMensaje - recibidoTotal);

                recibidoTotal += recibido;
                
                if (recibido == 0)
                {
                    throw new SocketException();
                }
            }
            return datos;
        }

        public async Task EnvioDeDatosAsync(string datos)
        {
            byte[] mensaje = Encoding.ASCII.GetBytes(datos);

            await EnvioDeDatosByteAsync(mensaje);
        }

        public async Task EnvioDeDatosByteAsync(byte[] mensaje)
        {
            int largoMensaje = mensaje.Length;
            int enviados = 0;
           await networkStream.WriteAsync(mensaje, enviados, largoMensaje - enviados);
        }

        public void Desconectar()
        {
            networkStream.Close(); 
            socket.Close();
        }
    }
}