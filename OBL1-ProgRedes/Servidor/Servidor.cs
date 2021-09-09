using System;

namespace Servidor
{
    public class Servidor
    {
        private static FuncionalidadesServidor funcionalidadesServidor;
        static void Main(string[] args)
        {
            funcionalidadesServidor = new FuncionalidadesServidor();
            SocketServidor server = new SocketServidor(funcionalidadesServidor);
            server.StartListening();

        }
    }
}
