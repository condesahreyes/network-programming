using LogicaNegocio;
using Servidor.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace Servidor.FuncionalidadesPorEntidad
{
    public class FuncionalidadesJuego
    {
        private Persistencia persistencia;

        public FuncionalidadesJuego()
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

        public void PublicarJuego()
        {
            bool esJuegoExistente = true;

            Juego juego = null;

            while (esJuegoExistente)
            {
                juego = Juego.CrearJuego();
                esJuegoExistente = EsJuegoExistente(juego);

                if (esJuegoExistente)
                    Console.WriteLine("El titulo " + juego.Titulo + 
                        " ya esta registrado en el sistema.");
            }

            persistencia.juegos.Add(juego);
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

        public void BuscarJuegoPorCalificacion()
        {
            //Que carajos hacemos aca
            throw new NotImplementedException();
        }
    }
}
