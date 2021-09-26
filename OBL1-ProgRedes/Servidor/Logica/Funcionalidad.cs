using Servidor.FuncionalidadesPorEntidad;
using Servidor.FuncionalidadesEntidades;
using Protocolo.Transferencia_de_datos;
using System.Collections.Generic;
using LogicaNegocio;
using Protocolo;
using System.IO;
using System;

namespace Servidor
{
    public class Funcionalidad
    {
        LogicaUsuario funcionesUsuario;
        LogicaJuego funcionesJuego;
        Transferencia transferencia;

        public Funcionalidad(Transferencia transferencia)
        {
            this.transferencia = transferencia;
            this.funcionesUsuario = new LogicaUsuario();
            this.funcionesJuego = new LogicaJuego();
        }

        internal Usuario InicioSesionCliente(Usuario usuario, int largoMensajeARecibir)
        {
            usuario = Controlador.RecibirUsuario(transferencia, largoMensajeARecibir);

            EnviarRespuesta(usuario != null);

            return funcionesUsuario.ObtenerUsuario(usuario);
        }

        public void CrearJuego(int largoMensajeARecibir)
        {
            Juego juego = Controlador.PublicarJuego(transferencia, largoMensajeARecibir);

            string caratula = RecibirArchivos();
            juego.Caratula = caratula;

            EnviarRespuesta(funcionesJuego.AgregarJuego(juego));
        }

        internal void EnviarListaJuegos()
        {
            List<Juego> juegos = funcionesJuego.ObtenerJuegos();
            string juegosString = Mapper.ListaDeJuegosAString(juegos);

            EnviarMensaje(juegosString, Accion.ListaJuegos);
        }

        internal void EnviarDetalleDeUnJuego(int largoMensaje)
        {
            string tituloJuego = Controlador.RecibirMensajeGenerico(transferencia, largoMensaje);
            Juego juego = funcionesJuego.ObtenerJuegoPorTitulo(tituloJuego);
            string juegoEnString = Mapper.JuegoAString(juego);

            EnviarMensaje(juegoEnString, Accion.EnviarDetalleJuego);
            ControladorDeArchivos.EnviarArchivo(Directory.GetCurrentDirectory() + @"\" +juego.Caratula, transferencia);
        }

        public void CrearCalificacion(int largoMensajeARecibir)
        {
            Calificacion calificacion = Controlador.PublicarCalificacion(transferencia, largoMensajeARecibir);

            funcionesJuego.AgregarCalificacion(calificacion);

            EnviarRespuesta(calificacion != null);
        }

        public void BuscarJuegoPorTitulo(int largoMensajeARecibir)
        {
            string tituloJuego = Controlador.RecibirMensajeGenerico(transferencia, largoMensajeARecibir);
            Juego juego = funcionesJuego.ObtenerJuegoPorTitulo(tituloJuego);
            string juegoEnString = Mapper.JuegoAString(juego);

            EnviarMensaje(juegoEnString, Accion.BuscarTitulo);
        }

        public void BuscarJuegoPorGenero(int largoMensajeARecibir)
        {
            string generoJuego = Controlador.RecibirMensajeGenerico(transferencia, largoMensajeARecibir);
            List<Juego> juego = funcionesJuego.BuscarJuegoPorGenero(generoJuego);
            string juegoEnString = Mapper.ListaJuegosAString(juego);

            EnviarMensaje(juegoEnString, Accion.BuscarGenero);

        }

        public void BuscarJuegoPorCalificacion(int largoMensajeARecibir)
        {
            string rankingString = Controlador.RecibirMensajeGenerico(transferencia, largoMensajeARecibir);

            int ranking = Convert.ToInt32(rankingString);

            List<Juego> juegos = funcionesJuego.BuscarJuegoPorCalificacion(ranking);

            string juegosString = Mapper.ListaJuegosAString(juegos);

            EnviarMensaje(juegosString, Accion.BuscarCalificacion);
        }

        internal void EliminarJuego(int largoMensaje)
        {
            string tituloJuego = Controlador.RecibirMensajeGenerico(transferencia, largoMensaje);
            funcionesJuego.EliminarJuego(tituloJuego);
            EnviarRespuesta(funcionesJuego.BuscarJuegoPortTitulo(tituloJuego) == null);
        }

        private void EnviarRespuesta(bool respuestaOk)
        {
            if (respuestaOk)
                Controlador.EnviarMensajeClienteOk(transferencia);
            else
                Controlador.EnviarMensajeClienteError(transferencia);
        }

        private void EnviarMensaje(string mensaje, string accion)
        {
            Encabezado encabezado = new Encabezado(mensaje.Length, accion);

            Controlador.EnviarEncabezado(transferencia, encabezado);
            Controlador.EnviarDatos(transferencia, mensaje);
        }

        public void ModificarJuego(int largoMensaje)
        {
            string tituloJuego = Controlador.RecibirMensajeGenerico(transferencia, largoMensaje);
            Encabezado encabezado = Controlador.RecibirEncabezado(transferencia);

            string juegoEnString = Controlador.RecibirMensajeGenerico(transferencia, encabezado.largoMensaje);

            Juego juego = Mapper.StringAJuego(juegoEnString);

            string caratula = RecibirArchivos();
            juego.Caratula = caratula;

            funcionesJuego.EliminarJuego(tituloJuego);
            funcionesJuego.AgregarJuego(juego);

            EnviarRespuesta(juego!=null);
        }

        public string RecibirArchivos()
        {
            Encabezado encabezado = Controlador.RecibirEncabezado(transferencia);

            string nombreArchivo = Controlador.RecibirMensajeGenerico(transferencia, encabezado.largoMensaje);
            ControladorDeArchivos.RecibirArchivos(transferencia, nombreArchivo);

            return nombreArchivo;
        }

    }
}
