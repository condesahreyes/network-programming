using Cliente.Constantes;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Cliente
{
    public class Cliente
    {
        private static Menu menu;

        static void Main(string[] args)
        {
            try
            {
                menu = new Menu();
                //var threadClient = new Thread(() => menu = new Menu());
                //threadClient.Start();
            }

            catch (SocketException)
            {
                Mensaje.ConexionPerdida();
            }
        }
    }
}
