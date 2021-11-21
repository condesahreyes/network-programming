using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net;
using WebApiAdministrativa.Modelos.UsuarioModelos;
using IServices;
using LogicaNegocio;

namespace WebApiAdministrativa.Controllers
{
    [Route("administrativo/usuarios")]
    public class UsuarioController : ControllerBase
    {
        private IUsuarioService servicioUsuario;

        public UsuarioController(IUsuarioService servicioUsuario)
        {
            this.servicioUsuario = servicioUsuario;
        }

        [HttpPost]
        public async Task<ActionResult> AltaDeUsuario([FromBody] UsuarioEntradaSalida unUsuario)
        {
            Usuario usuario = await servicioUsuario.ObtenerUsuario(UsuarioEntradaSalida.ModeloADominio(unUsuario));

            return (StatusCode((int)HttpStatusCode.OK, UsuarioEntradaSalida.DominioAModelo(usuario)));
        }
    }
}