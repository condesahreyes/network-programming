using Grpc.Core;
using LogicaNegocio;
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

        public override async Task<bool> AgregarCalificacion(CalificacionProto calificacion, ServerCallContext context)
        {
            bool retorno = true; 
            JuegoProto juegoProto = BuscarJuegoPortTitulo(juego);
            JuegosProto juegosProto = ObtenerJuegos();

            if (juegoProto == null)
                retorno =  false;

            juegoProto.calificaciones.Add(calificacion);

            juegoProto.Notas += calificacion.Nota;
            juegoProto.Ranking = Math.Abs((juegoProto.Notas) / juegoProto.calificaciones.Count);

            return await Task.FromResult(retorno);
        }

        public override async Task<bool> EliminarJuego(Mensaje titulo, ServerCallContext context)
        {
            bool retorno = false;
            JuegosProto juegosProto = ObtenerJuegos();

            foreach (JuegoProto juego in juegosProto)
                if (juego.Titulo == titulo)
                {
                    juegosProto.Remove(juego);
                    retorno = true;
                }
            return await Task.FromResult(retorno);
        }
        public override async Task<JuegosProto> BuscarJuegoPorCalificacion(MensajeInt ranking, ServerCallContext context)
        {
            JuegosProto juegosProto = ObtenerJuegos();
            JuegosProto juegosARetornar = new JuegosProto();

            foreach (JuegoProto j in juegosProto.Juego)
            {
                if (j.Ranking == ranking)
                {
                    juegosARetornar.Add(j);
                }
            }
            return await Task.FromResult(juegosARetornar);
        }

        public override async Task<JuegoProto> AdquirirJuegoPorUsuario(Mensaje juego, UsuarioProto usuario, ServerCallContext context)
        {
            JuegoProto juegoProto = BuscarJuegoPortTitulo(juego);

            UsuarioProto usuarioProto = ObtenerUsuario(usuario.Nombre);
            if(usuarioProto != null && juegoProto != null)
            {
                juegoProto.users.Add(usuarioProto);
            }

            return await Task.FromResult(juegoProto);
        }

        private Usuario ObtenerUsuario(string nombreUsuario)
        {
            return this.persistencia.usuarios.Find(x => x.NombreUsuario == nombreUsuario);
        }

        public override async Task<JuegoProto> BuscarJuegoPorGenero(Mensaje genero, ServerCallContext context)
        {
            JuegosProto juegosProto = ObtenerJuegos();
            JuegoProto juegoARetornar = new JuegoProto();

            foreach (JuegoProto j in juegosProto.Juego)
            {
                if (j.Genero == genero)
                {
                    juegoARetornar = j;
                }
            }
            return await Task.FromResult(juegoARetornar);
        }

        public override async Task<JuegosProto> JuegoUsuarios(UsuarioProto usuario, ServerCallContext context)
        {
            JuegosProto juegosProto = ObtenerJuegos();
            JuegosProto juegosARetornar = new JuegosProto();

           foreach(JuegoProto juego in juegosProto.juego)
            {
                foreach(UsuarioProto usuario in JuegoProto.usuarios)
                {
                    if(usuario.Nombre = usuario.Nombre)
                    {
                        juegosARetornar.Add(juego);
                    }
                }
            }
           return await Task.FromResult(juegosARetornar);

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
