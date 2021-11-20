using System.Collections.Generic;
using LogicaNegocio;
using System;
using IServices;
using Grpc.Net.Client;
using ServidorAdministrativo.Protos;
using System.Threading.Tasks;

namespace Servicios
{
    public class JuegoService: IJuegoService
    {
        GrpcChannel canal = GrpcChannel.ForAddress("https://localhost:5001");
        ServicioJuego.ServicioJuegoClient juegoProtoService;
        public JuegoService()
        {
            this.juegoProtoService = new ServicioJuego.ServicioJuegoClient(canal);
        }

        public bool EsJuegoExistente(Juego unJuego)
        {/*
            lock (persistencia.juegos)
            {
                List<Juego> juegos = persistencia.juegos;
                foreach (var juego in juegos)
                    if (unJuego.Titulo == juego.Titulo)
                        return true;

                return false;
            }*/
            return false;
        }

        public async Task<List<Juego>> ObtenerJuegos()
        {
            JuegosProto juegosProto = await juegoProtoService.ObtenerJuegosAsync(new MensajeVacio() { });
            return await (MapearJuegosProto(juegosProto));
        }

        public async Task<bool> AgregarJuego(Juego juego)
        {
            ProtoBool agregado =  await juegoProtoService.AgregarJuegosAsync(MapperJuegoProto(juego));

            return await Task.FromResult(agregado.BoolProto);
        }

        public bool AgregarCalificacion(Calificacion calificacion)
        {/*
            Juego juego = BuscarJuegoPortTitulo(calificacion.TituloJuego);

            if (juego == null)
                return false;

            lock (persistencia.juegos)
            {
                juego.calificaciones.Add(calificacion);

                juego.Notas += calificacion.Nota;
                juego.Ranking = Math.Abs((juego.Notas) / juego.calificaciones.Count);
            }

            return true;
            */
            return false;
        }

        public Juego AdquirirJuegoPorUsuario(string juego, Usuario usuario)
        {/*
            lock (persistencia)
            {
                Juego unJuego = ObtenerJuegoPorTitulo(juego);
                if (unJuego == null)
                    return unJuego;
                foreach (Usuario unUsuario in persistencia.usuarios)
                {
                    if (unUsuario.NombreUsuario == usuario.NombreUsuario)
                    {
                        unJuego.usuarios.Add(unUsuario);
                        return unJuego;
                    }
                }
                return unJuego;
            }
            */
            return null;
        }

        public List<Juego> JuegoUsuarios(Usuario usuario)
        {/*
            lock (persistencia)
            {
                List<Juego> juegosUsuario = new List<Juego>();
                foreach (Juego juego in persistencia.juegos)
                {
                    foreach (Usuario unUsuario in juego.usuarios)
                    {
                        if (unUsuario.NombreUsuario == usuario.NombreUsuario)
                        {
                            juegosUsuario.Add(juego);
                        }
                    }
                }
                return juegosUsuario;
            }
            */
            return null;
        }

        public Juego BuscarJuegoPortTitulo(string unTitulo)
        {/*
            lock (persistencia.juegos)
            {
                List<Juego> juegos = persistencia.juegos;
                foreach (var juego in juegos)
                    if (unTitulo == juego.Titulo)
                        return juego;

                return null;
            }
            */
            return null;
        }

        public List<Juego> BuscarJuegoPorGenero(string unGenero)
        {
            /*
            lock (persistencia.juegos)
            {
                List<Juego> juegos = persistencia.juegos;
                List<Juego> juegosPorgenero = new List<Juego>();

                foreach (var juego in juegos)
                    if (unGenero == juego.Genero)
                        juegosPorgenero.Add(juego);

                return juegosPorgenero;
            }
            */
            return null;
        }

        public Juego ObtenerJuegoPorTitulo(string tituloJuego)
        {
            /*
            lock (persistencia.juegos)
            {
                List<Juego> juegos = persistencia.juegos;
                foreach (Juego juego in juegos)
                    if (juego.Titulo == tituloJuego)
                        return juego;

                return null;
            }
            */
            return null;

        }

        public List<Juego> BuscarJuegoPorCalificacion(int ranking)
        {
            /*
            lock (persistencia.juegos)
            {
                List<Juego> juegos = new List<Juego>();

                foreach (Juego juego in persistencia.juegos)
                    if (juego.Ranking == ranking)
                        juegos.Add(juego);

                return juegos;
            }
            */
            return null;
        }

        public bool EliminarJuego(string tituloJuego)
        {
            /*
            lock (persistencia.juegos)
            {
                foreach (Juego juego in persistencia.juegos)
                    if (juego.Titulo == tituloJuego)
                    {
                        persistencia.juegos.Remove(juego);
                        return true;
                    }
            }
            return false;
            */
            return false;
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

        private List<Usuario> MapearProtoUsuarios(JuegoProto proto)
        {
            List<Usuario> usuarios = new List<Usuario>();
            List<UsuarioProto> usuariosProto = new List<UsuarioProto>();

            if(proto.Usuarios != null)
            foreach (var usu in proto.Usuarios)
                usuarios.Add(MapearProtoUsuario(usu));

            return usuarios;
        }

        private static Usuario MapearProtoUsuario(UsuarioProto proto)
        {
            return new Usuario(proto.Nombre);
        }

        private List<Calificacion> MapearProtoCalificaciones(JuegoProto proto)
        {
            List<Calificacion> calificaciones = new List<Calificacion>();

            if(proto.Calificaciones != null)
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
