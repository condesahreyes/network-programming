using Grpc.Core;
using LogicaNegocio;
using ServidorAdministrativo.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public override async Task<JuegoProto> BuscarJuegoPortTitulo(Mensaje titulo, ServerCallContext context)
        {
            JuegosProto juegosProto = ObtenerJuegos();
            JuegoProto juegoARetornar = new JuegoProto();

            foreach(JuegoProto j in juegosProto.Juego)
            {
                if(j.Titulo == titulo) {
                    juegoARetornar = j;
                }
            }
            return await Task.FromResult(juegoARetornar);
        }

        public override async Task<bool> EsJuegoExistente(JuegoProto unJuego, ServerCallContext context)
        {
            bool juegoExistente = this.persistencia.juegos.Exists(x => x.Titulo.Equals(unJuego.Titulo));
            return await Task.FromResult(juegoExistente);

        } 

        public override async Task<bool> AgregarJuegos(JuegoProto unJuego, ServerCallContext context)
        {
            JuegoProto juegoNuevo = new JuegoProto();
            bool agregaJuego = false;
            JuegosProto juegosProto = ObtenerJuegos();

            foreach(JuegoProto juegoP in juegosProto.Juego)
            {
                if(juegoP != unJuego)
                {
                    agregaJuego = true;
                    juegoNuevo = unJuego;
                }
            }
            return await Task.FromResult(agregaJuego);
        }

        public override async Task<JuegosProto> ObtenerJuegos(MensajeVacio request, ServerCallContext context)
        {
            List<JuegoProto> juegosProto = new List<JuegoProto>();
            List<Juego> juegosDominio = this.persistencia.juegos;

            JuegosProto juegos = new JuegosProto();

            juegosDominio.ForEach(x => juegosProto.Add(new JuegoProto { Titulo = x.Titulo }));

            juegos.Juego.AddRange(juegosProto);

            return await Task.FromResult(juegos);
        }
    }
}
