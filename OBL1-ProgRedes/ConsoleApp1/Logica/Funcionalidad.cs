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
            if (_instancia == null)
                _instancia = new Funcionalidad();
  
            return _instancia;
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

        public void PublicarJuego()
        {
            Juego unJuego = Juego.CrearJuego();

            string juegoEnString = Mapper.JuegoAString(unJuego);

            string caratula = unJuego.Caratula;

            EnvioMensajeConPrevioEncabezado(juegoEnString, Accion.PublicarJuego);
            conexionCliente.EnvioDeArchivo(caratula);
            RecibirRespuestas(Mensaje.JuegoCreado, Mensaje.JuegoExistente);
        }

        public void BuscarJuego()
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

            List<Juego> juegos = BuscarJuegoPorFiltro(filtro, accion);
            Mensaje.MostrarObjetoJuego(juegos);
        }

        public void DetalleDeUnJuego()
        {
            string titulo = DevolverTituloJuegoSeleccionado();

            if (titulo == "")
            {
                Mensaje.NoExistenJuegos();
                return;
            }

            try
            {
                Juego juego = conexionCliente.RecibirUnJuegoPorTitulo(titulo);

                conexionCliente.RecibirArchivos(juego.Caratula);
                Mensaje.MostrarJuego(juego);
            }
            catch(Exception)
            {
                Mensaje.JuegoEliminado();
            }
        }

        public void CalificarUnJuego(Usuario usuario)
        {
            string titulo = DevolverTituloJuegoSeleccionado();

            if (titulo == "")
            {
                Mensaje.NoExistenJuegos();
                return;
            }

            Calificacion unaCalificacion = Calificacion.CrearCalificacion(usuario.NombreUsuario, titulo);

            string calificacionEnString = Mapper.CalificacionAString(unaCalificacion);

            EnvioYRespuesta(calificacionEnString, Accion.PublicarCalificacion,
                Mensaje.CalificacionCreada, Mensaje.ErrorGenerico);
        }

        public void AdquirirJuego(Usuario usuario)
        {
           string tituloJuego = DevolverTituloJuegoSeleccionado();
           EnvioYRespuesta(tituloJuego, Accion.AdquirirJuego, Mensaje.JuegoAdquirido, Mensaje.JuegoInexistente);
        }

        public void ListaJuegosAdquiridos(Usuario usuario)
        {
            conexionCliente.EnvioEncabezado(0, Accion.VerJuegosAdquiridos);
            List<string> juegosAdquiridos = conexionCliente.RecibirListaDeJuegosAdquiridos();
            Mensaje.MostrarJuegos(juegosAdquiridos);
        }

        public void BajaModificacionJuego()
        {
            string titulo = DevolverTituloJuegoSeleccionado();

            if (titulo == "")
            {
                Mensaje.NoExistenJuegos();
                return;
            }

            int opcion = Metodo.ObtenerOpcion(Mensaje.bajaModificacion, 0, 1);

            if (opcion == 0)
                EnvioYRespuesta(titulo, Accion.EliminarJuego, Mensaje.JuegoEliminadoOk, 
                    Mensaje.JuegoEliminadoError);
            else if (opcion == 1)
                ModificarUnJuego(titulo);
        }

        private void ModificarUnJuego(string titulo)
        {
            Juego juegoModificado = Juego.ModificarJuego();
            string juegoEnString = Mapper.JuegoAString(juegoModificado);

            conexionCliente.EnvioEncabezado(titulo.Length, Accion.ModificarJuego);
            conexionCliente.EnvioDeMensaje(titulo);

            EnvioMensajeConPrevioEncabezado(juegoEnString, Accion.ModificarJuego);

            conexionCliente.EnvioDeArchivo(juegoModificado.Caratula);

            RecibirRespuestas(Mensaje.JuegoModificadoOk, Mensaje.JuegoModificadoError);
        }

        private List<Juego> BuscarJuegoPorFiltro(string filtro, string accion)
        {
            conexionCliente.EnvioEncabezado(filtro.Length, accion);
            conexionCliente.EnvioDeMensaje(filtro);

            string mensaje = conexionCliente.RecibirMensaje();

            return Mapper.PasarStringAListaDeJuegos(mensaje);
        }

        private string DevolverTituloJuegoSeleccionado()
        {
            List<string> juegos = conexionCliente.RecibirListaDeJuegos();

            if (juegos.Count == 0)
            {
                Mensaje.NoExistenJuegos();
                return "";
            }

            return SeleccionarUnTituloDeJuego(juegos);
        }

        private string SeleccionarUnTituloDeJuego(List<string> juegos)
        {
            Mensaje.MostrarJuegos(juegos);
            int juegoSeleccionado = Metodo.ObtenerOpcion(Mensaje.seleccioneJuego, 0, juegos.Count - 1);

            return juegos[juegoSeleccionado];
        }

        private void EnvioYRespuesta(string mensaje, string accion, Action Ok, Action Error)
        {
            EnvioMensajeConPrevioEncabezado(mensaje, accion);

            RecibirRespuestas(Ok, Error);
        }

        private void EnvioMensajeConPrevioEncabezado(string mensaje, string accion)
        {
            conexionCliente.EnvioEncabezado(mensaje.Length, accion);
            conexionCliente.EnvioDeMensaje(mensaje);
        }

        private void RecibirRespuestas(Action RespuestaOk, Action RespuestaError)
        {
            string respuestaServidor = conexionCliente.EsperarPorRespuesta();

            if (respuestaServidor == Constante.MensajeOk)
                RespuestaOk();
            else
                RespuestaError();
        }

    }
}
