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

        public static async Task InstanciarConexion()
        {
            conexionCliente = new Conexion();
            await conexionCliente.InstanciarTransferencia();
        }

        public static async Task<Funcionalidad> ObtenerInstancia()
        {
            if (_instancia == null)
            {
                _instancia = new Funcionalidad();
                await InstanciarConexion();
            }
  
            return _instancia;
        }

        public async Task<Usuario> InicioSesion()
        {
            Usuario usuario = Usuario.CrearUsuario();
            string nombreUsuario = usuario.NombreUsuario;

            await EnvioYRespuesta(nombreUsuario, Accion.Login, Mensaje.InicioSesion, 
                Mensaje.ErrorGenerico, Mensaje.ErrorGenerico);

            return usuario;
        }

        public void DesconectarUsuario(Usuario usuario)
        {
            conexionCliente.DesconectarUsuario(usuario);
        }

        public async Task PublicarJuego()
        {
            Juego unJuego = Juego.CrearJuego();

            string juegoEnString = Mapper.JuegoAString(unJuego);

            string caratula = unJuego.Caratula;

            await EnvioMensajeConPrevioEncabezado(juegoEnString, Accion.PublicarJuego);
            await conexionCliente.EnvioDeArchivo(caratula);
            await RecibirRespuestas(Mensaje.JuegoCreado, Mensaje.JuegoExistente, Mensaje.JuegoEliminado);
        }

        public async Task BuscarJuego()
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

            List<Juego> juegos = await BuscarJuegoPorFiltro(filtro, accion);
            Mensaje.MostrarObjetoJuego(juegos);
        }

        public async Task DetalleDeUnJuego()
        {
            string titulo = await DevolverTituloJuegoSeleccionado();

            if (titulo == "")
            {
                Mensaje.NoExistenJuegos();
                return;
            }
            try
            {
                Juego juego = await conexionCliente.RecibirUnJuegoPorTitulo(titulo);

                await conexionCliente.RecibirArchivos(juego.Caratula);
                Mensaje.MostrarJuego(juego);
            }
            catch(Exception)
            {
                Mensaje.JuegoEliminado();
            }
        }

        public async Task CalificarUnJuego(Usuario usuario)
        {
            string titulo = await DevolverTituloJuegoSeleccionado();

            if (titulo == "")
            {
                Mensaje.NoExistenJuegos();
                return;
            }

            Calificacion unaCalificacion = Calificacion.CrearCalificacion(usuario.NombreUsuario, titulo);

            string calificacionEnString = Mapper.CalificacionAString(unaCalificacion);

            await EnvioYRespuesta(calificacionEnString, Accion.PublicarCalificacion,
                Mensaje.CalificacionCreada, Mensaje.ErrorGenerico, Mensaje.JuegoEliminado);
        }

        public async Task AdquirirJuego(Usuario usuario)
        {
           string tituloJuego = await DevolverTituloJuegoSeleccionado();
           await EnvioYRespuesta(tituloJuego, Accion.AdquirirJuego, Mensaje.JuegoAdquirido, Mensaje.ErrorAdquirirJuego, Mensaje.JuegoEliminado);
        }

        public async Task ListaJuegosAdquiridos(Usuario usuario)
        {
            await conexionCliente.EnvioEncabezado(0, Accion.VerJuegosAdquiridos);
            List<string> juegosAdquiridos = await conexionCliente.RecibirListaDeJuegosAdquiridos();
            Mensaje.MostrarJuegos(juegosAdquiridos, Mensaje.noHayJuegosRegistrados, Mensaje.listadoJuegoAdquiridos);
        }

        public async Task BajaModificacionJuego()
        {
            string titulo = await DevolverTituloJuegoSeleccionado();

            if (titulo == "")
            {
                Mensaje.NoExistenJuegos();
                return;
            }

            int opcion = Metodo.ObtenerOpcion(Mensaje.bajaModificacion, 0, 1);

            if (opcion == 0)
                await EnvioYRespuesta(titulo, Accion.EliminarJuego, Mensaje.JuegoEliminadoOk, 
                    Mensaje.JuegoEliminadoError, Mensaje.JuegoEliminado);
            else if (opcion == 1)
                await ModificarUnJuego(titulo);
        }

        private async Task ModificarUnJuego(string titulo)
        {
            Juego juegoModificado = Juego.ModificarJuego();
            string juegoEnString = Mapper.JuegoAString(juegoModificado);

            await conexionCliente.EnvioEncabezado(titulo.Length, Accion.ModificarJuego);
            await conexionCliente.EnvioDeMensaje(titulo);

            await EnvioMensajeConPrevioEncabezado(juegoEnString, Accion.ModificarJuego);

            await conexionCliente.EnvioDeArchivo(juegoModificado.Caratula);

            await RecibirRespuestas(Mensaje.JuegoModificadoOk, Mensaje.JuegoModificadoError, Mensaje.JuegoEliminado);
        }

        private async Task<List<Juego>> BuscarJuegoPorFiltro(string filtro, string accion)
        {
            await conexionCliente.EnvioEncabezado(filtro.Length, accion);
            await conexionCliente.EnvioDeMensaje(filtro);

            string mensaje = await conexionCliente.RecibirMensaje();

            return Mapper.PasarStringAListaDeJuegos(mensaje);
        }

        private async Task<string> DevolverTituloJuegoSeleccionado()
        {
            List<string> juegos = await conexionCliente.RecibirListaDeJuegos();

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

        private async Task EnvioYRespuesta(string mensaje, string accion, Action Ok, Action Error, Action Eliminado)
        {
            await EnvioMensajeConPrevioEncabezado(mensaje, accion);

            await RecibirRespuestas(Ok, Error, Eliminado);
        }

        private async Task EnvioMensajeConPrevioEncabezado(string mensaje, string accion)
        {
            await conexionCliente.EnvioEncabezado(mensaje.Length, accion);
            await conexionCliente.EnvioDeMensaje(mensaje);
        }

        private async Task RecibirRespuestas(Action RespuestaOk, Action RespuestaError, Action Eliminado)
        {
            string respuestaServidor = await conexionCliente.EsperarPorRespuesta();

            if (respuestaServidor == Constante.MensajeOk)
                RespuestaOk();
            else if (respuestaServidor == Constante.MensajeError)
                RespuestaError();
            else if (respuestaServidor == Constante.MensajeObjetoEliminado)
                Eliminado();
        }

    }
}
