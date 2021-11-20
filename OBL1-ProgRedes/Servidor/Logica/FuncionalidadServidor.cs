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

        public FuncionalidadServidor()
        {
            this.usuarioService = new UsuarioService();
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
    }
}
