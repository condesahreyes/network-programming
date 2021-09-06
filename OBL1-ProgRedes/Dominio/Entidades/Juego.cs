using System;
using System.Collections.Generic;

namespace LogicaNegocio
{
    public  class Juego
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

            Console.WriteLine("Se ha dado de alta el juego " + titulo + " con éxito");

            return new Juego(titulo, genero, sinopsis, null);

            
        }
    }
}
