using LogicaNegocio;
using System.Text;
using System;
using System.Threading.Tasks;

namespace Protocolo
{
    public class Controlador
    {
        public static async Task<Encabezado> RecibirEncabezado(Transferencia transferencia)
        {
            int largoEncabezado = Constante.largoEncabezado;

            string stringRecibido = await RecibirMensajeGenerico(transferencia, largoEncabezado);

            Encabezado encabezadoRecibido = Mapper.StringAEncabezado(stringRecibido);

            return encabezadoRecibido;
        }

        public static async Task EnviarEncabezado(Transferencia transferencia, Encabezado encabezado)
        {
            string mensajeAEnviar = Mapper.EncabezadoAString(encabezado);

            await transferencia.EnvioDeDatosAsync(mensajeAEnviar);
        }

        public static async Task<Usuario> RecibirUsuario(Transferencia transferencia, int largoMensaje)
        {
            string stringRecibido = await RecibirMensajeGenerico(transferencia, largoMensaje);

            Usuario usuario = Mapper.StringAUsuario(stringRecibido);

            return usuario;
        }

        public static async Task<string> RecibirMensajeGenerico(Transferencia transferencia, int largoMensaje)
        {
            byte[] datos = await transferencia.RecibirDatosAsync(largoMensaje);

            return Encoding.ASCII.GetString(datos, 0, largoMensaje);
        }

        public static async Task EnviarDatos(Transferencia transferencia, string datos)
        {
            await transferencia.EnvioDeDatosAsync(datos);
        }

        public static async Task<Juego> PublicarJuego(Transferencia transferencia, int largoMensaje)
        {
            byte[] datos = await transferencia.RecibirDatosAsync(largoMensaje);

            string stringRecibido = Encoding.ASCII.GetString(datos, 0, largoMensaje);

            Juego juego = Mapper.StringAJuego(stringRecibido);

            return juego;
        }

        public static async Task EnviarMensajeClienteOk(Transferencia transferencia)
        {
            Encabezado encabezado = new Encabezado(0, Constante.MensajeOk);
            string encabezadoEnString = Mapper.EncabezadoAString(encabezado);

            await transferencia.EnvioDeDatosAsync(encabezadoEnString);
        }

        public static async Task EnviarMensajeClienteError(Transferencia transferencia)
        {
            Encabezado encabezado = new Encabezado(0, Constante.MensajeError);
            string encabezadoEnString = Mapper.EncabezadoAString(encabezado);

            await transferencia.EnvioDeDatosAsync(encabezadoEnString);
        }

        public static async Task EnviarMensajeClienteObjetoEliminado(Transferencia transferencia)
        {
            Encabezado encabezado = new Encabezado(0, Constante.MensajeObjetoEliminado);
            string encabezadoEnString = Mapper.EncabezadoAString(encabezado);

            await transferencia.EnvioDeDatosAsync(encabezadoEnString);
        }

        public static async Task<Juego> RecibirUnJuegoPorTitulo(Transferencia transferencia, string tituloJuego)
        {
            Encabezado encabezado = new Encabezado(tituloJuego.Length, Accion.PedirDetalleJuego);
            await EnviarEncabezado(transferencia, encabezado);
            await EnviarDatos (transferencia, tituloJuego);
            string juego = await RecibirEncabezadoYMensaje(transferencia, Accion.EnviarDetalleJuego);
            return Mapper.StringAJuego(juego);
        }

        public static async Task<string> RecibirEncabezadoYMensaje(Transferencia transferencia, string accionEsperada)
        {

            Encabezado encabezadoRecibido = await RecibirEncabezado(transferencia);

            string accion = encabezadoRecibido.accion;

            if (accion != accionEsperada)
                throw new Exception();

            int largoMensaje = encabezadoRecibido.largoMensaje;

            return await RecibirMensajeGenerico(transferencia, largoMensaje);
        }

        public static async Task<Calificacion> PublicarCalificacion(Transferencia transferencia, int largoMensaje)
        {
            byte[] datos = await transferencia.RecibirDatosAsync(largoMensaje);

            string stringRecibido = Encoding.ASCII.GetString(datos, 0, largoMensaje);

            Calificacion calificacion = Mapper.StringACalificacion(stringRecibido);

            return calificacion;
        }
    }
}
