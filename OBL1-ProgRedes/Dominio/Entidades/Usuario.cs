using System;
using System.Collections.Generic;

namespace LogicaNegocio
{
    public class Usuario
    {
        List<Juego> juegos;
        string NombreUsuario { get; set; }
        string Contraseña { get; set; } //Preguntarle a juli si le parece que aplica o lo sacamos

        public Usuario(string nombreUsuario, string contraseña)
        {
            this.juegos = new List<Juego>();
            this.NombreUsuario = nombreUsuario;
            this.Contraseña = contraseña;
        }


    }
}
