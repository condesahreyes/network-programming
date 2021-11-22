using ServidorAdministrativo.Protos;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogicaNegocio;
using Grpc.Core;

namespace ServidorAdministrativo.Services
{
    public class JuegoProtoService : ServicioJuego.ServicioJuegoBase
    {
        Persistencia persistencia;
        public JuegoProtoService()
        {
            this.persistencia = Persistencia.ObtenerPersistencia();
        }
        
        public override async Task<ProtoBool> AgregarCalificacion(CalificacionProto calificacion, ServerCallContext context)
        {
            Calificacion calificacionNueva = MapearProtoCalificaciones(calificacion);
            Juego juego = JuegoPorTitulo(calificacion.TituloJuegoo);

            if(juego == null)
                return await Task.FromResult(new ProtoBool() { BoolProto = false });

            juego.calificaciones.Add(calificacionNueva);

            return await Task.FromResult(new ProtoBool() { BoolProto = true });
        }

        public override async Task<ProtoBool> EliminarJuego(Mensaje nombreJuego, ServerCallContext context)
        {
            List<Juego> juegos = this.persistencia.juegos;
            foreach (var juego in juegos)
                if (nombreJuego.Mensaje_ == juego.Titulo)
                {
                    this.persistencia.juegos.Remove(juego);
                    return await Task.FromResult(new ProtoBool() { BoolProto = true });
                }

            return await Task.FromResult(new ProtoBool() { BoolProto = false });
        }

        public override async Task<JuegosProto> BuscarJuegoPorCalificacion(MensajeInt ranking, ServerCallContext context)
        {
            List<Juego> juegos = this.persistencia.juegos;
            JuegosProto juegosRetorno = new JuegosProto();
            foreach (var juego in juegos)
                if (ranking.Mensaje == juego.Ranking)
                {
                    juegosRetorno.Juego.Add(MapperJuegoProto(juego));
                }
            return await Task.FromResult(juegosRetorno);
        }

        public override async Task<JuegoProto> AdquirirJuegoPorUsuario(JuegoPorUsuarioProto juegoUsuario, ServerCallContext context)
        {
            Usuario usuario = ObtenerUsuario(juegoUsuario.NombreUsuario);
            Juego juego = JuegoPorTitulo(juegoUsuario.TituloJuego);
            List<Usuario> usuariosJuego = new List<Usuario>();  

            if (usuario == null || juego == null)
                return null;

            if(juego.usuarios == null)
            {
                usuariosJuego.Add(usuario);
                juego.usuarios = usuariosJuego;
            }
            else
            {
                juego.usuarios.Add(usuario);
            }   

            return await Task.FromResult(MapperJuegoProto(juego));
        }

        public override async Task<JuegosProto> BuscarJuegoPorGenero(Mensaje genero, ServerCallContext context)
        {
            List<Juego> juegos = this.persistencia.juegos;
            JuegosProto juegosRetorno = new JuegosProto();
            foreach (var juego in juegos)
                if (genero.Mensaje_ == juego.Genero)
                {
                    juegosRetorno.Juego.Add(MapperJuegoProto(juego));
                }
            return await Task.FromResult(juegosRetorno);
        }

        public override async Task<JuegosProto> JuegoUsuarios(Protos.ProtoUsuario usuario, ServerCallContext context)
        {
            List<Juego> juegos = JuegosUsuarios(MapearProtoUsuario(usuario));
            JuegosProto juegosProto = new JuegosProto();

            foreach(var juego  in juegos)
            {
                juegosProto.Juego.Add(MapperJuegoProto(juego));
            }

            return await Task.FromResult(juegosProto);
        }

        public override async Task<JuegoProto> BuscarJuegoPortTitulo(Mensaje titulo, ServerCallContext context)
        {
            List<Juego> juegos = this.persistencia.juegos;
            JuegoProto juegoRetorno = new JuegoProto();
            foreach (var juego in juegos)
                if (titulo.Mensaje_ == juego.Titulo)
                {
                    juegoRetorno = MapperJuegoProto(juego);
                }

            return await Task.FromResult(juegoRetorno);
        }

        public override async Task<ProtoBool> EsJuegoExistente(JuegoProto unJuego, ServerCallContext context)
        {
            bool juegoExistente = this.persistencia.juegos.Exists(x => x.Titulo.Equals(unJuego.Titulo));
            return await Task.FromResult(new ProtoBool() { BoolProto = juegoExistente });
        } 
        
