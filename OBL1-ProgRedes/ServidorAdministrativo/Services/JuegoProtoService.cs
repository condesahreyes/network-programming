using Grpc.Core;
using LogicaNegocio;
using ServidorAdministrativo.Protos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServidorAdministrativo.Services
{
    public class JuegoProtoService : ServicioJuego.ServicioJuegoBase
    {
        Persistencia persistencia;
        public JuegoProtoService()
        {
            this.persistencia = Persistencia.ObtenerPersistencia();
        }
        
        public override async Task<BoolProto> AgregarCalificacion(CalificacionProto calificacion, ServerCallContext context)
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
                    return await Task.FromResult(new BoolProto { BoolProto_ = true });
                }
            }

            return await Task.FromResult(new BoolProto { BoolProto_ = false });

        }

        public override async Task<BoolProto> EliminarJuego(Mensaje titulo, ServerCallContext context)
        {
            bool retorno = false;
            JuegosProto juegosProto = await ObtenerJuegos(new Protos.MensajeVacio() { }, context);

            foreach (JuegoProto juego in juegosProto.Juego)
                if (juego.Titulo == titulo.Mensaje_)
                {
                    juegosProto.Juego.Remove(juego);
                    retorno = true;
                }

            return await Task.FromResult(new BoolProto() { BoolProto_ = retorno });
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

        public override async Task<BoolProto> EsJuegoExistente(JuegoProto unJuego, ServerCallContext context)
        {
            bool juegoExistente = this.persistencia.juegos.Exists(x => x.Titulo.Equals(unJuego.Titulo));
            return await Task.FromResult(new BoolProto() { BoolProto_ = juegoExistente });
        } 

        
        public override async Task<BoolProto> AgregarJuegos(JuegoProto unJuego, ServerCallContext context)
        {
            JuegoProto juegoNuevo = new JuegoProto();
            bool agregaJuego = false;
            JuegosProto juegosProto = await ObtenerJuegos(new Protos.MensajeVacio() { }, context);

            foreach(JuegoProto juegoP in juegosProto.Juego)
            {
                if(juegoP != unJuego)
                {
                    agregaJuego = true;
                    juegoNuevo = unJuego;
                }
            }

            return await Task.FromResult(new BoolProto() { BoolProto_ = agregaJuego });
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
    }
}
