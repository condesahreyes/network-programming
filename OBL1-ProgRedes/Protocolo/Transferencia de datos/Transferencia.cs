using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<byte[]> RecibirDatos(int largoMensaje)
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

        public async Task EnvioDeDatos(string datos)
        {
            byte[] mensaje = Encoding.ASCII.GetBytes(datos);

            await EnvioDeDatosByte(mensaje);
        }

        public async Task EnvioDeDatosByte(byte[] mensaje)
        {
            int largoMensaje = mensaje.Length;
            int enviados = 0;

            /*while (enviados < largoMensaje)
                enviados +=*/ 
            await networkStream.WriteAsync(mensaje, enviados, largoMensaje - enviados);
        }

        public void Desconectar()
        {
            //socket.Shutdown(SocketShutdown.Both); //Pasar esto al metodo async
            networkStream.Close(); //Preguntar a delia
            socket.Close();
        }
    }
}