        public override async Task<ProtoBool> AgregarJuegos(JuegoProto unJuego, ServerCallContext context)
        {
            JuegoProto juegoNuevo = new JuegoProto();
            bool existeJuego = ExisteJuego(unJuego.Titulo);

            if(existeJuego)
                return await Task.FromResult(new ProtoBool() { BoolProto = false });

            Juego nuevoJuego = MapperProtoJuego(unJuego);
            this.persistencia.juegos.Add(nuevoJuego);

            return await Task.FromResult(new ProtoBool() { BoolProto = true });
        }

        public override async Task<JuegosProto> ObtenerJuegos(Protos.MensajeVacio request, ServerCallContext context)
        {
            List<Juego> juegosDominio = this.persistencia.juegos;
            JuegosProto juegos = new JuegosProto();

            juegosDominio.ForEach(x => juegos.Juego.Add(MapperJuegoProto(x)));

            return await Task.FromResult(juegos);
        }

        public override async Task<ProtoBool> DesasociarJuegoUsuario(JuegoPorUsuarioProto request, ServerCallContext context)
        {
            Juego juego = JuegoPorTitulo(request.TituloJuego);

            foreach (Usuario usuario in juego.usuarios)
                if (usuario.NombreUsuario == request.NombreUsuario)
                {
                    juego.usuarios.Remove(usuario);
                    return await Task.FromResult(new ProtoBool { BoolProto = true });
                }

            return await Task.FromResult(new ProtoBool { BoolProto = false });
        }

        public override async Task<JuegoProto> ModificarJuego(JuegoModificarProto request, ServerCallContext context)
        {
            Juego juego = JuegoPorTitulo(request.TituloJuego);
            juego.Caratula = request.JuegoModificado.Caratula;
            juego.Sinopsis = request.JuegoModificado.Sinposis;
            juego.Genero = request.JuegoModificado.Genero;
            
            return await Task.FromResult(MapperJuegoProto(juego));
        }

        private Usuario ObtenerUsuario(string nombreUsuario)
        {
            return this.persistencia.usuarios.Find(x => x.NombreUsuario == nombreUsuario);
        }

        private bool ExisteJuego(string tituloJuego)
        {
            return this.persistencia.juegos.Exists(x => x.Titulo.Equals(tituloJuego));
        }

        private List<Juego> JuegosUsuarios(Usuario usuario)
        {
            List<Juego> retorno = new List<Juego>();
            foreach(var juego in persistencia.juegos)
            {
              foreach(var usu in juego.usuarios)
                {
                    if(usu.NombreUsuario == usuario.NombreUsuario)
                    {
                        retorno.Add(juego);
                    }
                }
            }

            return retorno;   
        }

        private Juego JuegoPorTitulo(string tituloJuego)
        {
            return this.persistencia.juegos.Find(x => x.Titulo.Equals(tituloJuego));
        }


        private Juego MapperProtoJuego(JuegoProto unJuego)
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

        private JuegoProto MapperJuegoProto(Juego juego)
        {
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
        }

        private ProtoUsuarios MapperUsuariosProto(List<Usuario> usuarios)
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

        private CalificacionesProto MapperCalificacionesProto(List<Calificacion> calificaciones)
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


        private List<Usuario> MapearProtoUsuarios(JuegoProto proto)
        {
            List<Usuario> usuarios = new List<Usuario>();
            List<UsuarioProto> usuariosProto = new List<UsuarioProto>();

            if (proto.Usuarios == null)
                return null;

            foreach (var usu in proto.Usuarios.Usuarios)
                usuarios.Add(MapearProtoUsuario(usu));

            return usuarios;
        }

    

        private static Usuario MapearProtoUsuario(Protos.ProtoUsuario proto)
        {
            return new Usuario(proto.Nombre);
        }

        private List<Calificacion> MapearProtoCalificaciones(JuegoProto proto)
        {
            List<Calificacion> calificaciones = new List<Calificacion>();

            foreach (var usu in proto.Calificaciones.Calificaciones)
                calificaciones.Add(MapearProtoCalificaciones(usu));

            return calificaciones;
        }

        private static Calificacion MapearProtoCalificaciones(CalificacionProto proto)
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
