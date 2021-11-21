using System.Collections.Generic;
using LogicaNegocio;

namespace WebApiAdministrativa.Modelos.CalificacionModelos
{
    public class CalificacionEntradaSalida
    {
        public int Nota { get; set; }
        public string Comentario { get; set; }
        public string Usuario { get; set; }
        public string TituloJuego { get; set; }

        public CalificacionEntradaSalida() { }

        public CalificacionEntradaSalida(Calificacion calificacion)
        {
            this.TituloJuego = calificacion.TituloJuego;
            this.Comentario = calificacion.Comentario;
            this.Nota = calificacion.Nota;
            this.Usuario = calificacion.Usuario;
        }

        public static List<CalificacionEntradaSalida> ListarCalificacionModelo
            (List<Calificacion> calificaciones)
        {
            List<CalificacionEntradaSalida> calificacionesModelo = 
                new List<CalificacionEntradaSalida>();

            calificaciones.ForEach(c => calificacionesModelo.Add
            (new CalificacionEntradaSalida(c)));

            return calificacionesModelo;
        }
    }
}