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
            List<Juego> juegos = persistencia.juegos;
            foreach (var juego in juegos)
                if (unJuego.Titulo == juego.Titulo)
                    return true;

            return false;
        }

        internal List<Juego> ObtenerJuegos()
        {
            return persistencia.juegos;
        }

        public bool AgregarJuego(Juego juego)
        {
            bool esJuegoExistente = EsJuegoExistente(juego);

            if (esJuegoExistente)
                return false;
            else
                persistencia.juegos.Add(juego);
            return true;
        }

        public void AgregarCalificacion(Calificacion calificacion)
        {
            Juego juego = BuscarJuegoPortTitulo(calificacion.TituloJuego);
            juego.calificaciones.Add(calificacion);

            juego.Ranking = Math.Abs((juego.Ranking + calificacion.Nota) / juego.calificaciones.Count);
        }

        public void VerCatalogoJuegos()
        {
            List<Juego> juegos = persistencia.juegos;
            foreach (var juego in juegos)
                    Console.WriteLine(juego.ToString());
        }

        public Juego BuscarJuegoPortTitulo(string unTitulo)
        {
            List<Juego> juegos = persistencia.juegos;
            foreach (var juego in juegos)
                if (unTitulo == juego.Titulo)
                    return juego;

            return null;
        }

        public List<Juego> BuscarJuegoPorGenero(string unGenero)
        {
            List<Juego> juegos = persistencia.juegos;
            List<Juego> juegosPorgenero = new List<Juego>();

            foreach (var juego in juegos)
                if (unGenero == juego.Genero)
                    juegosPorgenero.Add(juego);

            return juegosPorgenero;
        }

        internal Juego ObtenerJuegoPorTitulo(string tituloJuego)
        {
            List<Juego> juegos = persistencia.juegos;
            foreach (Juego juego in juegos)
                if (juego.Titulo == tituloJuego)
                    return juego;

            return null;
        }

        public List<Juego> BuscarJuegoPorCalificacion(int ranking)
        {
            List<Juego> juegos = new List<Juego>();

            foreach (Juego juego in persistencia.juegos)
                if (juego.Ranking == ranking)
                    juegos.Add(juego);

            return juegos;
        }

        internal void EliminarJuego(string tituloJuego)
        {
            foreach (Juego juego in persistencia.juegos)
                if(juego.Titulo == tituloJuego)
                {
                    persistencia.juegos.Remove(juego);
                    return;
                }
        }

    }
}
