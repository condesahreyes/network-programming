using IServices;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using WebApiAdministrativa.Modelos.JuegoModelos;

namespace WebApiAdministrativa.Controllers
{
    [Route("administrativo/juegos")]
    public class JuegoController : ControllerBase
    {
        private IJuegoService servicioJuego;
        private const string juegoExistente = "Ya existe juego";
        private const string juegoInexistente = "No existe juego";
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
        [HttpDelete("{tituloJuego}")]
        public async Task<ActionResult> Update([FromRoute] string tituloJuego, [FromBody] )
        {
            bool eliminado = await servicioJuego.EliminarJuego(tituloJuego);

            return (eliminado == true) ? (StatusCode((int)HttpStatusCode.NoContent, "")) :
                (StatusCode((int)HttpStatusCode.BadRequest, juegoInexistente));
        }*/

    }
   
}
