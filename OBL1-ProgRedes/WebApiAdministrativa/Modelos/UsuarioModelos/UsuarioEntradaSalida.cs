using LogicaNegocio;

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

    }
}
