using System.Collections.Generic;
using LogicaNegocio;

namespace IRepositorio
{
    public interface IRepositorioUsuario
    {
        void AgregarUsuario(Usuario usuario);

        List<Usuario> ObtenerUsuarios();

        void ActualizarEstadoUsuario(string nombreUsuario, bool estado);

        bool EliminarUsuario(string nombreUsuario);

        bool ModificarUsuario(string nombreUsuario, string nombreUsuarioModificado);
    }
}
