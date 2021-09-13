using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Protocolo
{
    public static class TransferenciaDatos
    {

        public static int EscucharPorEncabezado(Socket socket)
        {
            byte[] dataLength = new byte[1];
            int receivedTotal = 0;
            try
            {
                while (receivedTotal < 1)
                {
                    var received = 0;
                    received += socket.Receive(dataLength, receivedTotal, 1 - receivedTotal, SocketFlags.None);
                    if (received == 0)
                    {
                        throw new SocketException();
                    }
                    receivedTotal += received;
                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                {
                    Thread.Sleep(100);
                }
            }
            string returndata = Encoding.ASCII.GetString(dataLength, 0, dataLength.Length);
            int length = Convert.ToInt32(returndata);

            return length;
        }

        public static void EscucharPorMensajes(Socket socket, int largoMensaje)
        {
            int receivedTotal = 0;

            byte[] data = new byte[largoMensaje];

            while (receivedTotal < largoMensaje)
            {
                var received = socket.Receive(data, receivedTotal, largoMensaje - receivedTotal, SocketFlags.None);
                receivedTotal += received;
            }

            string message = Encoding.UTF8.GetString(data, 0, largoMensaje); //Borrar en algun momento
            Console.WriteLine(message); //Borrar en algun momento
        }


    }
}
