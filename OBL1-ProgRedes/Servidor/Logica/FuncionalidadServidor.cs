using IServices;
using LogicaNegocio;
using Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servidor.Logica
{
    public class FuncionalidadServidor
    {
        private IUsuarioService usuarioService;
        private IJuegoService juegoService;

        public FuncionalidadServidor()
        {
            this.usuarioService = new UsuarioService();
            this.juegoService = new JuegoService();
        }

        public async Task VerListaUsuario()
        {
            List<Usuario> usuarios = await usuarioService.ObtenerUsuarios();

            if (usuarios.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No se han ingresados usuarios");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (var usuario in usuarios)
                Console.WriteLine(usuario.NombreUsuario);
        }

        public async Task VerCatalogoJuegos()
        {
            List<Juego> juegos = await juegoService.ObtenerJuegos();

            if (juegos.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No se han ingresados juegos");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (var juego in juegos)
                Console.WriteLine(juego.ToString());
            
        }
    }
}
