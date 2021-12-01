using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServidorAdministrativo;
using Servicios.Mappers;
using Grpc.Net.Client;
using LogicaNegocio;
using IServices;
using System.IO;

namespace Servicios
{ 
    public class UsuarioService : IUsuarioService {

        IConfiguration configuracion = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AppSettings.json", optional: false).Build();

        ServicioUsuario.ServicioUsuarioClient userProtoService;

        public UsuarioService()
        {
            GrpcChannel canal = GrpcChannel.ForAddress(configuracion["canalGrpc"]);
            this.userProtoService = new ServicioUsuario.ServicioUsuarioClient(canal);
        }

        public async Task<Usuario> ObtenerUsuarioAsync(Usuario usuario)
        {
            List<Usuario> usuariosDominio = await ObtenerUsuariosAsync();

            Usuario miUsuario = usuariosDominio.Find(x => x.NombreUsuario == usuario.NombreUsuario);
            
            if (miUsuario != null)
                return miUsuario;

            UsuarioProto usuarioRequest = new UsuarioProto { Nombre = usuario.NombreUsuario };
            await userProtoService.AltaUsuarioAsync(usuarioRequest);

            return usuario;
        }

        public async Task ActualizarAUsuarioInactivoAsync(string nombreUsuario)
        {
            UsuarioProto usuarioRequest = new UsuarioProto { Nombre = nombreUsuario };
            await userProtoService.ActualizarAUsuarioInactivoAsync(usuarioRequest);
        }

        public async Task<List<Usuario>> ObtenerUsuariosAsync()
        {
            UsuariosProto usuariosProto = await userProtoService.ObtenerUsuariosAsync(new MensajeVacio());

            List<Usuario> usuariosDominio = MapperUsuario.MapearProtoUsuarios(usuariosProto);

            return usuariosDominio;
        }

        public async Task ActualizarAUsuarioActivoAsync(string nombreUsuario)
        {
            UsuarioProto usuarioRequest = new UsuarioProto { Nombre = nombreUsuario };
            await userProtoService.ActualizarAUsuarioActivoAsync(usuarioRequest);
        }

        public async Task<bool> EliminarUsuarioAsync(string nombreUsuario)
        {
            UsuarioProto usuarioRequest = new UsuarioProto { Nombre = nombreUsuario };
            BoolProto protoBool = await userProtoService.EliminarUsuarioAsync(usuarioRequest);

            return protoBool.Estado;
        }

        public async Task<bool> ModificarUsuarioAsync(string nombreUsuario, string nuevoNombreUsuario)
        {
            List<Usuario> usuarios = await ObtenerUsuariosAsync();
            UsuarioModificacionProto modificacionProto = new UsuarioModificacionProto();

            modificacionProto.Nombre = nombreUsuario;
            modificacionProto.NombreModificado = nuevoNombreUsuario;

            foreach (Usuario unUsuario in usuarios)
                if (unUsuario.NombreUsuario == nombreUsuario && unUsuario.UsuarioActivo == false)
                {
                    BoolProto modificado = await userProtoService.ModificarUsuarioAsync(modificacionProto);
                    return modificado.Estado;
                }

            return false;
        }
    }
}
