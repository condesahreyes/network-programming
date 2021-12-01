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
        private const string operacionExitosa = "Operacion exitosa";


        private IUsuarioService servicioUsuario;

        public UsuarioController(IUsuarioService servicioUsuario)
        {
            this.servicioUsuario = servicioUsuario;
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] UsuarioEntradaSalida unUsuario)
        {
            Usuario usuario = await servicioUsuario.ObtenerUsuarioAsync(UsuarioEntradaSalida.ModeloADominio(unUsuario));

            return (StatusCode((int)HttpStatusCode.Created, UsuarioEntradaSalida.DominioAModelo(usuario)));
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            List<Usuario> usuarios = await servicioUsuario.ObtenerUsuariosAsync();
            return (StatusCode((int)HttpStatusCode.OK, UsuarioEntradaSalida.ListarUsuarioModelo(usuarios)));
        }

        [HttpPut("{nombreUsuario}")]
        public async Task<ActionResult> PutAsync([FromRoute] string nombreUsuario, [FromBody] UsuarioEntradaSalida nuevoUsuario)
        {
            bool modifico = await servicioUsuario.ModificarUsuarioAsync(nombreUsuario, nuevoUsuario.NombreUsuario);
            return (modifico==true)?(StatusCode((int)HttpStatusCode.OK, usuarioModificado)) :
                (StatusCode((int)HttpStatusCode.BadRequest, noExisteUsuario));
        }

        [HttpDelete("{nombreUsuario}")]
        public async Task<ActionResult> DeleteAsync([FromRoute] string nombreUsuario)
        {
            bool elimino = await servicioUsuario.EliminarUsuarioAsync(nombreUsuario);
            return (elimino==true)?(StatusCode((int)HttpStatusCode.NoContent, "")):
                (StatusCode((int)HttpStatusCode.BadRequest, noExisteUsuario));
        }
    }
}