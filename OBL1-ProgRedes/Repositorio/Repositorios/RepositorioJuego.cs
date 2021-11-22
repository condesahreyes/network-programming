using System.Collections.Generic;
using LogicaNegocio;
using IRepositorio;

namespace Repositorio.Repositorios
{
    public class RepositorioJuego : IRepositorioJuego
    {
        Persistencia persistencia;

        public RepositorioJuego()
        {
            this.persistencia = Persistencia.ObtenerPersistencia();
        }

        public bool AgregarCalificacion(Calificacion calificacion)
        {
                Juego juego = JuegoPorTitulo(calificacion.TituloJuego);

                if (juego == null)
                    return false;

            lock (persistencia.juegos)
            {
                juego.calificaciones.Add(calificacion);
            }
            return true;
        }

        public bool EliminarJuego(string tituloJuego)
        {
            Juego juego = JuegoPorTitulo(tituloJuego);
            
            if (juego == null)
                return false;

            lock (persistencia.juegos)
            {
                this.persistencia.juegos.Remove(juego);
            }
            return true;
        }

        public bool AgregarJuego(Juego juego)
        {
            Juego juegoExistente = JuegoPorTitulo(juego.Titulo);
            if (juego == null)
                return false;

            lock (persistencia.juegos)
            {
                this.persistencia.juegos.Add(juego);
            }
            return true;
        }

        public List<Juego> ObtenerJuegos()
        {
            lock (persistencia.juegos)
            {
                return this.persistencia.juegos;
            }
        }

        public bool DesasociarJuegoUsuario(string tituloJuego, string nombreUsuario)
        {
            lock (persistencia.juegos)
            {
                Juego juego = JuegoPorTitulo(tituloJuego);

                if (juego == null)
                    return false;

                foreach (Usuario usuario in juego.usuarios)
                    if (usuario.NombreUsuario == nombreUsuario)
                    {
                        juego.usuarios.Remove(usuario);
                        return true;
                    }
            }

            return false;
        }

        public Juego AsociarJuegoUsuario(string tituloJuego, string nombreUsuario)
        {
            Juego juego = JuegoPorTitulo(tituloJuego);
            Usuario usuario = ObtenerUsuario(nombreUsuario);

            if (juego == null || usuario == null)
                return null;

            lock (persistencia.juegos)
            {
                juego.usuarios.Add(usuario);
            }
            return juego;
        }

        public Juego ModificarJuego(string tituloJuego, string caratula, string sinopsis, string genero)
        {
            Juego juego = JuegoPorTitulo(tituloJuego);

            if (juego == null)
                return null;

            juego.Caratula = caratula;
            juego.Sinopsis = sinopsis;
            juego.Genero = genero;

            return juego;
        }

        private Usuario ObtenerUsuario(string nombreUsuario)
        {
            lock (persistencia.juegos)
            {
                return this.persistencia.usuarios.Find(x => x.NombreUsuario == nombreUsuario);
            }
        }

        private Juego JuegoPorTitulo(string tituloJuego)
        {
            lock (persistencia.juegos)
            {
                return this.persistencia.juegos.Find(x => x.Titulo.Equals(tituloJuego));
            }
        }

    }
}
