using LogicaNegocio;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IServices
{
    public interface IJuegoService
    {
            bool EsJuegoExistente(Juego unJuego);

            Task<List<Juego>> ObtenerJuegos();

            Task<bool> AgregarJuego(Juego juego);

            bool AgregarCalificacion(Calificacion calificacion);

            Juego AdquirirJuegoPorUsuario(string juego, Usuario usuario);

            List<Juego> JuegoUsuarios(Usuario usuario);

            Juego BuscarJuegoPortTitulo(string unTitulo);

            List<Juego> BuscarJuegoPorGenero(string unGenero);

            Juego ObtenerJuegoPorTitulo(string tituloJuego);

            List<Juego> BuscarJuegoPorCalificacion(int ranking);

            bool EliminarJuego(string tituloJuego);
}
}
