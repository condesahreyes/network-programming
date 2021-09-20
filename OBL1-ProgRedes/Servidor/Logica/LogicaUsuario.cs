using System.Collections.Generic;
using LogicaNegocio;
using System;

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
                persistencia.usuarios.Add(usuario);
                Console.WriteLine("El usuario " + usuario.NombreUsuario + " no existia en el sistema. \n" +
                    "Lo hemos dado de alta.");
            }
            else
                Console.WriteLine("Se ha iniciado sesión el usuario " + usuario.NombreUsuario);

            return DevolverUsuarioExistente(usuario);
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
