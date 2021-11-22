using ServidorAdministrativo.Protos;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogicaNegocio;

namespace Servicios.Mappers
{
    public class MapperJuego
    {

        public static Task<List<Juego>> MapearJuegosProto(JuegosProto juegos)
        {
            List<Juego> listaJuegos = new List<Juego>();

            foreach (var unJuego in juegos.Juego)
            {
                listaJuegos.Add(MapperProtoJuego(unJuego));
            }

            return Task.FromResult(listaJuegos);
        }

        public static JuegoProto MapperJuegoProto(Juego juego)
        {
            return new JuegoProto()
            {
                Titulo = juego.Titulo,
                Sinposis = juego.Sinopsis,
                Genero = juego.Genero,
                Notas = juego.Notas,
                Caratula = juego.Caratula,
                Ranking = juego.Ranking,
                Calificaciones = MapperCalificacionesProto(juego.calificaciones)
            };
        }
        public static CalificacionProto MapperCalificacionProto(Calificacion calificacion)
        {
            return new CalificacionProto()
            {
                Comentario = calificacion.Comentario,
                Nota = calificacion.Nota,
                TituloJuegoo = calificacion.TituloJuego,
                Usuario = calificacion.Usuario
            };
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
        public static List<Juego> MapperProtoJuegos(JuegosProto proto)
        {
            List<Juego> juegos = new List<Juego>();
            List<JuegoProto> juegosProto = new List<JuegoProto>();

            if (proto != null)
                foreach (var juego in proto.Juego)
                    juegos.Add(MapperProtoJuego(juego));

            return juegos;
        }

        public static List<Usuario> MapearProtoUsuarios(JuegoProto proto)
        {
            List<Usuario> usuarios = new List<Usuario>();
            List<ProtoUsuario> usuariosProto = new List<ProtoUsuario>();

            if (proto.Usuarios != null)
                foreach (var usu in proto.Usuarios.Usuarios)
                    usuarios.Add(MapearProtoUsuario(usu));

            return usuarios;
        }

        public static ProtoUsuario MapearUsuarioProto(Usuario usuario)
        {
            return new ProtoUsuario
            {
                Nombre = usuario.NombreUsuario
            };

        }

        public static Usuario MapearProtoUsuario(ProtoUsuario proto)
        {
            return new Usuario(proto.Nombre);
        }

        public static List<Calificacion> MapearProtoCalificaciones(JuegoProto proto)
        {
            List<Calificacion> calificaciones = new List<Calificacion>();

            if (proto.Calificaciones != null)
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
