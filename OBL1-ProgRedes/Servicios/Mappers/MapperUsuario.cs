using System.Collections.Generic;
using ServidorAdministrativo;
using LogicaNegocio;

namespace Servicios.Mappers
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
                usuarios.Add(MapearProtoUsuario(usu));

            return usuarios;
        }
    }
}
