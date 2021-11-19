using Grpc.Core;
using LogicaNegocio;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServidorAdministrativo.Services
{
    public class UsuarioProtoService : ServicioUsuario.ServicioUsuarioBase
    {
        Persistencia persistencia;
        public UsuarioProtoService()
        {
            this.persistencia = Persistencia.ObtenerPersistencia();
        }

        public override async Task<RespuestaProto> AltaUsuario(UsuarioProto request, ServerCallContext context)
        {
            Usuario usuario = new Usuario(request.Nombre);
            this.persistencia.usuarios.Add(usuario);

            return await Task.FromResult(new RespuestaProto
            {
                Estado = "Ok"
            });
        }

        public override async Task<UsuariosProto> ObtenerUsuarios(MensajeVacio request, ServerCallContext context)
        {
            List<UsuarioProto> usuariosProto = new List<UsuarioProto>();
            List<Usuario> usuariosDominio = this.persistencia.usuarios;

            UsuariosProto usuarios = new UsuariosProto();

            usuariosDominio.ForEach(x => usuariosProto.Add(new UsuarioProto { Nombre = x.NombreUsuario }));

            usuarios.Usuario.AddRange(usuariosProto);

            return await Task.FromResult(usuarios);
        }
    }
}
