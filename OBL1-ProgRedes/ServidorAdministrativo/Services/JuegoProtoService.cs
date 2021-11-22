using ServidorAdministrativo.Mappers;
using ServidorAdministrativo.Protos;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogicaNegocio;
using IRepositorio;
using Grpc.Core;

namespace ServidorAdministrativo.Services
{
    public class JuegoProtoService : ServicioJuego.ServicioJuegoBase
    {
        IRepositorioJuego repositorioJuego; 
        LogServices logServices;

        public JuegoProtoService(LogServices logServices, IRepositorioJuego repositorioJuego)
        {
            this.logServices = logServices;
            this.repositorioJuego = repositorioJuego;
        }
        
        public override async Task<ProtoBool> AgregarCalificacion(CalificacionProto calificacion, ServerCallContext context)
        {
            Calificacion calificacionNueva = MapperJuego.MapearProtoCalificaciones(calificacion);
            ProtoBool agregado = new ProtoBool { BoolProto = repositorioJuego.AgregarCalificacion(calificacionNueva) };

            if(agregado.BoolProto == true)
                this.logServices.SendMessages("juego " + calificacion.TituloJuegoo + " calificado");

            return await Task.FromResult(agregado);
        }

        public override async Task<ProtoBool> EliminarJuego(Mensaje nombreJuego, ServerCallContext context)
        {
            ProtoBool eliminado = new ProtoBool { BoolProto = repositorioJuego.EliminarJuego(nombreJuego.Mensaje_) };
            
            if (eliminado.BoolProto == true)
                this.logServices.SendMessages("juego " + nombreJuego.Mensaje_ + " eliminado");

            return await Task.FromResult(eliminado);
        }

        public override async Task<JuegosProto> BuscarJuegoPorCalificacion(MensajeInt ranking, ServerCallContext context)
        {
            List<Juego> juegos = repositorioJuego.ObtenerJuegos();
            JuegosProto juegosRetorno = new JuegosProto();
            foreach (var juego in juegos)
                if (ranking.Mensaje == juego.Ranking)
                    juegosRetorno.Juego.Add(MapperJuego.MapperJuegoProto(juego));

            this.logServices.SendMessages("juegos buscados por calificación " + ranking);

            return await Task.FromResult(juegosRetorno);
        }

        public override async Task<JuegoProto> AdquirirJuegoPorUsuario(JuegoPorUsuarioProto juegoUsuario, ServerCallContext context)
        {
            Juego juego = repositorioJuego.AsociarJuegoUsuario(juegoUsuario.TituloJuego, juegoUsuario.NombreUsuario);

            if (juego == null)
                return null;

            this.logServices.SendMessages("juego " + juegoUsuario.TituloJuego + 
                " adquirido por " + juegoUsuario.NombreUsuario);

            return await Task.FromResult(MapperJuego.MapperJuegoProto(juego));
        }

        public override async Task<JuegosProto> BuscarJuegoPorGenero(Mensaje genero, ServerCallContext context)
        {
            List<Juego> juegos = repositorioJuego.ObtenerJuegos();
            JuegosProto juegosRetorno = new JuegosProto();
            foreach (var juego in juegos)
                if (genero.Mensaje_ == juego.Genero)
                {
                    juegosRetorno.Juego.Add(MapperJuego.MapperJuegoProto(juego));
                }

            this.logServices.SendMessages("juegos buscado por genero " + genero.Mensaje_);

            return await Task.FromResult(juegosRetorno);
        }

        public override async Task<JuegosProto> JuegoUsuarios(Protos.ProtoUsuario usuario, ServerCallContext context)
        {
            List<Juego> juegos = JuegosUsuarios(MapperJuego.MapearProtoUsuario(usuario));
            JuegosProto juegosProto = new JuegosProto();

            foreach(var juego  in juegos)
            {
                juegosProto.Juego.Add(MapperJuego.MapperJuegoProto(juego));
            }

            this.logServices.SendMessages("juegos buscado por usuario " + usuario.Nombre);

            return await Task.FromResult(juegosProto);
        }

        public override async Task<JuegoProto> BuscarJuegoPortTitulo(Mensaje titulo, ServerCallContext context)
        {
            List<Juego> juegos = repositorioJuego.ObtenerJuegos();
            JuegoProto juegoRetorno = new JuegoProto();
            foreach (var juego in juegos)
                if (titulo.Mensaje_ == juego.Titulo)
                {
                    juegoRetorno = MapperJuego.MapperJuegoProto(juego);
                }

            this.logServices.SendMessages("juegos buscado por titulo " + titulo.Mensaje_);

            return await Task.FromResult(juegoRetorno);
        }

        public override async Task<ProtoBool> EsJuegoExistente(JuegoProto unJuego, ServerCallContext context)
        {
            bool juegoExistente = repositorioJuego.ObtenerJuegos().Exists(x => x.Titulo.Equals(unJuego.Titulo));
            this.logServices.SendMessages("Consulta juego " + unJuego.Titulo + " existe");

            return await Task.FromResult(new ProtoBool() { BoolProto = juegoExistente });
        } 
        
        public override async Task<ProtoBool> AgregarJuegos(JuegoProto unJuego, ServerCallContext context)
        {
            Juego nuevoJuego = MapperJuego.MapperProtoJuego(unJuego);
            bool juegoAgregado = repositorioJuego.AgregarJuego(nuevoJuego);

            if(juegoAgregado)
                this.logServices.SendMessages("Alta juego " + unJuego.Titulo);

            return await Task.FromResult(new ProtoBool() { BoolProto = juegoAgregado });
        }

        public override async Task<JuegosProto> ObtenerJuegos(Protos.MensajeVacio request, ServerCallContext context)
        {
            List<Juego> juegosDominio = repositorioJuego.ObtenerJuegos();
            JuegosProto juegos = new JuegosProto();

            juegosDominio.ForEach(x => juegos.Juego.Add(MapperJuego.MapperJuegoProto(x)));

            this.logServices.SendMessages("Consulta de juegos");

            return await Task.FromResult(juegos);
        }

        public override async Task<ProtoBool> DesasociarJuegoUsuario(JuegoPorUsuarioProto request, ServerCallContext context)
        {
            bool desasociado = repositorioJuego.DesasociarJuegoUsuario(request.TituloJuego, request.NombreUsuario);
            
            if(desasociado)
            this.logServices.SendMessages("Juego " + request.TituloJuego +  "desasociado al usuario " + request.NombreUsuario);

            return await Task.FromResult(new ProtoBool { BoolProto = desasociado });
        }

        public override async Task<JuegoProto> ModificarJuego(JuegoModificarProto request, ServerCallContext context)
        {
            Juego juego = repositorioJuego.ModificarJuego(request.TituloJuego, request.JuegoModificado.Caratula,
                request.JuegoModificado.Sinposis, request.JuegoModificado.Genero);

            if(juego!=null)
                this.logServices.SendMessages("Juego " + request.TituloJuego + " modificado");

            return await Task.FromResult(MapperJuego.MapperJuegoProto(juego));
        }

        private List<Juego> JuegosUsuarios(Usuario usuario)
        {
            List<Juego> juegosPersistidos = repositorioJuego.ObtenerJuegos();
            List<Juego> retorno = new List<Juego>();
            foreach(var juego in juegosPersistidos)
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

    }
}
