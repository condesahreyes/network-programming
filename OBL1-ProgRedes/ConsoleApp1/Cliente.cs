using Cliente.Constantes;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Cliente
{
    public class Cliente
    {
        static async Task MainAsync(string[] args)
        {
            try
            {
                Menu menu = new Menu();
                await menu.MenuPrincipalAsync();
            }
            catch (Exception)
            {
                Mensaje.ConexionPerdida();
            }
        }
    }
}
