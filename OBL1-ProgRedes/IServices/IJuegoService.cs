using System.Collections.Generic;
using System.Threading.Tasks;
using LogicaNegocio;

namespace IServices
{
    public interface IJuegoService
    {
        Task<bool> EsJuegoExistenteAsync(Juego unJuego);

        Task<List<Juego>> ObtenerJuegosAsync();

        Task<bool> AgregarJuegoAsync(Juego juego);

        Task<bool> AgregarCalificacionAsync(Calificacion calificacion);

        Task<Juego> AdquirirJuegoPorUsuarioAsync(string juego, Usuario usuario);

        Task<List<Juego>> JuegoUsuariosAsync(Usuario usuario);

        Task<Juego> BuscarJuegoPortTituloAsync(string unTitulo);

        Task<List<Juego>> BuscarJuegoPorGeneroAsync(string unGenero);

        Task<Juego> ObtenerJuegoPorTituloAsync(string tituloJuego);

        Task<List<Juego>> BuscarJuegoPorCalificacionAsync(int ranking);

        Task<bool> EliminarJuegoAsync(string tituloJuego);

        Task<bool> DesasociarJuegoAUsuarioAsync(string juego, Usuario usuario);

        Task<Juego> ModificarJuegoAsync(string tiuuloJuego, Juego juegoModificado);
    }
}
