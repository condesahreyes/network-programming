using WebApiAdministrativa.Modelos.CalificacionModelos;
using WebApiAdministrativa.Modelos.UsuarioModelos;
using System.Collections.Generic;
using LogicaNegocio;

namespace WebApiAdministrativa.Modelos.JuegoModelos
{
    public class JuegoSalida
    {
        public string Sinopsis { get; set; }
        public string Caratula { get; set; }
        public string Titulo { get; set; }
        public string Genero { get; set; }

        public int Ranking { get; set; }
        public int Notas { get; set; }

        public List<CalificacionEntradaSalida> calificaciones;

        public List<UsuarioEntradaSalida> usuarios;

        public JuegoSalida() { }

        public JuegoSalida(Juego unJuego)
        {
            this.calificaciones = CalificacionEntradaSalida.
                ListarCalificacionModelo(unJuego.calificaciones);

            this.usuarios = UsuarioEntradaSalida.
                ListarUsuarioModelo(unJuego.usuarios);

            this.Titulo = unJuego.Titulo;
            this.Genero = unJuego.Genero;
            this.Sinopsis = unJuego.Sinopsis;
            this.Caratula = unJuego.Caratula;
            this.Ranking = unJuego.Ranking;
            this.Notas = unJuego.Notas;
        }
    }
}
