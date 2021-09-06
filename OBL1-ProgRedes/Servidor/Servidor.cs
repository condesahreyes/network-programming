using System;

namespace Servidor
{
    public class Servidor
    {
        static void Main(string[] args)
        {
            SocketServidor server = new SocketServidor();
            server.StartListening();
        }
    }
}
