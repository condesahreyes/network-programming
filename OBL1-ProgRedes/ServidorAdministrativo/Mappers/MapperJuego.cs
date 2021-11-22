using ServidorAdministrativo.Protos;
using System.Collections.Generic;
using LogicaNegocio;

namespace ServidorAdministrativo.Mappers
{
    public class MapperJuego
    {
        public static Juego MapperProtoJuego(JuegoProto unJuego)
        {
            return new Juego
            {
                Sinopsis = unJuego.Sinposis,
                Caratula = unJuego.Caratula,
                Genero = unJuego.Genero,
                Notas = unJuego.Notas,
                Ranking = unJuego.Ranking,
                Titulo = unJuego.Titulo,
                usuarios = MapearProtoUsuarios(unJuego),
                calificaciones = MapearProtoCalificaciones(unJuego)
            };
        }

        public static JuegoProto MapperJuegoProto(Juego juego)
        {
            if (juego != null)
                return new JuegoProto()
                {
                    Titulo = juego.Titulo,
                    Sinposis = juego.Sinopsis,
                    Genero = juego.Genero,
                    Notas = juego.Notas,
                    Caratula = juego.Caratula,
                    Ranking = juego.Ranking,
                    Usuarios = MapperUsuariosProto(juego.usuarios),
                    Calificaciones = MapperCalificacionesProto(juego.calificaciones)
                };

            return null;
        }

        public static ProtoUsuarios MapperUsuariosProto(List<Usuario> usuarios)
        {
            if (usuarios == null)
                return null;

            ProtoUsuarios usuariosProto = new ProtoUsuarios();
            foreach (Usuario usuario in usuarios)
            {
                ProtoUsuario usuarioProto = new ProtoUsuario()
                {
                    Nombre = usuario.NombreUsuario
                };

                usuariosProto.Usuarios.Add(usuarioProto);
            }

            return usuariosProto;
        }

        public static CalificacionesProto MapperCalificacionesProto(List<Calificacion> calificaciones)
        {
            CalificacionesProto calificacionesProto = new CalificacionesProto();
            foreach (Calificacion calificacion in calificaciones)
            {
                CalificacionProto miCalificacion = new CalificacionProto()
                {
                    Comentario = calificacion.Comentario,
                    Nota = calificacion.Nota,
                    TituloJuegoo = calificacion.TituloJuego,
                    Usuario = calificacion.Usuario
                };

                calificacionesProto.Calificaciones.Add(miCalificacion);
            }

            return calificacionesProto;
        }
        public static List<Usuario> MapearProtoUsuarios(JuegoProto proto)
        {
            List<Usuario> usuarios = new List<Usuario>();
            List<UsuarioProto> usuariosProto = new List<UsuarioProto>();

            if (proto.Usuarios == null)
                return null;

            foreach (var usu in proto.Usuarios.Usuarios)
                usuarios.Add(MapearProtoUsuario(usu));

            return usuarios;
        }

        public static Usuario MapearProtoUsuario(Protos.ProtoUsuario proto)
        {
            return new Usuario(proto.Nombre);
        }

        public static List<Calificacion> MapearProtoCalificaciones(JuegoProto proto)
        {
            List<Calificacion> calificaciones = new List<Calificacion>();

            foreach (var usu in proto.Calificaciones.Calificaciones)
                calificaciones.Add(MapearProtoCalificaciones(usu));

            return calificaciones;
        }

        public static Calificacion MapearProtoCalificaciones(CalificacionProto proto)
        {
            return new Calificacion
            {
                Comentario = proto.Comentario,
                Nota = proto.Nota,
                TituloJuego = proto.TituloJuegoo,
                Usuario = proto.Usuario
            };
        }
    }
}
