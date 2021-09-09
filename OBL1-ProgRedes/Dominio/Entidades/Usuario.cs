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


    }
}
