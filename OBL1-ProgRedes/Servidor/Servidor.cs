using System.Threading.Tasks;

namespace Servidor
{
    public class Servidor
    {
        static async Task MainAsync(string[] args)
        {
            Conexion server = new Conexion();
            await server.EscucharAsync();
        }
    }
}