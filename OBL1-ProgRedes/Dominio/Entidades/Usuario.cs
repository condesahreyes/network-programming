using System;
using System.Collections.Generic;

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
            Console.WriteLine("Ingrese su nombre de usuario");
            string nombreUsuario = Console.ReadLine();

            return new Usuario(nombreUsuario);
        }
    }
}
