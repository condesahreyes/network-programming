using System.Text.RegularExpressions;
using System;

namespace LogicaNegocio
{
    public class Calificacion
    {
        public int Nota { get; set; }
        public string Comentario { get; set; }
        public string Usuario { get; set; }
        public string TituloJuego { get; set; }

        public Calificacion(string tituloJuego, int nota, string comentario, string usuario)
        {
            this.TituloJuego = tituloJuego;
            this.Comentario = comentario;
            this.Nota = nota;
            this.Usuario = usuario;
        }
        public Calificacion() { }

        public static Calificacion CrearCalificacion(string usuario, string tituloJuego)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("***************** Calificación: " + tituloJuego +" ******************");

            int nota = ObtenerOpcion("Ingrese cantidad de estrellas:");

            MostrarMensaje("Ingrese un comentario:");
            string comentario = Console.ReadLine();

            return new Calificacion(tituloJuego, nota, comentario, usuario);
        }

        private static int ObtenerOpcion(string mensaje)
        {
            int opcionInt = 0;
            var opcion = "";
            var opcionObtenida = false;
            while (!opcionObtenida)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(mensaje);
                Console.ForegroundColor = ConsoleColor.White;

                opcion = Console.ReadLine();
                if (!Regex.IsMatch(opcion, "^[" + 1 + "-" + 5 + "]$"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" \nIngrese una opción valida. Entre " + 1 + " y "
                        + 5 +"\n");
                }
                else
                {
                    opcionInt = Convert.ToInt32(opcion);
                    opcionObtenida = true;

                }
            }
            return opcionInt;
        }

        private static void MostrarMensaje(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n" + mensaje);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public override string ToString()
        {
            return "Comentario: " + Comentario + " - Nota: " + Nota + " - Usuario: " + Usuario;
        }
    }
}
