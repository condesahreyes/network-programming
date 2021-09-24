using System.Collections.Generic;
using Cliente.Constantes;
using LogicaNegocio;
using Protocolo;
using System;
using Encabezado = Protocolo.Encabezado;

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

        public void BajaModificacion()
        {
            List<string> juegos = conexionCliente.RecibirListaDeJuegos();

            if (juegos.Count == 0)
            {
                Mensaje.NoExistenJuegos();
                return;
            }

            string titulo = SeleccionarUnTituloDeJuego(juegos);

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

            EnvioYRespuesta(juegoEnString, Accion.ModificarJuego, Mensaje.JuegoModificadoOk,
                Mensaje.JuegoModificadoError);
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

        public void BuscarJuego()
        {
            string opciones = "0. Buscar por titulo \n1. Buscar por genero \n2. Buscar por Calificacion";
            Juego.MostrarMensaje("Seleccion por que desea buscar:");
            int opcion = Metodo.ObtenerOpcion(opciones, 0, 2);
            string filtro = "";
            string accion = "";

            if (opcion == 0)
            {
                Juego.MostrarMensaje("Ingrese el Titulo por el que desea filtrar");
                filtro = Console.ReadLine();
                accion = Accion.BuscarTitulo;
            }
            else if (opcion == 1)
            {
                Juego.MostrarMensaje("Ingrese el Genero por el que desea filtrar");
                filtro = Console.ReadLine();
                accion = Accion.BuscarGenero;
            }
            else  if (opcion == 2)
            {
                filtro = Metodo.ObtenerOpcion(Mensaje.ranking, 1, 5).ToString();
                accion = Accion.BuscarCalificacion;
            }

            List<Juego> juegos = BuscarJuegoPorFiltro(filtro, accion);
            Mensaje.MostrarObjetoJuego(juegos);
        }

        public List<Juego> BuscarJuegoPorFiltro(string filtro, string accion) 
        {
            conexionCliente.EnvioEncabezado(filtro.Length, accion);
            conexionCliente.EnvioDeMensaje(filtro);

            Encabezado encabezado = conexionCliente.RecibirEncabezado();

            string mensaje = conexionCliente.RecibirMensaje(encabezado.largoMensaje);

            return Mapper.PasarStringAListaDeJuegos(mensaje);
        }

    }
}
