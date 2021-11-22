﻿using System.Collections.Generic;
using System.Threading.Tasks;
using LogicaNegocio;

namespace IServices
{
    public interface IJuegoService
    {
        Task<bool> EsJuegoExistente(Juego unJuego);

        Task<List<Juego>> ObtenerJuegos();

        Task<bool> AgregarJuego(Juego juego);

        Task<bool> AgregarCalificacion(Calificacion calificacion);

        Task<Juego> AdquirirJuegoPorUsuario(string juego, Usuario usuario);

        Task<List<Juego>> JuegoUsuarios(Usuario usuario);

        Task<Juego> BuscarJuegoPortTitulo(string unTitulo);

        Task<List<Juego>> BuscarJuegoPorGenero(string unGenero);

        Task<Juego> ObtenerJuegoPorTitulo(string tituloJuego);

        Task<List<Juego>> BuscarJuegoPorCalificacion(int ranking);

        Task<bool> EliminarJuego(string tituloJuego);

        Task<bool> DesasociarJuegoAUsuario(string juego, Usuario usuario);

        Task<Juego> ModificarJuego(string tiuuloJuego, Juego juegoModificado);
    }
}
