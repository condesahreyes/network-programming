using System.Collections.Generic;
using System.Threading.Tasks;
using LogicaNegocio;
using IRepositorio;
using Grpc.Core;

namespace ServidorAdministrativo.Services
{
    public class UsuarioProtoService : ServicioUsuario.ServicioUsuarioBase
    {
        LogServices logServices;
        IRepositorioUsuario repositorioUsuario;

        public UsuarioProtoService(LogServices logServices, IRepositorioUsuario repositorioUsuario)
        {
            this.logServices = logServices;
            this.repositorioUsuario = repositorioUsuario;
        }

        public override async Task<RespuestaProto> AltaUsuarioAsync(UsuarioProto request, ServerCallContext context)
        {
            Usuario usuario = new Usuario(request.Nombre);
            repositorioUsuario.AgregarUsuario(usuario);

            this.logServices.EnviarMensaje("usuario " + request.Nombre + " dado de alta");
            return await Task.FromResult(new RespuestaProto
            {
                Estado = "Ok"
            });
        }

        public override async Task<UsuariosProto> ObtenerUsuariosAsync(MensajeVacio request, ServerCallContext context)
        {
            List<Usuario> usuariosDominio = repositorioUsuario.ObtenerUsuarios();

            UsuariosProto usuarios = new UsuariosProto();

            usuariosDominio.ForEach(x => usuarios.Usuario.Add(new UsuarioProto { Nombre = x.NombreUsuario }));

            this.logServices.EnviarMensaje("Lista de usuarios solicitada");
            return await Task.FromResult(usuarios);
        }

        public override async Task<MensajeVacio> ActualizarAUsuarioActivoAsync(UsuarioProto request, ServerCallContext context)
        {
            repositorioUsuario.ActualizarEstadoUsuario(request.Nombre, true);

            this.logServices.EnviarMensaje("usuario " + request.Nombre + " inicio sesión");
            return await Task.FromResult(new MensajeVacio() { });
        }

        public override async Task<MensajeVacio> ActualizarAUsuarioInactivoAsync(UsuarioProto request, ServerCallContext context)
        {
            repositorioUsuario.ActualizarEstadoUsuario(request.Nombre, false);

            this.logServices.EnviarMensaje("usuario " + request.Nombre + " cerro sesión");
            return await Task.FromResult(new MensajeVacio() { });
        }

        public override async Task<BoolProto> EliminarUsuarioAsync(UsuarioProto request, ServerCallContext context)
        {
            bool eliminado = repositorioUsuario.EliminarUsuario(request.Nombre);

            if(eliminado)
                this.logServices.EnviarMensaje("usuario " + request.Nombre + " eliminado");

            return await Task.FromResult(new BoolProto() { Estado = eliminado });
        }

        public override async Task<BoolProto> ModificarUsuarioAsync(UsuarioModificacionProto request, ServerCallContext context)
        {
            bool modificado = repositorioUsuario.ModificarUsuario(request.Nombre, request.NombreModificado);

            if(modificado)
                this.logServices.EnviarMensaje("usuario " + request.Nombre + " modificado");

            return await Task.FromResult(new BoolProto() { Estado = modificado });
        }
    }
}