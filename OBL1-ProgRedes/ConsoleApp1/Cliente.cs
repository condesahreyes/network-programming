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
                menu.MenuPrincipal();
            }

            catch (SocketException e)
            {
                Console.WriteLine("Se perdió la conexión con el servidor: " + e.Message);
                Console.WriteLine("Presione enter para salir");
                Console.ReadLine();
            }
        }
    }
}
