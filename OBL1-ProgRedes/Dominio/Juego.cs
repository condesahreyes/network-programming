using System;


namespace Dominio
{
    public class Juego
    {
        string Titulo { get; set; }
        string Genero { get; set; }
        string Sinopsis { get; set; }
        Calificacion Calificacion { get; set; }
        Byte[] Caratula { get; set; }

        public Juego(string titulo, string genero, Calificacion calificacion, string sinopsis, 
            byte[] caratula)
        {
            this.Titulo = titulo;
            this.Genero = genero;
            this.Calificacion = calificacion;
            this.Sinopsis = sinopsis;
            this.Caratula = caratula;
        }

    }
}
