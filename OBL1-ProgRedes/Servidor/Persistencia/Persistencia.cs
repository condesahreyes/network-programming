using System.Collections.Generic;
using LogicaNegocio;

namespace Servidor
{
    public class Persistencia
    {
        private static Persistencia _persistencia;

        public List<Usuario> usuarios;
        public List<Juego> juegos;

        public Persistencia()
        {
            this.usuarios = new List<Usuario>();
            this.juegos = new List<Juego>();
        }

        public static Persistencia ObtenerPersistencia()
        {
            return _persistencia == null ? new Persistencia() : _persistencia;
        }
    }
}
