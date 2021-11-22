using System.Collections.Generic;
using System.Threading.Tasks;
using LogicaNegocio;

namespace IServices
{
    public interface IUsuarioService
    {
        Task ActualizarAUsuarioInactivo(string nombreUsuario);

        Task ActualizarAUsuarioActivo(string nombreUsuario);

        Task<Usuario> ObtenerUsuario(Usuario usuario);

        Task<bool> EliminarUsuario(string nombreUsuario);

        Task<bool> ModificarUsuario(string nombreUsuario, string nuevoNombreUsuario);

        Task<List<Usuario>> ObtenerUsuarios();
    }
}
