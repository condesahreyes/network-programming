using System.Collections.Generic;
using LogicaNegocio;

namespace ServidorAdministrativo
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
            if(_persistencia == null)
            {
                _persistencia = new Persistencia();
            }

            return _persistencia;
        }
    }
}
