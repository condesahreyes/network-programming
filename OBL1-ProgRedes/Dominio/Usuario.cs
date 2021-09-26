using System.Collections.Generic;
using System;

namespace LogicaNegocio
{
    public class Usuario
    {
        List<Juego> juegos;
        public string NombreUsuario { get; set; }

        public Usuario(string nombreUsuario)
        {
            this.juegos = new List<Juego>();
            this.NombreUsuario = nombreUsuario;
        }

        public static Usuario CrearUsuario()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Ingrese su nombre de usuario:");
            Console.ForegroundColor = ConsoleColor.White;
            string nombreUsuario = Console.ReadLine();

            return new Usuario(nombreUsuario);
        }
    }
}
