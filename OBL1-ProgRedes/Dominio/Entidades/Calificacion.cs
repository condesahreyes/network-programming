using System;
using System.Collections.Generic;
using System.Text;

namespace LogicaNegocio
{
    public class Calificacion
    {
        public int Nota { get; set; }
        public string Comentario { get; set; }
        public string Usuario { get; }
        public string TituloJuego { get; set; }

        public Calificacion(string tituloJuego, int nota, string comentario, string usuario)
        {
            this.TituloJuego = tituloJuego;
            this.Comentario = comentario;
            this.Nota = nota;
            this.Usuario = usuario;
        }

        public static Calificacion CrearCalificacion(string usuario, string tituloJuego)
        {
            Console.WriteLine("Ingrese cantidad de estrellas");
            int nota = Convert.ToInt32(Console.ReadLine());  

            Console.WriteLine("Ingrese un comentario");
            string comentario = Console.ReadLine();


            return new Calificacion(tituloJuego, nota, comentario, usuario);
        }

        public override string ToString()
        {
            return "Comentario: " + Comentario + " - Nota: " + Nota + " - Usuario: " + Usuario;
        }
    }
}
