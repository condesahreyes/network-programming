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
            lock (persistencia.juegos)
            {
                List<Juego> juegos = persistencia.juegos;
                foreach (var juego in juegos)
                    if (unJuego.Titulo == juego.Titulo)
                        return true;

                return false;
            }
        }

        public List<Juego> ObtenerJuegos()
        {
            lock (persistencia.juegos)
            {
                return persistencia.juegos;
            }
        }

        public bool AgregarJuego(Juego juego)
        {
            lock (persistencia.juegos) {
                bool esJuegoExistente = EsJuegoExistente(juego);

                if (esJuegoExistente)
                    return false;
                else
                    persistencia.juegos.Add(juego);
                return true;
            }
        }

        public bool AgregarCalificacion(Calificacion calificacion)
        {
            Juego juego = BuscarJuegoPortTitulo(calificacion.TituloJuego);

            if (juego == null)
                return false;

            lock (persistencia.juegos)
            {
                juego.calificaciones.Add(calificacion);

                juego.Notas += calificacion.Nota;
                juego.Ranking = Math.Abs((juego.Notas) / juego.calificaciones.Count);
            }

            return true;
        }

        public void VerCatalogoJuegos()
        {
            List<Juego> juegos = persistencia.juegos;

            if (juegos.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No se han ingresados juegos");
                return;
            }

            lock (persistencia.juegos)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                foreach (var juego in juegos)
                    Console.WriteLine(juego.ToString());
            }
        }

        public Juego AdquirirJuegoPorUsuario(string juego, Usuario usuario)
        {
            lock (persistencia)
            {
                Juego unJuego = ObtenerJuegoPorTitulo(juego);
                if (unJuego == null)
                    return unJuego;
                foreach (Usuario unUsuario in persistencia.usuarios)
                {
                    if (unUsuario.NombreUsuario == usuario.NombreUsuario)
                    {
                        unJuego.usuarios.Add(unUsuario);
                        return unJuego;
                    }
                }
                return unJuego;
            }
        }

        public List<Juego> JuegoUsuarios(Usuario usuario)
        {
            lock (persistencia)
            {
                List<Juego> juegosUsuario = new List<Juego>();
                foreach (Juego juego in persistencia.juegos)
                {
                    foreach (Usuario unUsuario in juego.usuarios)
                    {
                        if (unUsuario.NombreUsuario == usuario.NombreUsuario)
                        {
                            juegosUsuario.Add(juego);
                        }
                    }
                }
                return juegosUsuario;
            }
        }

        public Juego BuscarJuegoPortTitulo(string unTitulo)
        {
            lock (persistencia.juegos)
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
            lock (persistencia.juegos)
            {
                List<Juego> juegos = persistencia.juegos;
                List<Juego> juegosPorgenero = new List<Juego>();

                foreach (var juego in juegos)
                    if (unGenero == juego.Genero)
                        juegosPorgenero.Add(juego);

                return juegosPorgenero;
            }

        }

        public Juego ObtenerJuegoPorTitulo(string tituloJuego)
        {
            lock (persistencia.juegos)
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
            lock (persistencia.juegos)
            {
                List<Juego> juegos = new List<Juego>();

                foreach (Juego juego in persistencia.juegos)
                    if (juego.Ranking == ranking)
                        juegos.Add(juego);

                return juegos;
            }
        }

        public bool EliminarJuego(string tituloJuego)
        {
            lock (persistencia.juegos)
            {
                foreach (Juego juego in persistencia.juegos)
                    if (juego.Titulo == tituloJuego)
                    {
                        persistencia.juegos.Remove(juego);
                        return true;
                    }
            }
            return false;
        }
    }
}
