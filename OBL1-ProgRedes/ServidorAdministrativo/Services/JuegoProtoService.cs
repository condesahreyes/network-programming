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
            foreach (Juego juego in this.persistencia.juegos)
            {
                if (juego.Titulo == calificacion.TituloJuegoo)
                {
                    Calificacion nuevaCalificacion = new Calificacion
                    {
                        Comentario = calificacion.Comentario,
                        Nota = calificacion.Nota,
                        TituloJuego = calificacion.TituloJuegoo,
                        Usuario = calificacion.Usuario
                    };

                    juego.calificaciones.Add(nuevaCalificacion);
                    return await Task.FromResult(new ProtoBool { BoolProto = true });
                }
            }

            return await Task.FromResult(new ProtoBool { BoolProto = false });
        }

        public override async Task<ProtoBool> EliminarJuego(Mensaje titulo, ServerCallContext context)
        {
            bool retorno = false;
            JuegosProto juegosProto = await ObtenerJuegos(new Protos.MensajeVacio() { }, context);

            foreach (JuegoProto juego in juegosProto.Juego)
                if (juego.Titulo == titulo.Mensaje_)
                {
                    juegosProto.Juego.Remove(juego);
                    retorno = true;
                }

            return await Task.FromResult(new ProtoBool() { BoolProto = retorno });
        }

        public override async Task<JuegosProto> BuscarJuegoPorCalificacion(MensajeInt ranking, ServerCallContext context)
        {
            JuegosProto juegosProto = await ObtenerJuegos(new Protos.MensajeVacio() { }, context);
            JuegosProto juegosARetornar = new JuegosProto();

            foreach (JuegoProto juego in juegosProto.Juego)
            {
                if (juego.Ranking == ranking.Mensaje)
                {
                    juegosARetornar.Juego.Add(juego);
                }
            }
            return await Task.FromResult(juegosARetornar);
        }

        public override async Task<JuegoProto> AdquirirJuegoPorUsuario(JuegoPorUsuarioProto juegoUsuario, ServerCallContext context)
        {
            Usuario usuario = ObtenerUsuario(juegoUsuario.NombreUsuario);
            JuegoProto juegoProto = await BuscarJuegoPortTitulo(new Mensaje() { Mensaje_ = juegoUsuario.TituloJuego }, context);

            if (usuario == null || juegoProto == null)
                return null;
            
            foreach (Juego juego in this.persistencia.juegos)
            {
                if(juego.Titulo == juegoUsuario.TituloJuego)
                {
                    juego.usuarios.Add(usuario);
                    return await Task.FromResult(juegoProto);
                }   
            }

            return await Task.FromResult(juegoProto);
        }

        public override async Task<JuegoProto> BuscarJuegoPorGenero(GeneroProto genero, ServerCallContext context)
        {
            JuegosProto juegosProto = await ObtenerJuegos(new Protos.MensajeVacio() { }, context);
            JuegoProto juegoARetornar = new JuegoProto();

            foreach (JuegoProto juego in juegosProto.Juego)
            {
                if (juego.Genero == genero.Genero)
                {
                    juegoARetornar = juego;
                }
            }
            return await Task.FromResult(juegoARetornar);
        }

        public override async Task<JuegosProto> JuegoUsuarios(Protos.UsuarioProto usuario, ServerCallContext context)
        {
            JuegosProto juegosProto = await ObtenerJuegos(new Protos.MensajeVacio() { }, context);
            JuegosProto juegosARetornar = new JuegosProto();

           foreach(JuegoProto juego in juegosProto.Juego)
           {
                foreach(Protos.UsuarioProto unUsuario in juego.Usuarios)
                {
                    if(unUsuario.Nombre == usuario.Nombre)
                    {
                        juegosARetornar.Juego.Add(juego);
                    }
                }
            }

           return await Task.FromResult(juegosARetornar);
        }

        public override async Task<JuegoProto> BuscarJuegoPortTitulo(Mensaje titulo, ServerCallContext context)
        {
            JuegosProto juegosProto = await ObtenerJuegos(new Protos.MensajeVacio() { }, context);
            JuegoProto juegoARetornar = new JuegoProto();

            foreach(JuegoProto juego in juegosProto.Juego)
            {
                if(juego.Titulo == titulo.Mensaje_) {
                    juegoARetornar = juego;
                }
            }

            return await Task.FromResult(juegoARetornar);
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
            List<JuegoProto> juegosProto = new List<JuegoProto>();
            List<Juego> juegosDominio = this.persistencia.juegos;

            JuegosProto juegos = new JuegosProto();

            juegosDominio.ForEach(x => juegosProto.Add(new JuegoProto { Titulo = x.Titulo }));

            juegos.Juego.AddRange(juegosProto);

            return await Task.FromResult(juegos);
        }

        private Usuario ObtenerUsuario(string nombreUsuario)
        {
            return this.persistencia.usuarios.Find(x => x.NombreUsuario == nombreUsuario);
        }

        private bool ExisteJuego(string tituloJuego)
        {
            return this.persistencia.juegos.Exists(x => x.Titulo.Equals(tituloJuego));
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

        private List<Usuario> MapearProtoUsuarios(JuegoProto proto)
        {
            List<Usuario> usuarios = new List<Usuario>();
            List<UsuarioProto> usuariosProto = new List<UsuarioProto>();

            foreach (var usu in proto.Usuarios)
                usuarios.Add(MapearProtoUsuario(usu));

            return usuarios;
        }

        private static Usuario MapearProtoUsuario(Protos.UsuarioProto proto)
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
