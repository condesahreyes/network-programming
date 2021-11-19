using Protocolo.Transferencia_de_datos;
using System.Collections.Generic;
using LogicaNegocio;
using Protocolo;
using System.IO;
using System;
using System.Threading.Tasks;
using IServices;
using Servicios;

namespace Servidor
{
    public class Funcionalidad
    {
        private IUsuarioService usuarioService;
        private IJuegoService juegoService;
        Transferencia transferencia;

        public Funcionalidad(Transferencia transferencia)
        {
            this.transferencia = transferencia;
            this.usuarioService = new UsuarioService();
            this.juegoService = new JuegoService();
        }

        public async Task<Usuario> InicioSesionCliente(Usuario usuario, int largoMensajeARecibir)
        {
            usuario = await Controlador.RecibirUsuarioAsync(transferencia, largoMensajeARecibir);

            await EnviarRespuesta(usuario, usuario != null);

            Usuario usuarioCreado = await usuarioService.ObtenerUsuario(usuario);

            usuarioService.ActualizarAUsuarioActivo(usuario.NombreUsuario);

            return usuarioCreado;
        }

        public async Task CrearJuego(int largoMensajeARecibir)
        {
            Juego juego = await Controlador.PublicarJuegoAsync(transferencia, largoMensajeARecibir);

            string caratula = await RecibirArchivos();
            juego.Caratula = caratula;

            await EnviarRespuesta(juego, juegoService.AgregarJuego(juego));
        }

        public async Task EnviarListaJuegos()
        {
            List<Juego> juegos = juegoService.ObtenerJuegos();
            string juegosString =  Mapper.ListaDeJuegosAString(juegos);

            await EnviarMensaje(juegosString, Accion.ListaJuegos);
        }

        public async Task VerJuegosAdquiridos(int largoMensajeARecibir, Usuario usuario)
        {
            List<Juego> juegos = juegoService.JuegoUsuarios(usuario);
            string juegosString =  Mapper.ListaDeJuegosAString(juegos);
            await EnviarMensaje(juegosString, Accion.VerJuegosAdquiridos);
        }

        public async Task AdquirirJuego(int largoMensajeARecibir, Usuario usuario)
        {
            string tituloJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensajeARecibir);

            Juego juegoAdquirido = juegoService.AdquirirJuegoPorUsuario(tituloJuego, usuario);
            await EnviarRespuesta(juegoAdquirido, juegoAdquirido!=null);
        }

        public async Task EnviarDetalleDeUnJuego(int largoMensaje)
        {
            string  tituloJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensaje);
            Juego juego = juegoService.ObtenerJuegoPorTitulo(tituloJuego);
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

            bool fueAgregado = juegoService.AgregarCalificacion(calificacion);

            await EnviarRespuesta(null, fueAgregado);
        }

        public async Task BuscarJuegoPorTitulo(int largoMensajeARecibir)
        {
            string tituloJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensajeARecibir);
            Juego juego = juegoService.ObtenerJuegoPorTitulo(tituloJuego);
            string juegoEnString =  Mapper.JuegoAString(juego);

            await EnviarMensaje(juegoEnString, Accion.BuscarTitulo);
        }

        public async Task BuscarJuegoPorGenero(int largoMensajeARecibir)
        {
            string generoJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensajeARecibir);
            List<Juego> juego = juegoService.BuscarJuegoPorGenero(generoJuego);
            string juegoEnString = Mapper.ListaJuegosAString(juego);

            await EnviarMensaje(juegoEnString, Accion.BuscarGenero);
        }

        public async Task BuscarJuegoPorCalificacion(int largoMensajeARecibir)
        {
            string rankingString = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensajeARecibir);

            int ranking = Convert.ToInt32(rankingString);

            List<Juego> juegos = juegoService.BuscarJuegoPorCalificacion(ranking);

            string juegosString = Mapper.ListaJuegosAString(juegos);

            await EnviarMensaje(juegosString, Accion.BuscarCalificacion);
        }

        public async Task EliminarJuego(int largoMensaje)
        {
            string tituloJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensaje);
            juegoService.EliminarJuego(tituloJuego);
            await EnviarRespuesta(tituloJuego, juegoService.BuscarJuegoPortTitulo(tituloJuego) == null);
        }

        public async Task ModificarJuego(int largoMensaje)
        {
            string tituloJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensaje);
            Encabezado encabezado = await Controlador.RecibirEncabezadoAsync(transferencia);

            string juegoEnString = await Controlador.RecibirMensajeGenericoAsync(transferencia, encabezado.largoMensaje);

            Juego juego = Mapper.StringAJuego(juegoEnString);

            string caratula = await RecibirArchivos();
            juego.Caratula = caratula;

            bool fueEliminado = juegoService.EliminarJuego(tituloJuego);
            bool fueAgregadoElModificado = false;

            if (fueEliminado == false)
            {
                await EnviarRespuesta(null, fueAgregadoElModificado);
                return;
            }

            fueAgregadoElModificado = juegoService.AgregarJuego(juego);

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