using System.Collections.Generic;
using LogicaNegocio;
using System;

namespace Servidor.FuncionalidadesPorEntidad
{
    public class LogicaJuego
    {
        private Persistencia persistencia;

        public LogicaJuego()
        {
            this.persistencia = Persistencia.ObtenerPersistencia();
        }

        public bool EsJuegoExistente(Juego unJuego)
        {
            lock (persistencia)
            {
                List<Juego> juegos = persistencia.juegos;
                foreach (var juego in juegos)
                    if (unJuego.Titulo == juego.Titulo)
                        return true;

                return false;
            }
        }

        internal List<Juego> ObtenerJuegos()
        {
            lock (persistencia)
            {
                return persistencia.juegos;
            }
        }

        public bool AgregarJuego(Juego juego)
        {
            lock (persistencia) {
                bool esJuegoExistente = EsJuegoExistente(juego);

                if (esJuegoExistente)
                    return false;
                else
                    persistencia.juegos.Add(juego);
                return true;
            }
        }

        public void AgregarCalificacion(Calificacion calificacion)
        {
            lock (persistencia)
            {
                Juego juego = BuscarJuegoPortTitulo(calificacion.TituloJuego);
                juego.calificaciones.Add(calificacion);

                juego.Ranking = Math.Abs((juego.Ranking + calificacion.Nota) / juego.calificaciones.Count);
            }
        }

        public void VerCatalogoJuegos()
        {
            List<Juego> juegos = persistencia.juegos;

            if (juegos.Count == 0)
            {
                Console.WriteLine("No se han ingresados juegos");
                return;
            }

            lock (persistencia)
            {
                    foreach (var juego in juegos)
                    Console.WriteLine(juego.ToString());
            }
        }

        public bool AdquirirJuegoPorUsuario(string juego, Usuario usuario)
        {
            Juego unJuego = ObtenerJuegoPorTitulo(juego);
            if (unJuego == null)
                return false;
            foreach (Usuario unUsuario in persistencia.usuarios)
            {
                if(unUsuario.NombreUsuario == usuario.NombreUsuario)
                {
                    unJuego.usuarios.Add(unUsuario);
                    return true;
                }
            }
            return false;
        }

        public List<Juego> JuegoUsuarios(Usuario usuario)
        {
            List<Juego> juegosUsuario = new List<Juego>();
            foreach (Juego juego in persistencia.juegos)
            {
                foreach (Usuario unUsuario in juego.usuarios)
                {
                    if(unUsuario.NombreUsuario == usuario.NombreUsuario)
                    {
                        juegosUsuario.Add(juego);
                    }
                }
            }
            return juegosUsuario;
        }

        public Juego BuscarJuegoPortTitulo(string unTitulo)
        {
            lock (persistencia)
            {
                List<Juego> juegos = persistencia.juegos;
                foreach (var juego in juegos)
                    if (unTitulo == juego.Titulo)
                        return juego;

                return null;
            }
        }

        public List<Juego> BuscarJuegoPorGenero(string unGenero)
        {
            lock (persistencia)
            {
                List<Juego> juegos = persistencia.juegos;
                List<Juego> juegosPorgenero = new List<Juego>();

                foreach (var juego in juegos)
                    if (unGenero == juego.Genero)
                        juegosPorgenero.Add(juego);

                return juegosPorgenero;
            }

        }

        internal Juego ObtenerJuegoPorTitulo(string tituloJuego)
        {
            lock (persistencia)
            {
                List<Juego> juegos = persistencia.juegos;
                foreach (Juego juego in juegos)
                    if (juego.Titulo == tituloJuego)
                        return juego;

                return null;
            }

        }

        public List<Juego> BuscarJuegoPorCalificacion(int ranking)
        {
            lock (persistencia)
            {
                List<Juego> juegos = new List<Juego>();

                foreach (Juego juego in persistencia.juegos)
                    if (juego.Ranking == ranking)
                        juegos.Add(juego);

                return juegos;
            }
        }

        internal void EliminarJuego(string tituloJuego)
        {
            lock (persistencia)
            {
                foreach (Juego juego in persistencia.juegos)
                    if (juego.Titulo == tituloJuego)
                    {
                        persistencia.juegos.Remove(juego);
                        return;
                    }
            }

        }
    }
}
