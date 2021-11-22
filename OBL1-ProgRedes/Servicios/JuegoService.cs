using Microsoft.Extensions.Configuration;
using ServidorAdministrativo.Protos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Servicios.Mappers;
using Grpc.Net.Client;
using LogicaNegocio;
using IServices;
using System.IO;

namespace Servicios
{
    public class JuegoService : IJuegoService
    {
        IConfiguration configuracion = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AppSettings.json", optional: false).Build();

        ServicioJuego.ServicioJuegoClient juegoProtoService;

        public JuegoService()
        {
            GrpcChannel canal = GrpcChannel.ForAddress(configuracion["canalGrpc"]);
            this.juegoProtoService = new ServicioJuego.ServicioJuegoClient(canal);
        }

        public async Task<bool> EsJuegoExistente(Juego unJuego)
        {
            ProtoBool juegoProto = await juegoProtoService.EsJuegoExistenteAsync(
                MapperJuego.MapperJuegoProto(unJuego));
            return juegoProto.BoolProto;
        }

        public async Task<List<Juego>> ObtenerJuegos()
        {
            JuegosProto juegosProto = await juegoProtoService.ObtenerJuegosAsync(new MensajeVacio() { });
            return (MapperJuego.MapearJuegosProto(juegosProto));
        }

        public async Task<bool> AgregarJuego(Juego juego)
        {
            ProtoBool agregado = await juegoProtoService.AgregarJuegosAsync(
                MapperJuego.MapperJuegoProto(juego));

            return agregado.BoolProto;
        }

        public async Task<bool> AgregarCalificacion(Calificacion calificacion)
        {
            ProtoBool agregada = await juegoProtoService.
                AgregarCalificacionAsync(MapperJuego.MapperCalificacionProto(calificacion));
            return agregada.BoolProto;
        }

        public async Task<Juego> AdquirirJuegoPorUsuario(string juego, Usuario usuario)
        {
            JuegoProto juegoProto = await juegoProtoService.AdquirirJuegoPorUsuarioAsync
                (new JuegoPorUsuarioProto { TituloJuego = juego, NombreUsuario = usuario.NombreUsuario });
            
            return MapperJuego.MapperProtoJuego(juegoProto); 
        }

        public async Task<List<Juego>> JuegoUsuarios(Usuario usuario)
        {
            JuegosProto jugoRequest = await juegoProtoService.JuegoUsuariosAsync(MapperJuego.MapearUsuarioProto(usuario));
            return MapperJuego.MapperProtoJuegos(jugoRequest);
        }

        public async Task<Juego> BuscarJuegoPortTitulo(string unTitulo)
        {
            JuegoProto juegoProto = await juegoProtoService.
                BuscarJuegoPortTituloAsync(new Mensaje { Mensaje_ = unTitulo });
            return MapperJuego.MapperProtoJuego(juegoProto);
        }

        public async Task<List<Juego>> BuscarJuegoPorGenero(string unGenero)
        {
            JuegosProto juegoProto = await juegoProtoService.
                BuscarJuegoPorGeneroAsync(new Mensaje { Mensaje_ = unGenero });
            return MapperJuego.MapperProtoJuegos(juegoProto);
        }

        public async  Task<Juego> ObtenerJuegoPorTitulo(string tituloJuego)
        {
           JuegoProto juegoProto = await juegoProtoService.
                BuscarJuegoPortTituloAsync(new Mensaje { Mensaje_ = tituloJuego});
            return MapperJuego.MapperProtoJuego(juegoProto);
        }

        public async Task<List<Juego>> BuscarJuegoPorCalificacion(int ranking)
        {
            JuegosProto juegoProto = await juegoProtoService.
                BuscarJuegoPorCalificacionAsync(new MensajeInt { Mensaje = ranking});
            return MapperJuego.MapperProtoJuegos(juegoProto);
        }

        public async Task<bool> EliminarJuego(string tituloJuego)
        {
            ProtoBool protoBool = await juegoProtoService.
                EliminarJuegoAsync(new Mensaje { Mensaje_ = tituloJuego});
            return protoBool.BoolProto;
        }

        public async Task<bool> DesasociarJuegoAUsuario(string juego, Usuario usuario)
        {
            JuegoProto juegoGuardado = await juegoProtoService.
                BuscarJuegoPortTituloAsync(new Mensaje { Mensaje_ = juego });
           
            if(juegoGuardado != null)
            {
                JuegoPorUsuarioProto juegoUsuario = new JuegoPorUsuarioProto {
                    NombreUsuario = usuario.NombreUsuario, TituloJuego = juego };
                ProtoBool desasociado = await juegoProtoService.DesasociarJuegoUsuarioAsync(juegoUsuario);
                return desasociado.BoolProto;
            }

            return false;
        }

        public async Task<Juego> ModificarJuego(string tituloJuego, Juego juegoModificado)
        {
            JuegoProto juegoGuardado = await juegoProtoService.
                BuscarJuegoPortTituloAsync(new Mensaje { Mensaje_ = tituloJuego });
            
            if (juegoGuardado.Titulo == "" && juegoGuardado.Titulo != tituloJuego)
                return null;
            JuegoModificarProto juegoProtoModificar = new JuegoModificarProto
            {
                JuegoModificado = MapperJuego.MapperJuegoProto(juegoModificado),
                TituloJuego = tituloJuego
            };

            JuegoProto modificado = await juegoProtoService.ModificarJuegoAsync(juegoProtoModificar);
            return MapperJuego.MapperProtoJuego(modificado);
        }
    }
}
