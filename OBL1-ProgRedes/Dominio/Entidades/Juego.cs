using System;
using System.Collections.Generic;

namespace LogicaNegocio
{
    public  class Juego
    {
        public List<Calificacion> calificaciones;

        public string Titulo { get; set; }
        public string Genero { get; set; }
        public string Sinopsis { get; set; }

        public Byte[] Caratula { get; set; }

        public Juego(string titulo, string genero, string sinopsis, byte[] caratula)
        {
            this.Titulo = titulo;
            this.Genero = genero;
            this.calificaciones = new List<Calificacion>();
            this.Sinopsis = sinopsis;
            this.Caratula = caratula;
        }

        public Juego()
        {
            this.calificaciones = new List<Calificacion>();
        }

        public static Juego CrearJuego()
        {
            Console.WriteLine("Ingrese titulo del juego \n");
            string titulo = Console.ReadLine();

            Console.WriteLine("Ingrese género");
            string genero = Console.ReadLine();

            Console.WriteLine("Ingrese sinopsis");
            string sinopsis = Console.ReadLine();

            Console.WriteLine("Ingrese caratula");
            byte[] caratula; //Esperar respuesta de la profe

            return new Juego(titulo, genero, sinopsis, null);
        }

        public override string ToString()
        {
            return "Titulo: " + Titulo + " - Genero: " + Genero + " - Sinopsis: " + Sinopsis;
        }
    }
}
