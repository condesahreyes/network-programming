using LogicaNegocio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Servidor.FuncionalidadesEntidades
{

    public class FuncionalidadesUsuario
    {
        private Persistencia persistencia;

        public FuncionalidadesUsuario()
        {
            this.persistencia = Persistencia.ObtenerPersistencia();
        }

        private bool NoEsUsuarioExistente(Usuario unUsuario)
        {
            List<Usuario> misUsuarios = persistencia.usuarios;
            foreach (var usuario in misUsuarios)
                if (unUsuario.NombreUsuario == usuario.NombreUsuario)
                    return false;
            return true;
        }

        public Usuario ObtenerUsuario(string nombreUsuario)
        {
            Usuario unUsuario = new Usuario(nombreUsuario);

            bool noExisteUsuario = NoEsUsuarioExistente(unUsuario);

            if(noExisteUsuario)
                persistencia.usuarios.Add(unUsuario);

            return DevolverUsuarioExistente(unUsuario);
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
