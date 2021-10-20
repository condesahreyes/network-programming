using System.Threading.Tasks;

namespace Servidor
{
    public class Servidor
    {
        static async Task Main(string[] args)
        {
            Conexion server = new Conexion();
            await server.Escuchar();
        }
    }
}