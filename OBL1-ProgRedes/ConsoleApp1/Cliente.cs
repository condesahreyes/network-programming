using Cliente.Constantes;
using System.Net.Sockets;

namespace Cliente
{
    public class Cliente
    {
        static void Main(string[] args)
        {
            try
            {
                Menu menu = new Menu();
            }

            catch (SocketException)
            {
                Mensaje.ConexionPerdida();
            }
        }
    }
}
