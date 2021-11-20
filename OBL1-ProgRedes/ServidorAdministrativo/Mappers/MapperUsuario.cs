using LogicaNegocio;
using System.Collections.Generic;

namespace ServidorAdministrativo.Mappers
{
    public class MapperUsuario
    {
        public static Usuario MapearProtoUsuario(UsuarioProto proto)
        {
            return new Usuario(proto.Nombre);
        }

        public static List<Usuario> MapearProtoUsuarios(UsuariosProto proto)
        {
            List<Usuario> usuarios = new List<Usuario>();
            List<UsuarioProto> usuariosProto = new List<UsuarioProto>();

            foreach (var usu in proto.Usuario)
                usuarios.Add(MapperUsuario.MapearProtoUsuario(usu));

            return usuarios;
        }

        public static UsuariosProto MapearUsuariosProto(List<Usuario> misUsuarios)
        {
            List<Usuario> usuariosDominio = misUsuarios;

            UsuariosProto usuarios = new UsuariosProto();

            usuariosDominio.ForEach(x => usuarios.Usuario.Add(new UsuarioProto { Nombre = x.NombreUsuario }));

            return usuarios;
        }
    }
}
