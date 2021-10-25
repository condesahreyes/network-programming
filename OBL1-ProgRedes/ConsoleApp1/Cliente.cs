using Cliente.Constantes;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Cliente
{
    public class Cliente
    {
        static async Task Main(string[] args)
        {
            try
            {
                Menu menu = new Menu();
                await Task.Run(() => menu.MenuPrincipal());
            }
            catch (Exception)
            {
                Mensaje.ConexionPerdida();
            }
        }
    }
}
