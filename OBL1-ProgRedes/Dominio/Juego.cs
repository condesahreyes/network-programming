using System.Collections.Generic;
using System.IO;
using System;

namespace LogicaNegocio
{
    public  class Juego
    {
        public List<Calificacion> calificaciones;

        public List<Usuario> usuarios;
        public string Sinopsis { get; set; }
        public string Caratula { get; set; }
        public string Titulo { get; set; }
        public string Genero { get; set; }

        public int Ranking { get; set; }
        public int Notas { get; set; }

        public Juego(string titulo, string genero, string sinopsis, string caratula)
        {
            this.calificaciones = new List<Calificacion>();
            this.usuarios = new List<Usuario>();
            this.Titulo = titulo;
            this.Genero = genero;
            this.Sinopsis = sinopsis;
            this.Caratula = caratula;
            this.Ranking = 0;
            this.Notas = 0;
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

            return CreacionDeJuego();
        }

        public static Juego ModificarJuego()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("********************* Modificar juego **********************");

            return CreacionDeJuego();
        }

        private static Juego CreacionDeJuego()
        {
            MostrarMensaje("Ingrese titulo del juego:");
            Console.ForegroundColor = ConsoleColor.White;
            string titulo = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            MostrarMensaje("Ingrese género:");

            string genero = Console.ReadLine();

            MostrarMensaje("Ingrese sinopsis:");
            string sinopsis = Console.ReadLine();

            MostrarMensaje("Ingrese caratula:");

            string caratula = ObtenerCaratula();

            return new Juego(titulo, genero, sinopsis, caratula);
        }

        private static string ObtenerCaratula()
        {
            while (true)
            {
                string caratula = Console.ReadLine();
                if (File.Exists(caratula))
                    return caratula;
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nNo existe ningun archivo en dicha ruta");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\nIngrese caratula: ");
                    Console.ForegroundColor = ConsoleColor.White;
                }

            }
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
