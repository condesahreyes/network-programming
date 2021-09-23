using Cliente.Constantes;
using System;
using System.Net.Sockets;

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
            }

            catch (SocketException)
            {
                Mensaje.ConexionPerdida();
            }
        }
    }
}
