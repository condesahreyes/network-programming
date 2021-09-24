using System.Collections.Generic;
using System;

namespace LogicaNegocio
{
    public  class Juego
    {
        public List<Calificacion> calificaciones;

        public string Titulo { get; set; }
        public string Genero { get; set; }
        public string Sinopsis { get; set; }
        public int Ranking { get; set; }
        public Byte[] Caratula { get; set; }

        public Juego(string titulo, string genero, string sinopsis, byte[] caratula)
        {
            this.Titulo = titulo;
            this.Genero = genero;
            this.calificaciones = new List<Calificacion>();
            this.Sinopsis = sinopsis;
            this.Caratula = caratula;
            this.Ranking = 0;
        }

        public Juego()
        {
            this.calificaciones = new List<Calificacion>();
        }

        public static Juego CrearJuego()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("********************* Alta de juego **********************");
                               
            MostrarMensaje("Ingrese titulo del juego:");
            Console.ForegroundColor = ConsoleColor.White;
            string titulo = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            MostrarMensaje("Ingrese género:");

            string genero = Console.ReadLine();

            MostrarMensaje("Ingrese sinopsis:");
            string sinopsis = Console.ReadLine();

            MostrarMensaje("Ingrese caratula:");

            byte[] caratula;

            return new Juego(titulo, genero, sinopsis, null);
        }

        public static Juego ModificarJuego()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("********************* Modificar juego **********************");

            MostrarMensaje("Ingrese titulo del juego:");
            Console.ForegroundColor = ConsoleColor.White;
            string titulo = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            MostrarMensaje("Ingrese género:");

            string genero = Console.ReadLine();

            MostrarMensaje("Ingrese sinopsis:");
            string sinopsis = Console.ReadLine();

            MostrarMensaje("Ingrese caratula:");

            byte[] caratula; //Esperar respuesta de la profe

            return new Juego(titulo, genero, sinopsis, null);
        }

        public static void MostrarMensaje(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n" + mensaje);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public override string ToString()
        {
            string calificaciones = "";
            int enumerado = 0;

            foreach (Calificacion unaCalificacion in this.calificaciones)
            {
                enumerado++;
                calificaciones += enumerado + ". " + unaCalificacion.ToString() + "\n";
            }

            if(calificaciones != "")
                return "Titulo: " + Titulo + " - Genero: " + Genero + " - Sinopsis: " + 
                    Sinopsis + "\n" + "Calificaciones: " + "\n" + calificaciones;
            else
                return "Titulo: " + Titulo + " - Genero: " + Genero + " - Sinopsis: " +
                    Sinopsis + "\n" + "Calificaciones: Aun no ha sido calificado";
        }
    }
}
