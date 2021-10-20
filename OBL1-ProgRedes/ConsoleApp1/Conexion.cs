using Microsoft.Extensions.Configuration;
using Protocolo.Transferencia_de_datos;
using System.Collections.Generic;
using System.Net.Sockets;
using Cliente.Constantes;
using LogicaNegocio;
using System.Net;
using Protocolo;
using System.IO;
using System.Threading.Tasks;

namespace Cliente
{
    public class Conexion
    {
        Transferencia transferencia;

        public Conexion()
        {
            IConfiguration configuracion = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json", optional: false).Build();

            int puertoCliente = int.Parse(configuracion["puertoCliente"]);
            int puertoServidor = int.Parse(configuracion["puertoServidor"]);
            string ipServidor = configuracion["ipServidor"];
            string ipCliente = configuracion["ipCliente"];

            IPEndPoint endPointCliente = new IPEndPoint(IPAddress.Parse(ipCliente), puertoCliente);
            IPEndPoint endPointServidor = new IPEndPoint(IPAddress.Parse(ipServidor), puertoServidor);

            TcpClient sender = new TcpClient(endPointCliente);

            //sender.Bind(endPointCliente);

            

            sender.Connect(endPointServidor);
            Mensaje.Conectado(sender.ToString());
            transferencia = new Transferencia(sender);
        }

        public void DesconectarUsuario(Usuario usuario)
        {
            transferencia.Desconectar();
        }

        public async Task<string> EsperarPorRespuesta()
        {
            string respuesta = await Controlador.RecibirMensajeGenerico
                (transferencia, Constante.largoEncabezado);
            string[] respuestas = respuesta.Split("#");

            return respuestas[0];
        }

        public async Task EnvioEncabezado(int largoMensaje, string accion)
        {
            Encabezado encabezado = new Encabezado(largoMensaje, accion);
            await Controlador.EnviarEncabezado(transferencia, encabezado);
        }

        public async Task<Encabezado> RecibirEncabezado()
        {
            return await Controlador.RecibirEncabezado(transferencia);
        }

        public async Task EnvioDeMensaje(string mensaje)
        {
            await Controlador.EnviarDatos(transferencia, mensaje);
        }

        public async Task<string> RecibirMensaje()
        {
            Encabezado encabezado = await RecibirEncabezado();

            return await Controlador.RecibirMensajeGenerico(transferencia, encabezado.largoMensaje);
        }

        public async Task<Juego> RecibirUnJuegoPorTitulo(string titulo)
        {
            return await Controlador.RecibirUnJuegoPorTitulo(transferencia, titulo);
        }

        public async Task<List<string>> RecibirListaDeJuegos()
        {
            await EnvioEncabezado(0, Accion.ListaJuegos);

            string juegos = await Controlador.RecibirEncabezadoYMensaje
                (transferencia, Accion.ListaJuegos);

            return Mapper.StringAListaJuegosString(juegos);
        }

        public async Task<List<string>> RecibirListaDeJuegosAdquiridos()
        {
            string juegos = await Controlador.RecibirEncabezadoYMensaje
                (transferencia, Accion.VerJuegosAdquiridos);

            return Mapper.StringAListaJuegosString(juegos);
        }

        public async Task EnvioDeArchivo(string archivo)
        {
            await ControladorDeArchivos.EnviarArchivo(archivo, transferencia);
        }

        public async Task<string> RecibirArchivos(string caratula)
        {
            Encabezado encabezado = await RecibirEncabezado();

            if (encabezado.accion == Accion.EliminarJuego)
                return "";

            string nombreArchivo = await Controlador.RecibirMensajeGenerico(transferencia, encabezado.largoMensaje);
            await ControladorDeArchivos.RecibirArchivos(transferencia, nombreArchivo);

            return nombreArchivo;
        }
    }
}