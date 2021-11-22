using System.Collections.Generic;
using LogicaNegocio;
using IRepositorio;

namespace Repositorio
{
    public class RepositorioUsuario : IRepositorioUsuario
    {
        Persistencia persistencia;

        public RepositorioUsuario()
        {
            this.persistencia = Persistencia.ObtenerPersistencia();
        }

        public void AgregarUsuario(Usuario usuario)
        {
            this.persistencia.usuarios.Add(usuario);
        }

        public List<Usuario> ObtenerUsuarios()
        {
            return this.persistencia.usuarios;
        }

        public void ActualizarEstadoUsuario(string nombreUsuario, bool estado)
        {
            foreach (var usuario in this.persistencia.usuarios)
                if (nombreUsuario == usuario.NombreUsuario)
                {
                    usuario.UsuarioActivo = estado;
                    return;
                }
        }

        public bool EliminarUsuario(string nombreUsuario)
        {
            foreach (var usuario in this.persistencia.usuarios)
                if (nombreUsuario == usuario.NombreUsuario && usuario.UsuarioActivo == false)
                {
                    this.persistencia.usuarios.Remove(usuario);
                    return true;
                }

            return false;
        }

        public bool ModificarUsuario(string nombreUsuario, string nombreUsuarioModificado)
        {
            foreach (var usuario in this.persistencia.usuarios)
                if (nombreUsuario == usuario.NombreUsuario && usuario.UsuarioActivo == false)
                {
                    usuario.NombreUsuario = nombreUsuarioModificado;
                    return true;
                }

            return false;
        }
    }
}
