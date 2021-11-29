using System.Collections.Generic;
using System.Threading.Tasks;
using Cliente.Constantes;
using LogicaNegocio;
using Protocolo;
using System;

namespace Cliente
{
    public class Funcionalidad
    {
        private static Funcionalidad _instancia;
        private static Conexion conexionCliente;

        public  Funcionalidad() { }

        public static async Task InstanciarConexionAsync()
        {
            conexionCliente = new Conexion();
            await conexionCliente.InstanciarTransferenciaAsync();
        }

        public static async Task<Funcionalidad> ObtenerInstanciaAsync()
        {
            if (_instancia == null)
            {
                _instancia = new Funcionalidad();
                await InstanciarConexionAsync();
            }
  
            return _instancia;
        }

        public async Task<Usuario> InicioSesionAsync()
        {
            Usuario usuario = Usuario.CrearUsuario();
            string nombreUsuario = usuario.NombreUsuario;

            await EnvioYRespuestaAsync(nombreUsuario, Accion.Login, Mensaje.InicioSesion, 
                Mensaje.ErrorGenerico, Mensaje.ErrorGenerico);

            return usuario;
        }

        public void DesconectarUsuario(Usuario usuario)
        {
            conexionCliente.DesconectarUsuario(usuario);
        }

        public async Task PublicarJuegoAsync()
        {
            Juego unJuego = Juego.CrearJuego();

            string juegoEnString = Mapper.JuegoAString(unJuego);

            string caratula = unJuego.Caratula;

            await EnvioMensajeConPrevioEncabezadoAsync(juegoEnString, Accion.PublicarJuego);
            await conexionCliente.EnvioDeArchivoAsync(caratula);
            await RecibirRespuestasAsync(Mensaje.JuegoCreado, Mensaje.JuegoExistente, Mensaje.JuegoEliminado);
        }

        public async Task BuscarJuegoAsync()
        {
            int opcion = Metodo.ObtenerOpcion(Mensaje.buscarJuegoPorFiltro, 0, 2);
            string filtro = "";
            string accion = "";

            if (opcion == 0)
            {
                Mensaje.BuscarJuegoPorTitulo();
                filtro = Console.ReadLine();
                accion = Accion.BuscarTitulo;
            }
            else if (opcion == 1)
            {
                Mensaje.BuscarJuegoPorGenero();
                filtro = Console.ReadLine();
                accion = Accion.BuscarGenero;
            }
            else if (opcion == 2)
            {
                filtro = Metodo.ObtenerOpcion(Mensaje.ranking, 1, 5).ToString();
                accion = Accion.BuscarCalificacion;
            }

            List<Juego> juegos = await BuscarJuegoPorFiltroAsync(filtro, accion);
            Mensaje.MostrarObjetoJuego(juegos);
        }

        public async Task DetalleDeUnJuegoAsync()
        {
            string titulo = await DevolverTituloJuegoSeleccionadoAsync();

            if (titulo == "")
            {
                Mensaje.NoExistenJuegos();
                return;
            }
            try
            {
                Juego juego = await conexionCliente.RecibirUnJuegoPorTituloAsync(titulo);

                await conexionCliente.RecibirArchivosAsync(juego.Caratula);
                Mensaje.MostrarJuego(juego);
            }
            catch(Exception)
            {
                Mensaje.JuegoEliminado();
            }
        }

        public async Task CalificarUnJuegoAsync(Usuario usuario)
        {
            string titulo = await DevolverTituloJuegoSeleccionadoAsync();

            if (titulo == "")
            {
                Mensaje.NoExistenJuegos();
                return;
            }

            Calificacion unaCalificacion = Calificacion.CrearCalificacion(usuario.NombreUsuario, titulo);

            string calificacionEnString = Mapper.CalificacionAString(unaCalificacion);

            await EnvioYRespuestaAsync(calificacionEnString, Accion.PublicarCalificacion,
                Mensaje.CalificacionCreada, Mensaje.ErrorGenerico, Mensaje.JuegoEliminado);
        }

        public async Task AdquirirJuegoAsync(Usuario usuario)
        {
           string tituloJuego = await DevolverTituloJuegoSeleccionadoAsync();
           await EnvioYRespuestaAsync(tituloJuego, Accion.AdquirirJuego, Mensaje.JuegoAdquirido, Mensaje.ErrorAdquirirJuego, Mensaje.JuegoEliminado);
        }

