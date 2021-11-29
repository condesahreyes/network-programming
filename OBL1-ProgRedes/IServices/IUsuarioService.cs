using System.Collections.Generic;
using System.Threading.Tasks;
using LogicaNegocio;

namespace IServices
{
    public interface IUsuarioService
    {
        Task ActualizarAUsuarioInactivoAsync(string nombreUsuario);

        Task ActualizarAUsuarioActivoAsync(string nombreUsuario);

        Task<Usuario> ObtenerUsuarioAsync(Usuario usuario);

        Task<bool> EliminarUsuarioAsync(string nombreUsuario);

        Task<bool> ModificarUsuarioAsync(string nombreUsuario, string nuevoNombreUsuario);

        Task<List<Usuario>> ObtenerUsuariosAsync();
    }
}
