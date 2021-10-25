using System;
using System.Collections.Generic;
using LogicaNegocio;

namespace Servidor.FuncionalidadesEntidades
{

    public class LogicaUsuario
    {
        private Persistencia persistencia;

        public LogicaUsuario()
        {
            this.persistencia = Persistencia.ObtenerPersistencia();
        }

        public void ActualizarAUsuarioInactivo(string nombreUsuario)
        {
            lock (persistencia)
            {
                foreach (Usuario usuario in persistencia.usuarios)
                    if (usuario.NombreUsuario == nombreUsuario)
                        usuario.UsuarioActivo = false;
            }
        }

        public void ActualizarAUsuarioActivo(string nombreUsuario)
        {
            lock (persistencia)
            {
                foreach (Usuario usuario in persistencia.usuarios)
                    if (usuario.NombreUsuario == nombreUsuario)
                        usuario.UsuarioActivo = true;
            }
        }

        public Usuario ObtenerUsuario(Usuario usuario)
        {
            bool noExisteUsuario = NoEsUsuarioExistente(usuario);

            if (noExisteUsuario)
            {
                lock (persistencia)
                {
                    persistencia.usuarios.Add(usuario);
                }
            }

            return DevolverUsuarioExistente(usuario);
        }

        public bool EliminarUsuario(string nombreUsuario)
        {
            lock (persistencia.juegos)
            {
                foreach (Usuario unUsuario in persistencia.usuarios)
                    if (unUsuario.NombreUsuario == nombreUsuario && unUsuario.UsuarioActivo == false)
                    {
                        persistencia.usuarios.Remove(unUsuario);
                        return true;
                    }
                    else if(unUsuario.NombreUsuario == nombreUsuario && unUsuario.UsuarioActivo == true)
                    {
                        Console.WriteLine("Error el usuario no se puede eliminar dado que es un usuario activo");
                        return false;
                    }
            }
            return false;
        }

        public bool ModificarUsuario(string nombreUsuario, string nuevoNombreUsuario)
        {
            lock (persistencia.juegos)
            {
                foreach (Usuario unUsuario in persistencia.usuarios)
                    if (unUsuario.NombreUsuario == nombreUsuario && unUsuario.UsuarioActivo == false)
                    {
                        unUsuario.NombreUsuario = nuevoNombreUsuario;
                        return true;
                    }
                    else if (unUsuario.NombreUsuario == nombreUsuario && unUsuario.UsuarioActivo == true)
                    {
                        Console.WriteLine("Error el usuario no se puede modificar dado que es un usuario activo");
                        return false;
                    }
            }
            return false;

        }

        public void VerListaUsuario()
        {
            List<Usuario> usuarios;
            lock (persistencia.usuarios)
            {
                usuarios = persistencia.usuarios;
            }

            if (usuarios.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No se han ingresados usuarios");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (var usuario in usuarios)
                Console.WriteLine(usuario.NombreUsuario) ;   
        }

        private bool NoEsUsuarioExistente(Usuario unUsuario)
        {
            List<Usuario> misUsuarios = persistencia.usuarios;
            foreach (var usuario in misUsuarios)
                if (unUsuario.NombreUsuario == usuario.NombreUsuario)
                    return false;

            return true;
        }

        private Usuario DevolverUsuarioExistente(Usuario unUsuario)
        {
            List<Usuario> misUsuarios = persistencia.usuarios;
            foreach (var usuario in misUsuarios)
                if (unUsuario.NombreUsuario == usuario.NombreUsuario)
                    return usuario;

            return null;
        }
    }
}