        public async Task ListaJuegosAdquiridosAsync(Usuario usuario)
        {
            await conexionCliente.EnvioEncabezadoAsync(0, Accion.VerJuegosAdquiridos);
            List<string> juegosAdquiridos = await conexionCliente.RecibirListaDeJuegosAdquiridosAsync();
            Mensaje.MostrarJuegos(juegosAdquiridos, Mensaje.noHayJuegosRegistrados, Mensaje.listadoJuegoAdquiridos);
        }

        public async Task BajaModificacionJuegoAsync()
        {
            string titulo = await DevolverTituloJuegoSeleccionadoAsync();

            if (titulo == "")
            {
                Mensaje.NoExistenJuegos();
                return;
            }

            int opcion = Metodo.ObtenerOpcion(Mensaje.bajaModificacion, 0, 1);

            if (opcion == 0)
                await EnvioYRespuestaAsync(titulo, Accion.EliminarJuego, Mensaje.JuegoEliminadoOk, 
                    Mensaje.JuegoEliminadoError, Mensaje.JuegoEliminado);
            else if (opcion == 1)
                await ModificarUnJuegoAsync(titulo);
        }

        private async Task ModificarUnJuegoAsync(string titulo)
        {
            Juego juegoModificado = Juego.ModificarJuego();
            string juegoEnString = Mapper.JuegoAString(juegoModificado);

            await conexionCliente.EnvioEncabezadoAsync(titulo.Length, Accion.ModificarJuego);
            await conexionCliente.EnvioDeMensajeAsync(titulo);

            await EnvioMensajeConPrevioEncabezadoAsync(juegoEnString, Accion.ModificarJuego);

            await conexionCliente.EnvioDeArchivoAsync(juegoModificado.Caratula);

            await RecibirRespuestasAsync(Mensaje.JuegoModificadoOk, Mensaje.JuegoModificadoError, Mensaje.JuegoEliminado);
        }

        private async Task<List<Juego>> BuscarJuegoPorFiltroAsync(string filtro, string accion)
        {
            await conexionCliente.EnvioEncabezadoAsync(filtro.Length, accion);
            await conexionCliente.EnvioDeMensajeAsync(filtro);

            string mensaje = await conexionCliente.RecibirMensajeAsync();

            return Mapper.PasarStringAListaDeJuegos(mensaje);
        }

        private async Task<string> DevolverTituloJuegoSeleccionadoAsync()
        {
            List<string> juegos = await conexionCliente.RecibirListaDeJuegosAsync();

            if (juegos.Count == 0)
            {
                Mensaje.NoExistenJuegos();
                return "";
            }

            return SeleccionarUnTituloDeJuego(juegos);
        }

        private string SeleccionarUnTituloDeJuego(List<string> juegos)
        {
            Mensaje.MostrarJuegos(juegos, Mensaje.noHayJuegosRegistrados, Mensaje.listadoJuego);
            int juegoSeleccionado = Metodo.ObtenerOpcion(Mensaje.seleccioneJuego, 0, juegos.Count - 1);

            return juegos[juegoSeleccionado];
        }

        private async Task EnvioYRespuestaAsync(string mensaje, string accion, Action Ok, Action Error, Action Eliminado)
        {
            await EnvioMensajeConPrevioEncabezadoAsync(mensaje, accion);

            await RecibirRespuestasAsync(Ok, Error, Eliminado);
        }

        private async Task EnvioMensajeConPrevioEncabezadoAsync(string mensaje, string accion)
        {
            await conexionCliente.EnvioEncabezadoAsync(mensaje.Length, accion);
            await conexionCliente.EnvioDeMensajeAsync(mensaje);
        }

        private async Task RecibirRespuestasAsync(Action RespuestaOk, Action RespuestaError, Action Eliminado)
        {
            string respuestaServidor = await conexionCliente.EsperarPorRespuestaAsync();

            if (respuestaServidor == Constante.MensajeOk)
                RespuestaOk();
            else if (respuestaServidor == Constante.MensajeError)
                RespuestaError();
            else if (respuestaServidor == Constante.MensajeObjetoEliminado)
                Eliminado();
        }

    }
}
