using Protocolo.Transferencia_de_datos;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogicaNegocio;
using IServices;
using Servicios;
using Protocolo;
using System.IO;
using System;

namespace Servidor
{
    public class FuncionalidadCliente
    {
        private IUsuarioService usuarioService;
        private IJuegoService juegoService;
        Transferencia transferencia;

        public FuncionalidadCliente(Transferencia transferencia)
        {
            this.transferencia = transferencia;
            this.usuarioService = new UsuarioService();
            this.juegoService = new JuegoService();
        }

        public async Task<Usuario> InicioSesionClienteAsync(Usuario usuario, int largoMensajeARecibir)
        {
            usuario = await Controlador.RecibirUsuarioAsync(transferencia, largoMensajeARecibir);

            await EnviarRespuestaAsync(usuario, usuario != null);

            Usuario usuarioCreado = await usuarioService.ObtenerUsuarioAsync(usuario);

            await usuarioService.ActualizarAUsuarioActivoAsync(usuario.NombreUsuario);

            return usuarioCreado;
        }

        public async Task CrearJuegoAysnc(int largoMensajeARecibir)
        {
            Juego juego = await Controlador.PublicarJuegoAsync(transferencia, largoMensajeARecibir);

            string caratula = await RecibirArchivosAsync();
            juego.Caratula = caratula;

            await EnviarRespuestaAsync(juego, await juegoService.AgregarJuegoAsync(juego));
        }

        public async Task EnviarListaJuegosAysnc()
        {
            List<Juego> juegos = await juegoService.ObtenerJuegosAsync();
            string juegosString =  Mapper.ListaDeJuegosAString(juegos);

            await EnviarMensajeAsync(juegosString, Accion.ListaJuegos);
        }

        public async Task VerJuegosAdquiridosAysnc(int largoMensajeARecibir, Usuario usuario)
        {
            List<Juego> juegos = await juegoService.JuegoUsuariosAsync(usuario);
            string juegosString =  Mapper.ListaDeJuegosAString(juegos);
            await EnviarMensajeAsync(juegosString, Accion.VerJuegosAdquiridos);
        }

        public async Task AdquirirJuegoAsync(int largoMensajeARecibir, Usuario usuario)
        {
            string tituloJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensajeARecibir);

            Juego juegoAdquirido = await juegoService.AdquirirJuegoPorUsuarioAsync(tituloJuego, usuario);
            await EnviarRespuestaAsync(juegoAdquirido, juegoAdquirido!=null);
        }

        public async Task EnviarDetalleDeUnJuegoAsync(int largoMensaje)
        {
            string  tituloJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensaje);
            Juego juego = await juegoService.ObtenerJuegoPorTituloAsync(tituloJuego);
            if(juego != null)
            {
                string juegoEnString = Mapper.JuegoAString(juego);

                await EnviarMensajeAsync(juegoEnString, Accion.EnviarDetalleJuego);
                await ControladorDeArchivos.EnviarArchivoAsync(Directory.GetCurrentDirectory() + @"\" + 
                    juego.Caratula, transferencia);
            }
            else
            {
                await EnviarRespuestaAsync(juego, false);
            }
        }

        public async Task CrearCalificacionAsync(int largoMensajeARecibir)
        {
            Calificacion calificacion = await Controlador.PublicarCalificacionAsync(transferencia, largoMensajeARecibir);

            bool fueAgregado =  await juegoService.AgregarCalificacionAsync(calificacion);

            await EnviarRespuestaAsync(null, fueAgregado);
        }

        public async Task BuscarJuegoPorTituloAsync(int largoMensajeARecibir)
        {
            string tituloJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensajeARecibir);
            Juego juego = await  juegoService.ObtenerJuegoPorTituloAsync(tituloJuego);
            string juegoEnString =  Mapper.JuegoAString(juego);

            await EnviarMensajeAsync(juegoEnString, Accion.BuscarTitulo);
        }

        public async Task BuscarJuegoPorGeneroAsync(int largoMensajeARecibir)
        {
            string generoJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensajeARecibir);
            List<Juego> juego = await juegoService .BuscarJuegoPorGeneroAsync(generoJuego);
            string juegoEnString = Mapper.ListaJuegosAString(juego);

            await EnviarMensajeAsync(juegoEnString, Accion.BuscarGenero);
        }

        public async Task BuscarJuegoPorCalificacionAsync(int largoMensajeARecibir)
        {
            string rankingString = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensajeARecibir);

            int ranking = Convert.ToInt32(rankingString);

            List<Juego> juegos = await  juegoService.BuscarJuegoPorCalificacionAsync(ranking);

            string juegosString = Mapper.ListaJuegosAString(juegos);

            await EnviarMensajeAsync(juegosString, Accion.BuscarCalificacion);
        }

        public async Task EliminarJuegoAsync(int largoMensaje)
        {
            string tituloJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensaje);
            await juegoService.EliminarJuegoAsync(tituloJuego);
            await EnviarRespuestaAsync(tituloJuego, juegoService.BuscarJuegoPortTituloAsync(tituloJuego) == null);
        }

        public async Task ModificarJuegoAsync(int largoMensaje)
        {
            string tituloJuego = await Controlador.RecibirMensajeGenericoAsync(transferencia, largoMensaje);
            Encabezado encabezado = await Controlador.RecibirEncabezadoAsync(transferencia);

            string juegoEnString = await Controlador.RecibirMensajeGenericoAsync(transferencia, encabezado.largoMensaje);

            Juego juego = Mapper.StringAJuego(juegoEnString);

            string caratula = await RecibirArchivosAsync();
            juego.Caratula = caratula;

            bool fueEliminado = await  juegoService.EliminarJuegoAsync(tituloJuego);
            bool fueAgregadoElModificado = false;

            if (fueEliminado == false)
            {
                await EnviarRespuestaAsync(null, fueAgregadoElModificado);
                return;
            }

            fueAgregadoElModificado = await juegoService.AgregarJuegoAsync(juego);

            await EnviarRespuestaAsync(juego, fueAgregadoElModificado);
        }

        public async Task<string> RecibirArchivosAsync()
        {
            Encabezado encabezado = await Controlador.RecibirEncabezadoAsync(transferencia);

            string nombreArchivo = await Controlador.RecibirMensajeGenericoAsync(transferencia, encabezado.largoMensaje);
            await ControladorDeArchivos.RecibirArchivosAsync(transferencia, nombreArchivo);

            return nombreArchivo;
        }

        private async Task EnviarRespuestaAsync(object obj, bool ok)
        {
            if (obj == null && !ok)
                await Controlador.EnviarMensajeClienteObjetoEliminadoAsync(transferencia);
            else if (ok)
                await Controlador.EnviarMensajeClienteOkAsync(transferencia);
            else
                await Controlador.EnviarMensajeClienteErrorAsync(transferencia);
        }

        private async Task EnviarMensajeAsync(string mensaje, string accion)
        {
            Encabezado encabezado = new Encabezado(mensaje.Length, accion);

            await Controlador.EnviarEncabezadoAsync(transferencia, encabezado);
            await Controlador.EnviarDatosAsync(transferencia, mensaje);
        }

    }
}