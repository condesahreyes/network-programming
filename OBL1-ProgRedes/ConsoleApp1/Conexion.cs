using Microsoft.Extensions.Configuration;
using Protocolo.Transferencia_de_datos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using LogicaNegocio;
using System.Net;
using Protocolo;
using System.IO;

namespace Cliente
{
    public class Conexion
    {
        Transferencia transferencia;

        public Conexion() { }

        public async Task InstanciarTransferencia()
        {
            IConfiguration configuracion = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json", optional: false).Build();

            int puertoCliente = int.Parse(configuracion["puertoCliente"]);
            int puertoServidor = int.Parse(configuracion["puertoServidor"]);
            string ipServidor = configuracion["ipServidor"];
            string ipCliente = configuracion["ipCliente"];

            IPEndPoint endPointCliente = new IPEndPoint(IPAddress.Parse(ipCliente), puertoCliente);

            TcpClient sender = new TcpClient(endPointCliente);

            await sender.ConnectAsync(ipServidor, puertoServidor);
            transferencia = new Transferencia(sender);
        }

        public void DesconectarUsuario(Usuario usuario)
        {
            transferencia.Desconectar();
        }

        public async Task<string> EsperarPorRespuesta()
        {
            string respuesta = await Controlador.RecibirMensajeGenericoAsync
                (transferencia, Constante.largoEncabezado);
            string[] respuestas = respuesta.Split("#");

            return respuestas[0];
        }

        public async Task EnvioEncabezado(int largoMensaje, string accion)
        {
            Encabezado encabezado = new Encabezado(largoMensaje, accion);
            await Controlador.EnviarEncabezadoAsync(transferencia, encabezado);
        }

        public async Task<Encabezado> RecibirEncabezado()
        {
            return await Controlador.RecibirEncabezadoAsync(transferencia);
        }

        public async Task EnvioDeMensaje(string mensaje)
        {
            await Controlador.EnviarDatos(transferencia, mensaje);
        }

        public async Task<string> RecibirMensaje()
        {
            Encabezado encabezado = await RecibirEncabezado();

            return await Controlador.RecibirMensajeGenericoAsync(transferencia, encabezado.largoMensaje);
        }

        public async Task<Juego> RecibirUnJuegoPorTitulo(string titulo)
        {
            return await Controlador.RecibirUnJuegoPorTituloAsync(transferencia, titulo);
        }

        public async Task<List<string>> RecibirListaDeJuegos()
        {
            await EnvioEncabezado(0, Accion.ListaJuegos);

            string juegos = await Controlador.RecibirEncabezadoYMensajeAsync
                (transferencia, Accion.ListaJuegos);

            return Mapper.StringAListaJuegosString(juegos);
        }

        public async Task<List<string>> RecibirListaDeJuegosAdquiridos()
        {
            string juegos = await Controlador.RecibirEncabezadoYMensajeAsync
                (transferencia, Accion.VerJuegosAdquiridos);

            return Mapper.StringAListaJuegosString(juegos);
        }

        public async Task EnvioDeArchivo(string archivo)
        {
            await ControladorDeArchivos.EnviarArchivoAsync(archivo, transferencia);
        }

        public async Task<string> RecibirArchivos(string caratula)
        {
            Encabezado encabezado = await RecibirEncabezado();

            if (encabezado.accion == Accion.EliminarJuego)
                return "";

            string nombreArchivo = await Controlador.RecibirMensajeGenericoAsync(transferencia, encabezado.largoMensaje);
            await ControladorDeArchivos.RecibirArchivosAsync(transferencia, nombreArchivo);

            return nombreArchivo;
        }
    }
}