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

        public List<CalificacionEntradaSalida> calificaciones { get; set; }

        public List<UsuarioEntradaSalida> usuarios { get; set; }

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
        }

        public static List<JuegoSalida> JuegosAModelo(List<Juego> juegos)
        {
            List<JuegoSalida> juegosSalida = new List<JuegoSalida>();
            juegos.ForEach(j => juegosSalida.Add(new JuegoSalida(j)));

            return juegosSalida;
        }
    }
}
