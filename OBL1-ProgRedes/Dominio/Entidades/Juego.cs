using System;
using System.Collections.Generic;

namespace LogicaNegocio
{
    public class Juego
    {
        List<Calificacion> calificaciones;

        string Titulo { get; set; }
        string Genero { get; set; }
        string Sinopsis { get; set; }

        Byte[] Caratula { get; set; }

        public Juego(string titulo, string genero, string sinopsis, byte[] caratula)
        {
            this.Titulo = titulo;
            this.Genero = genero;
            this.calificaciones = new List<Calificacion>();
            this.Sinopsis = sinopsis;
            this.Caratula = caratula;
        }

    }
}
