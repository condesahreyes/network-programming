using WebApiAdministrativa.Modelos.UsuarioModelos;
using WebApiAdministrativa.Modelos.JuegoModelos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LogicaNegocio;
using System.Net;
using IServices;

namespace WebApiAdministrativa.Controllers
{
    [Route("administrativo/juegos")]
    public class JuegoController : ControllerBase
    {
        private IJuegoService servicioJuego;
        private const string juegoExistente = "Ya existe juego";
        private const string juegoInexistente = "No existe juego";
        private const string noExisteJuegoUsuario = "No existe juego y/o usuario";
        public JuegoController(IJuegoService servicioJuego)
        {
            this.servicioJuego = servicioJuego;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] JuegoEntrada unJuego)
        {
            bool creado = await servicioJuego.AgregarJuego(JuegoEntrada.ModeloADominio(unJuego));

            return (creado == true) ? (StatusCode((int)HttpStatusCode.Created, unJuego)) :
                (StatusCode((int)HttpStatusCode.BadRequest, juegoExistente));
        }

        [HttpDelete("{tituloJuego}")]
        public async Task<ActionResult> Delete([FromRoute] string tituloJuego)
        {
            bool eliminado = await servicioJuego.EliminarJuego(tituloJuego);
            
            return (eliminado == true) ? (StatusCode((int)HttpStatusCode.NoContent, "")) : 
                (StatusCode((int)HttpStatusCode.BadRequest, juegoInexistente));
        }

        /*
        [HttpPut("{tituloJuego}")]
        public async Task<ActionResult> Update([FromRoute] string tituloJuego, [FromBody] JuegoEntrada juegoModificado)
        {
            bool eliminado = await servicioJuego.(tituloJuego);

            return (eliminado == true) ? (StatusCode((int)HttpStatusCode.NoContent, "")) :
                (StatusCode((int)HttpStatusCode.BadRequest, juegoInexistente));
        }*/

        [HttpPost("{tituloJuego}")]
        public async Task<ActionResult> Delete([FromRoute] string tituloJuego, [FromBody] UsuarioEntradaSalida usuario)
        {
            Juego juego = await servicioJuego.AdquirirJuegoPorUsuario(tituloJuego, UsuarioEntradaSalida.ModeloADominio(usuario));
            bool eliminado = await servicioJuego.EliminarJuego(tituloJuego);

            return (juego != null) ? (StatusCode((int)HttpStatusCode.Created, juego)) :
                (StatusCode((int)HttpStatusCode.BadRequest, noExisteJuegoUsuario));
        }

    }
   
}
