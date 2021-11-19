using LogicaNegocio;
using System;
using System.Collections.Generic;
using System.Text;

namespace IServices
{
    public interface IJuegoService
    {
            bool EsJuegoExistente(Juego unJuego);

            List<Juego> ObtenerJuegos();

            bool AgregarJuego(Juego juego);

            bool AgregarCalificacion(Calificacion calificacion);

            void VerCatalogoJuegos();

            Juego AdquirirJuegoPorUsuario(string juego, Usuario usuario);

            List<Juego> JuegoUsuarios(Usuario usuario);

            Juego BuscarJuegoPortTitulo(string unTitulo);

            List<Juego> BuscarJuegoPorGenero(string unGenero);

            Juego ObtenerJuegoPorTitulo(string tituloJuego);

            List<Juego> BuscarJuegoPorCalificacion(int ranking);

            bool EliminarJuego(string tituloJuego);
}
}
