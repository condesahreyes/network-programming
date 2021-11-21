using LogicaNegocio;
using System.Collections.Generic;

namespace WebApiAdministrativa.Modelos.UsuarioModelos
{
    public class UsuarioEntradaSalida
    {
        public string NombreUsuario { get; set; }

        public UsuarioEntradaSalida() { }

        /*public UsuarioEntradaSalida(string nombreUsuario)
        {
            this.NombreUsuario = nombreUsuario;
        }*/

        public static Usuario ModeloADominio(UsuarioEntradaSalida unUsuario)
        {
            return new Usuario(unUsuario.NombreUsuario);
        }

        public static UsuarioEntradaSalida DominioAModelo(Usuario unUsuario)
        {
            return new UsuarioEntradaSalida()
            {
                NombreUsuario = unUsuario.NombreUsuario
            };
        }
        public static List<UsuarioEntradaSalida> ListarUsuarioModelo(List<Usuario> usuarios)
        {
            List<UsuarioEntradaSalida> usuariosModelo = new List<UsuarioEntradaSalida>();

            usuarios.ForEach(u => usuariosModelo.Add(UsuarioEntradaSalida.DominioAModelo(u)));

            return usuariosModelo;
        }

    }
}
