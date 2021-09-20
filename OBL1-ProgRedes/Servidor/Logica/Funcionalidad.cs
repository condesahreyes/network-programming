using Servidor.FuncionalidadesPorEntidad;
using Servidor.FuncionalidadesEntidades;
using System.Collections.Generic;
using LogicaNegocio;
using Protocolo;

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
        }

        public void CrearCalificacion(int largoMensajeARecibir)
        {
            Calificacion calificacion = Controlador.PublicarCalificacion(transferencia, largoMensajeARecibir);

            funcionesJuego.AgregarCalificacion(calificacion);

            EnviarRespuesta(calificacion != null);
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
    }
}
