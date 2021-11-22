using WebApiAdministrativa.Modelos.JuegoModelos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LogicaNegocio;
using System.Net;
using IServices;
using System.Collections.Generic;

namespace WebApiAdministrativa.Controllers
{
    [Route("administrativo/juegos")]
    [ApiController]
    public class JuegoController : ControllerBase
    {
        private const string noExisteJuegoUsuario = "No existe juego y/o usuario";
        private const string juegoInexistente = "No existe juego";
        private const string juegoExistente = "Ya existe juego";

        private IJuegoService servicioJuego;

        public JuegoController(IJuegoService servicioJuego)
        {
            this.servicioJuego = servicioJuego;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            List<Juego> juegos = await servicioJuego.ObtenerJuegos();
            return (StatusCode((int)HttpStatusCode.OK, JuegoSalida.JuegosAModelo(juegos)));
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] JuegoEntrada unJuego)
        {
            bool creado = await servicioJuego.AgregarJuego(JuegoEntrada.ModeloADominio(unJuego));

            return (creado == true) ? (StatusCode((int)HttpStatusCode.Created, unJuego)) :
                (StatusCode((int)HttpStatusCode.BadRequest, juegoExistente));
        }
        
        [HttpPut("{tituloJuego}")]
        public async Task<ActionResult> Put([FromRoute] string tituloJuego, [FromBody] JuegoModificar juegoNuevo)
        {
            Juego juegoModificado = await servicioJuego.ModificarJuego(tituloJuego, JuegoModificar.ModeloADominio(juegoNuevo, tituloJuego));

            return (juegoModificado != null) ? (StatusCode((int)HttpStatusCode.OK, juegoModificado)) :
                (StatusCode((int)HttpStatusCode.BadRequest, juegoInexistente));
        }

        [HttpDelete("{tituloJuego}")]
        public async Task<ActionResult> Delete([FromRoute] string tituloJuego)
        {
            bool eliminado = await servicioJuego.EliminarJuego(tituloJuego);
            
            return (eliminado == true) ? (StatusCode((int)HttpStatusCode.NoContent, "")) : 
                (StatusCode((int)HttpStatusCode.BadRequest, juegoInexistente));
        }

        [HttpPost("{tituloJuego}/usuarios/{nombreUsuario}")]
        public async Task<ActionResult> PostJuegoUsuario([FromRoute] string tituloJuego, [FromRoute] string nombreUsuario)
        {
            Juego juego = await servicioJuego.AdquirirJuegoPorUsuario(tituloJuego, new Usuario(nombreUsuario));

            return (juego != null) ? (StatusCode((int)HttpStatusCode.Created, juego)) :
                (StatusCode((int)HttpStatusCode.BadRequest, noExisteJuegoUsuario));
        }

        [HttpDelete("{tituloJuego}/usuarios/{usuarioName}")]
        public async Task<ActionResult> DeleteJuegoUsuario([FromRoute] string tituloJuego, [FromRoute] string usuarioName)
        {
            bool desasocia = await servicioJuego.DesasociarJuegoAUsuario(tituloJuego, new Usuario(usuarioName));

            return (desasocia == true) ? (StatusCode((int)HttpStatusCode.NoContent, "")) :
                (StatusCode((int)HttpStatusCode.BadRequest, noExisteJuegoUsuario));
        }

    }
}
