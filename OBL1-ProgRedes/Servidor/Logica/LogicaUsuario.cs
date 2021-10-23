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
                    if (unUsuario.NombreUsuario == nombreUsuario)
                    {
                        persistencia.usuarios.Remove(unUsuario);
                        return true;
                    }
            }
            return false;
        }

        public bool ModificarUsuario(string nombreUsuario, string nuevoNombreUsuario)
        {
            lock (persistencia.juegos)
            {
                foreach (Usuario unUsuario in persistencia.usuarios)
                    if (unUsuario.NombreUsuario == nombreUsuario)
                    {
                        unUsuario.NombreUsuario = nuevoNombreUsuario;
                        return true;
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
