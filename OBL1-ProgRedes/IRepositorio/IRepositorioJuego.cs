using System.Collections.Generic;
using LogicaNegocio;

namespace IRepositorio
{
    public interface IRepositorioJuego
    {
        bool AgregarCalificacion(Calificacion calificacion);

        bool EliminarJuego(string tituloJuego);

        bool AgregarJuego(Juego juego);

        List<Juego> ObtenerJuegos();

        bool DesasociarJuegoUsuario(string tituloJuego, string nombreUsuario);

        Juego AsociarJuegoUsuario(string tituloJuego, string nombreUsuario);

        Juego ModificarJuego(string tituloJuego, string caratula, string sinopsis, string genero);
    }
}
