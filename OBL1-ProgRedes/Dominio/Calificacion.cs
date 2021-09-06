using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio
{
    public class Calificacion
    {
        int CantidadEstrellas { get; set; }
        string Comentario { get; set; }
        string Nota { get; set; }
        Usuario Usuario { get; }

        public Calificacion(int estrellas, string comentario, string nota, Usuario usuario)
        {
            this.CantidadEstrellas = estrellas;
            this.Comentario = comentario;
            this.Nota = nota;
            this.Usuario = usuario;
        }
    }
}
