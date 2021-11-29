using System.Collections.Generic;
using System.Threading.Tasks;
using LogicaNegocio;
using IServices;
using Servicios;
using System;

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

        public async Task VerListaUsuarioAsync()
        {
            List<Usuario> usuarios = await usuarioService.ObtenerUsuariosAsync();

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

        public async Task VerCatalogoJuegosAsync()
        {
            List<Juego> juegos = await juegoService.ObtenerJuegosAsync();

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
