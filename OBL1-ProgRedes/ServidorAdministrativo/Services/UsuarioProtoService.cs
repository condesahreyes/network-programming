using System.Collections.Generic;
using System.Threading.Tasks;
using LogicaNegocio;
using Grpc.Core;

namespace ServidorAdministrativo.Services
{
    public class UsuarioProtoService : ServicioUsuario.ServicioUsuarioBase
    {
        LogServices logServices;
        Persistencia persistencia;
        public UsuarioProtoService(LogServices logServices)
        {
            this.logServices = logServices;
            this.persistencia = Persistencia.ObtenerPersistencia();
        }

        public override async Task<RespuestaProto> AltaUsuario(UsuarioProto request, ServerCallContext context)
        {
            this.logServices.SendMessages("usuario " + request.Nombre + "dado de alta");
            Usuario usuario = new Usuario(request.Nombre);
            this.persistencia.usuarios.Add(usuario);

            return await Task.FromResult(new RespuestaProto
            {
                Estado = "Ok"
            });
        }

        public override async Task<UsuariosProto> ObtenerUsuarios(MensajeVacio request, ServerCallContext context)
        {
            List<Usuario> usuariosDominio = this.persistencia.usuarios;

            UsuariosProto usuarios = new UsuariosProto();

            usuariosDominio.ForEach(x => usuarios.Usuario.Add(new UsuarioProto { Nombre = x.NombreUsuario }));

            return await Task.FromResult(usuarios);
        }

        public override async Task<MensajeVacio> ActualizarAUsuarioActivo(UsuarioProto request, ServerCallContext context)
        {
            List<Usuario> misUsuarios = this.persistencia.usuarios;
            foreach (var usuario in misUsuarios)
                if (request.Nombre == usuario.NombreUsuario)
                {
                    usuario.UsuarioActivo = true;
                    return await Task.FromResult(new MensajeVacio() { });
                }

            return await Task.FromResult(new MensajeVacio() { });
        }

        public override async Task<MensajeVacio> ActualizarAUsuarioInactivo(UsuarioProto request, ServerCallContext context)
        {
            List<Usuario> misUsuarios = this.persistencia.usuarios;
            foreach (var usuario in misUsuarios)
                if (request.Nombre == usuario.NombreUsuario)
                {
                    usuario.UsuarioActivo = false;
                    return await Task.FromResult(new MensajeVacio() { });
                }

            return await Task.FromResult(new MensajeVacio() { });
        }

        public override async Task<BoolProto> EliminarUsuario(UsuarioProto request, ServerCallContext context)
        {
            List<Usuario> misUsuarios = this.persistencia.usuarios;
            foreach (var usuario in misUsuarios)
                if (request.Nombre == usuario.NombreUsuario && usuario.UsuarioActivo==false)
                {
                    this.persistencia.usuarios.Remove(usuario);
                    return await Task.FromResult(new BoolProto() { Estado=true });
                }

            return await Task.FromResult(new BoolProto() { Estado = false });
        }

        public override async Task<MensajeVacio> ModificarUsuario(UsuarioModificacionProto request, ServerCallContext context)
        {
            List<Usuario> misUsuarios = this.persistencia.usuarios;
            foreach (var usuario in misUsuarios)
                if (request.Nombre == usuario.NombreUsuario)
                {
                    usuario.NombreUsuario = request.NombreModificado;
                    return await Task.FromResult(new MensajeVacio() { });
                }

            return await Task.FromResult(new MensajeVacio() { });
        }
    }
}