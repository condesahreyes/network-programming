using ServidorAdministrativo.Protos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;
using LogicaNegocio;
using IServices;
using System;

namespace Servicios
{
    public class JuegoService : IJuegoService
    {
        GrpcChannel canal = GrpcChannel.ForAddress("https://localhost:5001");
        ServicioJuego.ServicioJuegoClient juegoProtoService;
        public JuegoService()
        {
            this.juegoProtoService = new ServicioJuego.ServicioJuegoClient(canal);
        }

        public async Task<bool> EsJuegoExistente(Juego unJuego)
        {
            ProtoBool juegoProto = await juegoProtoService.EsJuegoExistenteAsync(MapperJuegoProto(unJuego));
            return await Task.FromResult(juegoProto.BoolProto);
        }

        public async Task<List<Juego>> ObtenerJuegos()
        {
            JuegosProto juegosProto = await juegoProtoService.ObtenerJuegosAsync(new MensajeVacio() { });
            return await (MapearJuegosProto(juegosProto));
        }

        public async Task<bool> AgregarJuego(Juego juego)
        {
            ProtoBool agregado = await juegoProtoService.AgregarJuegosAsync(MapperJuegoProto(juego));

            return await Task.FromResult(agregado.BoolProto);
        }

        public async Task<bool> AgregarCalificacion(Calificacion calificacion)
        {
            ProtoBool agregada = await juegoProtoService.AgregarCalificacionAsync(MapperCalificacionProto(calificacion));
            return await Task.FromResult(agregada.BoolProto);
        }

        public async Task<Juego> AdquirirJuegoPorUsuario(string juego, Usuario usuario)
        {
            JuegoProto juegoProto = await juegoProtoService.AdquirirJuegoPorUsuarioAsync(new JuegoPorUsuarioProto { TituloJuego = juego, NombreUsuario = usuario.NombreUsuario });
            return await Task.FromResult(MapperProtoJuego(juegoProto)); 
        }


        public async Task<List<Juego>> JuegoUsuarios(Usuario usuario)
        {

            JuegosProto jugoRequest = await juegoProtoService.JuegoUsuariosAsync(MapearUsuarioProto(usuario));
            return await Task.FromResult(MapperProtoJuegos(jugoRequest));
  
        }

        public async Task<Juego> BuscarJuegoPortTitulo(string unTitulo)
        {
            JuegoProto juegoProto = await juegoProtoService.BuscarJuegoPortTituloAsync(new Mensaje { Mensaje_ = unTitulo });
            return await Task.FromResult(MapperProtoJuego(juegoProto));
        }

        public async Task<List<Juego>> BuscarJuegoPorGenero(string unGenero)
        {
            JuegosProto juegoProto = await juegoProtoService.BuscarJuegoPorGeneroAsync(new Mensaje { Mensaje_ = unGenero });
            return await Task.FromResult(MapperProtoJuegos(juegoProto));
        }

        public async  Task<Juego> ObtenerJuegoPorTitulo(string tituloJuego)
        {
           JuegoProto juegoProto = await juegoProtoService.BuscarJuegoPortTituloAsync(new Mensaje { Mensaje_ = tituloJuego});
            return await Task.FromResult(MapperProtoJuego(juegoProto));
        }

        public async Task<List<Juego>> BuscarJuegoPorCalificacion(int ranking)
        {
            JuegosProto juegoProto = await juegoProtoService.BuscarJuegoPorCalificacionAsync(new MensajeInt { Mensaje = ranking});
            return await Task.FromResult(MapperProtoJuegos(juegoProto));
        }

        public async Task<bool> EliminarJuego(string tituloJuego)
        {
            ProtoBool protoBool = await juegoProtoService.EliminarJuegoAsync(new Mensaje { Mensaje_ = tituloJuego});
            return await Task.FromResult(protoBool.BoolProto);
        }

        private Task<List<Juego>> MapearJuegosProto(JuegosProto juegos)
        {
            List<Juego> listaJuegos = new List<Juego>();

            foreach (var unJuego in juegos.Juego)
            {
                listaJuegos.Add(MapperProtoJuego(unJuego));
            }

            return Task.FromResult(listaJuegos);
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
                Calificaciones = MapperCalificacionesProto(juego.calificaciones)
            };
        }


        private CalificacionProto MapperCalificacionProto(Calificacion calificacion)
        {
            return new CalificacionProto()
            {
                Comentario = calificacion.Comentario,
                Nota = calificacion.Nota,
                TituloJuegoo = calificacion.TituloJuego,
                Usuario = calificacion.Usuario
            };
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

        private List<Juego> MapperProtoJuegos(JuegosProto proto)
        {
            List<Juego> juegos = new List<Juego>();
            List<JuegoProto> juegosProto = new List<JuegoProto>();

            if (proto != null)
                foreach (var juego in proto.Juego)
                    juegos.Add(MapperProtoJuego(juego));

            return juegos;
        }


        private List<Usuario> MapearProtoUsuarios(JuegoProto proto)
        {
            List<Usuario> usuarios = new List<Usuario>();
            List<ProtoUsuario> usuariosProto = new List<ProtoUsuario>();

            if (proto.Usuarios != null)
                foreach (var usu in proto.Usuarios.Usuarios)
                    usuarios.Add(MapearProtoUsuario(usu));

            return usuarios;
        }

        private ProtoUsuario MapearUsuarioProto(Usuario usuario)
        {
            return new ProtoUsuario
            {
                Nombre = usuario.NombreUsuario
            };
                
        }


        private static Usuario MapearProtoUsuario(ProtoUsuario proto)
        {
            return new Usuario(proto.Nombre);
        }

        private List<Calificacion> MapearProtoCalificaciones(JuegoProto proto)
        {
            List<Calificacion> calificaciones = new List<Calificacion>();

            if (proto.Calificaciones != null)
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

        public Task<bool> DesasociarJuegoAUsuario(string juego, Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public Task<Juego> ModificarJuego(string tiuuloJuego, Juego juegoModificado)
        {
            throw new NotImplementedException();
        }
    }
}
