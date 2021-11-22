using WebApiAdministrativa.Modelos.UsuarioModelos;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LogicaNegocio;
using System.Net;
using IServices;

namespace WebApiAdministrativa.Controllers
{
    [Route("administrativo/usuarios")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private const string usuarioModificado = "El usuario fue modificado correctamente";
        private const string noExisteUsuario = "El usuario no existe o esta activo";

        private IUsuarioService servicioUsuario;

        public UsuarioController(IUsuarioService servicioUsuario)
        {
            this.servicioUsuario = servicioUsuario;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UsuarioEntradaSalida unUsuario)
        {
            Usuario usuario = await servicioUsuario.ObtenerUsuario(UsuarioEntradaSalida.ModeloADominio(unUsuario));

            return (StatusCode((int)HttpStatusCode.Created, UsuarioEntradaSalida.DominioAModelo(usuario)));
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
            return (modifico==true)?(StatusCode((int)HttpStatusCode.OK, usuarioModificado)) :
                (StatusCode((int)HttpStatusCode.BadRequest, noExisteUsuario));
        }

        [HttpDelete("{nombreUsuario}")]
        public async Task<ActionResult> Delete([FromRoute] string nombreUsuario)
        {
            bool elimino = await servicioUsuario.EliminarUsuario(nombreUsuario);
            return (elimino==true)?(StatusCode((int)HttpStatusCode.OK, elimino)):
                (StatusCode((int)HttpStatusCode.BadRequest, noExisteUsuario));
        }
    }
}