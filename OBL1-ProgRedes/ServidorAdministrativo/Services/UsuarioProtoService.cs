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

        public override async Task<RespuestaProto> AltaUsuario(UsuarioProto request, ServerCallContext context)
        {
            Usuario usuario = new Usuario(request.Nombre);
            repositorioUsuario.AgregarUsuario(usuario);

            this.logServices.SendMessages("usuario " + request.Nombre + " dado de alta");
            return await Task.FromResult(new RespuestaProto
            {
                Estado = "Ok"
            });
        }

        public override async Task<UsuariosProto> ObtenerUsuarios(MensajeVacio request, ServerCallContext context)
        {
            List<Usuario> usuariosDominio = repositorioUsuario.ObtenerUsuarios();

            UsuariosProto usuarios = new UsuariosProto();

            usuariosDominio.ForEach(x => usuarios.Usuario.Add(new UsuarioProto { Nombre = x.NombreUsuario }));

            this.logServices.SendMessages("Lista de usuarios solicitada");
            return await Task.FromResult(usuarios);
        }

        public override async Task<MensajeVacio> ActualizarAUsuarioActivo(UsuarioProto request, ServerCallContext context)
        {
            repositorioUsuario.ActualizarEstadoUsuario(request.Nombre, true);

            this.logServices.SendMessages("usuario " + request.Nombre + " inicio sesión");
            return await Task.FromResult(new MensajeVacio() { });
        }

        public override async Task<MensajeVacio> ActualizarAUsuarioInactivo(UsuarioProto request, ServerCallContext context)
        {
            repositorioUsuario.ActualizarEstadoUsuario(request.Nombre, false);

            this.logServices.SendMessages("usuario " + request.Nombre + " cerro sesión");
            return await Task.FromResult(new MensajeVacio() { });
        }

        public override async Task<BoolProto> EliminarUsuario(UsuarioProto request, ServerCallContext context)
        {
            bool eliminado = repositorioUsuario.EliminarUsuario(request.Nombre);

            if(eliminado)
                this.logServices.SendMessages("usuario " + request.Nombre + " eliminado");

            return await Task.FromResult(new BoolProto() { Estado = eliminado });
        }

        public override async Task<MensajeVacio> ModificarUsuario(UsuarioModificacionProto request, ServerCallContext context)
        {
            bool modificado = repositorioUsuario.ModificarUsuario(request.Nombre, request.NombreModificado);

            if(modificado)
                this.logServices.SendMessages("usuario " + request.Nombre + " modificado");

            return await Task.FromResult(new MensajeVacio() { });
        }
    }
}