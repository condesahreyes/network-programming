using System.Collections.Generic;
using System;

namespace LogicaNegocio
{
    public class Usuario
    {
        public string NombreUsuario { get; set; }
        public bool UsuarioActivo{ get; set; }

        public Usuario(string nombreUsuario)
        {
            this.NombreUsuario = nombreUsuario;
            this.UsuarioActivo = false;
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
