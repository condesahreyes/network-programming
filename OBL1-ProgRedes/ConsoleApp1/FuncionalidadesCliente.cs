using System.Collections.Generic;
using Cliente.Constantes;
using LogicaNegocio;
using Protocolo;

namespace Cliente
{
    public class FuncionalidadesCliente
    {
        private static FuncionalidadesCliente _instancia;
        private ConexionCliente conexionCliente;

        public  FuncionalidadesCliente()
        {
            this.conexionCliente = new ConexionCliente();
        }

        public static FuncionalidadesCliente ObtenerInstancia()
        {
            return _instancia == null ? new FuncionalidadesCliente() : _instancia;
        }

        public Usuario InicioSesion()
        {
            Usuario usuario = Usuario.CrearUsuario();
            string nombreUsuario = usuario.NombreUsuario;

            EnvioDeMensaje(nombreUsuario, Accion.Login);

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
            int juegoSeleccionado = Metodos.ObtenerOpcion(Mensaje.seleccioneJuego, 0, juegos.Count - 1);

            return juegos[juegoSeleccionado];
        }

        public void PublicarJuego()
        {
            Juego unJuego = Juego.CrearJuego();

            string juegoEnString = Mapper.JuegoAString(unJuego);

            EnvioDeMensaje(juegoEnString, Accion.PublicarJuego);

            string respuestaServidor = conexionCliente.EsperarPorRespuesta();

            if(respuestaServidor == ConstantesDelProtocolo.MensajeOk)
                Mensaje.JuegoCreado();
            else
                Mensaje.JuegoExistente();
        }

        private void EnvioDeMensaje(string mensaje, string accion)
        {
            conexionCliente.EnvioEncabezado(mensaje.Length, accion);

            conexionCliente.EnvioDeMensaje(mensaje);
        }

    }
}
