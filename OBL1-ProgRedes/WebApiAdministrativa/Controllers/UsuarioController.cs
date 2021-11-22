using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net;
using WebApiAdministrativa.Modelos.UsuarioModelos;
using IServices;
using LogicaNegocio;
using System.Collections.Generic;

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
        public async Task<ActionResult> Post([FromBody] UsuarioEntradaSalida unUsuario)
        {
            Usuario usuario = await servicioUsuario.ObtenerUsuario(UsuarioEntradaSalida.ModeloADominio(unUsuario));

            return (StatusCode((int)HttpStatusCode.OK, UsuarioEntradaSalida.DominioAModelo(usuario)));
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            List<Usuario> usuarios = await servicioUsuario.ObtenerUsuarios();
            return (StatusCode((int)HttpStatusCode.OK, UsuarioEntradaSalida.ListarUsuarioModelo(usuarios)));
        }

        [HttpPut("{nombreUsuario}")]
        public async Task<ActionResult> Put([FromRoute] string nombreUsuario, [FromBody] string nuevoNombre)
        {
            bool modifico = await servicioUsuario.ModificarUsuario(nombreUsuario, nuevoNombre);
            return (StatusCode((int)HttpStatusCode.OK, modifico));
        }

        [HttpDelete("{nombreUsuario}")]
        public async Task<ActionResult> Delete([FromRoute] string nombreUsuario, [FromBody] string nuevoNombre)
        {
            bool modifico = await servicioUsuario.ModificarUsuario(nombreUsuario, nuevoNombre);
            return (StatusCode((int)HttpStatusCode.OK, modifico));
        }
    }
}