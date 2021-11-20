using System.Collections.Generic;
using System.Threading.Tasks;
using ServidorAdministrativo;
using Grpc.Net.Client;
using LogicaNegocio;
using IServices;

namespace Servicios
{ 
    public class UsuarioService : IUsuarioService {

        GrpcChannel canal = GrpcChannel.ForAddress("https://localhost:5001");
        ServicioUsuario.ServicioUsuarioClient userProtoService;

        public UsuarioService()
        {
            this.userProtoService = new ServicioUsuario.ServicioUsuarioClient(canal);
        }

        public async Task<Usuario> ObtenerUsuario(Usuario usuario)
        {
            List<Usuario> usuariosDominio = await ObtenerUsuarios();

            Usuario miUsuario = usuariosDominio.Find(x => x.NombreUsuario == usuario.NombreUsuario);
            
            if (miUsuario != null)
                return miUsuario;

            UsuarioProto usuarioRequest = new UsuarioProto { Nombre = usuario.NombreUsuario };
            await userProtoService.AltaUsuarioAsync(usuarioRequest);

            return usuario;
        }

        public async Task ActualizarAUsuarioInactivo(string nombreUsuario)
        {
            UsuarioProto usuarioRequest = new UsuarioProto { Nombre = nombreUsuario };
            await userProtoService.ActualizarAUsuarioInactivoAsync(usuarioRequest);
        }

        public async Task<List<Usuario>> ObtenerUsuarios()
        {
            UsuariosProto usuariosProto = await userProtoService.ObtenerUsuariosAsync(new MensajeVacio());

            List<Usuario> usuariosDominio = MapearProtoUsuarios(usuariosProto);

            return usuariosDominio;
        }

        public async Task ActualizarAUsuarioActivo(string nombreUsuario)
        {
            UsuarioProto usuarioRequest = new UsuarioProto { Nombre = nombreUsuario };
            await userProtoService.ActualizarAUsuarioActivoAsync(usuarioRequest);
        }

        public async Task<bool> EliminarUsuario(string nombreUsuario)
        {
            UsuarioProto usuarioRequest = new UsuarioProto { Nombre = nombreUsuario };
            BoolProto protoBool = await userProtoService.EliminarUsuarioAsync(usuarioRequest);

            return protoBool.Estado;
        }

        public async Task<bool> ModificarUsuario(string nombreUsuario, string nuevoNombreUsuario)
        {
            List<Usuario> usuarios = await ObtenerUsuarios();

            foreach (Usuario unUsuario in usuarios)
                if (unUsuario.NombreUsuario == nombreUsuario && unUsuario.UsuarioActivo == false)
                {
                    unUsuario.NombreUsuario = nuevoNombreUsuario;
                    return true;
                }
                else if (unUsuario.NombreUsuario == nombreUsuario && unUsuario.UsuarioActivo == true)
                {
                    return false;
                }

            return false;
        }

        private async Task<bool> NoEsUsuarioExistente(Usuario unUsuario)
        {
            List<Usuario> misUsuarios = await ObtenerUsuarios();

            foreach (var usuario in misUsuarios)
                if (unUsuario.NombreUsuario == usuario.NombreUsuario)
                    return false;

            return true;
        }

        private async Task<Usuario> DevolverUsuarioExistente(Usuario unUsuario)
        {
            List<Usuario> misUsuarios = await ObtenerUsuarios();
            foreach (var usuario in misUsuarios)
                if (unUsuario.NombreUsuario == usuario.NombreUsuario)
                    return usuario;

            return null;
        }

        private static Usuario MapearProtoUsuario(UsuarioProto proto)
        {
            return new Usuario(proto.Nombre);
        }

        private List<Usuario> MapearProtoUsuarios(UsuariosProto proto)
        {
            List<Usuario> usuarios = new List<Usuario>();
            List<UsuarioProto> usuariosProto = new List<UsuarioProto>();

            foreach (var usu in proto.Usuario)
                usuarios.Add(MapearProtoUsuario(usu));

            return usuarios;
        }

        //IMPORTANTE BORRAR ESTO SI NO LO VOY A USAR
        private UsuariosProto MapearUsuariosProto(List<Usuario> misUsuarios)
        {
            List<Usuario> usuariosDominio = misUsuarios;

            UsuariosProto usuarios = new UsuariosProto();

            usuariosDominio.ForEach(x => usuarios.Usuario.Add(new UsuarioProto { Nombre = x.NombreUsuario }));

            return usuarios;
        }
    }
}
