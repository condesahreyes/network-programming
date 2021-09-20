using System.Collections.Generic;
using Cliente.Constantes;
using LogicaNegocio;
using Protocolo;
using System;

namespace Cliente
{
    public class Funcionalidad
    {
        private static Funcionalidad _instancia;
        private Conexion conexionCliente;

        public  Funcionalidad()
        {
            this.conexionCliente = new Conexion();
        }

        public static Funcionalidad ObtenerInstancia()
        {
            return _instancia == null ? new Funcionalidad() : _instancia;
        }

        public Usuario InicioSesion()
        {
            Usuario usuario = Usuario.CrearUsuario();
            string nombreUsuario = usuario.NombreUsuario;

            EnvioYRespuesta(nombreUsuario, Accion.Login, Mensaje.InicioSesion, Mensaje.ErrorGenerico);

            return usuario;
        }

        public void DesconectarUsuario(Usuario usuario)
        {
            conexionCliente.DesconectarUsuario(usuario);
        }

        public void DetalleDeUnJuego()
        {
            List<string> juegos = conexionCliente.RecibirListaDeJuegos();

            if (juegos.Count == 0)
            {
                Mensaje.NoExistenJuegos();
                return;
            }

            string titulo = SeleccionarUnTituloDeJuego(juegos);
            Juego juego = conexionCliente.RecibirUnJuegoPorTitulo(titulo);

            Mensaje.MostrarJuego(juego);
        }

        private string SeleccionarUnTituloDeJuego(List<string> juegos)
        {
            Mensaje.MostrarJuegos(juegos);
            int juegoSeleccionado = Metodo.ObtenerOpcion(Mensaje.seleccioneJuego, 0, juegos.Count - 1);

            return juegos[juegoSeleccionado];
        }

        public void PublicarJuego()
        {
            Juego unJuego = Juego.CrearJuego();

            string juegoEnString = Mapper.JuegoAString(unJuego);

            EnvioYRespuesta(juegoEnString, Accion.PublicarJuego, Mensaje.JuegoCreado, Mensaje.JuegoExistente);
        }

        public void CalificarUnJuego(Usuario usuario)
        {
            List<string> juegos = conexionCliente.RecibirListaDeJuegos();

            if (juegos.Count == 0)
            {
                Mensaje.NoExistenJuegos();
                return;
            }

            string titulo = SeleccionarUnTituloDeJuego(juegos);

            Calificacion unaCalificacion = Calificacion.CrearCalificacion(usuario.NombreUsuario, titulo);

            string calificacionEnString = Mapper.CalificacionAString(unaCalificacion);

            EnvioYRespuesta(calificacionEnString, Accion.PublicarCalificacion,
                Mensaje.CalificacionCreada, Mensaje.ErrorGenerico);
        }

        private void EnvioYRespuesta(string mensaje, string accion, Action RespuestaOk, Action RespuestaError)
        {
            conexionCliente.EnvioEncabezado(mensaje.Length, accion);

            conexionCliente.EnvioDeMensaje(mensaje);

            string respuestaServidor = conexionCliente.EsperarPorRespuesta();

            if (respuestaServidor == Constante.MensajeOk)
                RespuestaOk();
            else
                RespuestaError();
        }

    }
}
