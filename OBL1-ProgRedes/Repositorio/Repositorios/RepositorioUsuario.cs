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
            lock (persistencia.juegos)
            {
                this.persistencia.usuarios.Add(usuario);
            }
        }

        public List<Usuario> ObtenerUsuarios()
        {
            lock (persistencia.juegos)
            {
                return this.persistencia.usuarios;
            }
        }

        public void ActualizarEstadoUsuario(string nombreUsuario, bool estado)
        {
            lock (persistencia.juegos)
            {
                foreach (var usuario in this.persistencia.usuarios)
                    if (nombreUsuario == usuario.NombreUsuario)
                    {
                        usuario.UsuarioActivo = estado;
                        return;
                    }
            }
        }

        public bool EliminarUsuario(string nombreUsuario)
        {
            lock (persistencia.juegos)
            {
                foreach (var usuario in this.persistencia.usuarios)
                    if (nombreUsuario == usuario.NombreUsuario && usuario.UsuarioActivo == false)
                    {
                        this.persistencia.usuarios.Remove(usuario);
                        return true;
                    }
            }

            return false;
        }

        public bool ModificarUsuario(string nombreUsuario, string nombreUsuarioModificado)
        {
            if (ExisteUsuario(nombreUsuarioModificado))
                return false;

            lock (persistencia.juegos)
            {
                foreach (var usuario in this.persistencia.usuarios)
                    if (nombreUsuario == usuario.NombreUsuario && usuario.UsuarioActivo == false)
                    {
                        usuario.NombreUsuario = nombreUsuarioModificado;
                        return true;
                    }
            }
            return false;
        }

        private bool ExisteUsuario(string nombreUsuarioModificado)
        {
            List<Usuario> listaUsuarios = new List<Usuario>();
            lock (persistencia.usuarios)
            {
                listaUsuarios = persistencia.usuarios;
            }

            return listaUsuarios.Exists(u => u.NombreUsuario == nombreUsuarioModificado);
        }
    }
}
