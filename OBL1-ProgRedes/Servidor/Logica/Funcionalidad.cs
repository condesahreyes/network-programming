﻿using Servidor.FuncionalidadesPorEntidad;
using Servidor.FuncionalidadesEntidades;
using Protocolo.Transferencia_de_datos;
using System.Collections.Generic;
using LogicaNegocio;
using Protocolo;
using System.IO;
using System;
using System.Threading.Tasks;

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

        public async Task<Usuario> InicioSesionCliente(Usuario usuario, int largoMensajeARecibir)
        {
            usuario = await Controlador.RecibirUsuarioAsync(transferencia, largoMensajeARecibir);

            await EnviarRespuesta(usuario, usuario != null);

            Usuario usuarioCreado = funcionesUsuario.ObtenerUsuario(usuario);

            funcionesUsuario.ActualizarAUsuarioActivo(usuario.NombreUsuario);

            return usuarioCreado;
        }

        public async Task CrearJuego(int largoMensajeARecibir)
        {
            Juego juego = await Controlador.PublicarJuegoAsync(transferencia, largoMensajeARecibir);

            string caratula = await RecibirArchivos();
            juego.Caratula = caratula;

            await EnviarRespuesta(juego, funcionesJuego.AgregarJuego(juego));
        }

        public async Task EnviarListaJuegos()
        {
            List<Juego> juegos =  funcionesJuego.ObtenerJuegos();
            string juegosString =  Mapper.ListaDeJuegosAString(juegos);

            await EnviarMensaje(juegosString, Accion.ListaJuegos);
        }

        public async Task VerJuegosAdquiridos(int largoMensajeARecibir, Usuario usuario)
        {
            List<Juego> juegos = funcionesJuego.JuegoUsuarios(usuario);
            string juegosString =  Mapper.ListaDeJuegosAString(juegos);
            await EnviarMensaje(juegosString, Accion.VerJuegosAdquiridos);
        }

        public async Task AdquirirJuego(int largoMensajeARecibir, Usuario usuario)
        {
            string tituloJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensajeARecibir);

            Juego juegoAdquirido = funcionesJuego.AdquirirJuegoPorUsuario(tituloJuego, usuario);
            await EnviarRespuesta(juegoAdquirido, juegoAdquirido!=null);
        }

        public async Task EnviarDetalleDeUnJuego(int largoMensaje)
        {
            string  tituloJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensaje);
            Juego juego = funcionesJuego.ObtenerJuegoPorTitulo(tituloJuego);
            if(juego != null)
            {
                string juegoEnString = Mapper.JuegoAString(juego);

                await EnviarMensaje(juegoEnString, Accion.EnviarDetalleJuego);
                await ControladorDeArchivos.EnviarArchivoAsync(Directory.GetCurrentDirectory() + @"\" + 
                    juego.Caratula, transferencia);
            }
            else
            {
                await EnviarRespuesta(juego, false);
            }
        }

        public async Task CrearCalificacion(int largoMensajeARecibir)
        {
            Calificacion calificacion = await Controlador.PublicarCalificacionAsync(transferencia, largoMensajeARecibir);

            bool fueAgregado =  funcionesJuego.AgregarCalificacion(calificacion);

            await EnviarRespuesta(null, fueAgregado);
        }

        public async Task BuscarJuegoPorTitulo(int largoMensajeARecibir)
        {
            string tituloJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensajeARecibir);
            Juego juego = funcionesJuego.ObtenerJuegoPorTitulo(tituloJuego);
            string juegoEnString =  Mapper.JuegoAString(juego);

            await EnviarMensaje(juegoEnString, Accion.BuscarTitulo);
        }

        public async Task BuscarJuegoPorGenero(int largoMensajeARecibir)
        {
            string generoJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensajeARecibir);
            List<Juego> juego =  funcionesJuego.BuscarJuegoPorGenero(generoJuego);
            string juegoEnString = Mapper.ListaJuegosAString(juego);

            await EnviarMensaje(juegoEnString, Accion.BuscarGenero);
        }

        public async Task BuscarJuegoPorCalificacion(int largoMensajeARecibir)
        {
            string rankingString = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensajeARecibir);

            int ranking = Convert.ToInt32(rankingString);

            List<Juego> juegos = funcionesJuego.BuscarJuegoPorCalificacion(ranking);

            string juegosString = Mapper.ListaJuegosAString(juegos);

            await EnviarMensaje(juegosString, Accion.BuscarCalificacion);
        }

        public async Task EliminarJuego(int largoMensaje)
        {
            string tituloJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensaje);
            funcionesJuego.EliminarJuego(tituloJuego);
            await EnviarRespuesta(tituloJuego, funcionesJuego.BuscarJuegoPortTitulo(tituloJuego) == null);
        }

        public async Task ModificarJuego(int largoMensaje)
        {
            string tituloJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensaje);
            Encabezado encabezado = await Controlador.RecibirEncabezadoAsync(transferencia);

            string juegoEnString = await Controlador.RecibirMensajeGenericoAsync(transferencia, encabezado.largoMensaje);

            Juego juego = Mapper.StringAJuego(juegoEnString);

            string caratula = await RecibirArchivos();
            juego.Caratula = caratula;

            bool fueEliminado = funcionesJuego.EliminarJuego(tituloJuego);
            bool fueAgregadoElModificado = false;

            if (fueEliminado == false)
            {
                await EnviarRespuesta(null, fueAgregadoElModificado);
                return;
            }

            fueAgregadoElModificado = funcionesJuego.AgregarJuego(juego);

            await EnviarRespuesta(juego, fueAgregadoElModificado);
        }

        public async Task<string> RecibirArchivos()
        {
            Encabezado encabezado = await Controlador.RecibirEncabezadoAsync(transferencia);

            string nombreArchivo = await Controlador.RecibirMensajeGenericoAsync(transferencia, encabezado.largoMensaje);
            await ControladorDeArchivos.RecibirArchivosAsync(transferencia, nombreArchivo);

            return nombreArchivo;
        }

        private async Task EnviarRespuesta(object obj, bool ok)
        {
            if (obj == null && !ok)
                await Controlador.EnviarMensajeClienteObjetoEliminadoAsync(transferencia);
            else if (ok)
                await Controlador.EnviarMensajeClienteOkAsync(transferencia);
            else
                await Controlador.EnviarMensajeClienteErrorAsync(transferencia);
        }

        private async Task EnviarMensaje(string mensaje, string accion)
        {
            Encabezado encabezado = new Encabezado(mensaje.Length, accion);

            await Controlador.EnviarEncabezadoAsync(transferencia, encabezado);
            await Controlador.EnviarDatos(transferencia, mensaje);
        }
    }
}