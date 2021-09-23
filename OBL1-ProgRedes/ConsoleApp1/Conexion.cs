using System.Collections.Generic;
using System.Net.Sockets;
using Cliente.Constantes;
using LogicaNegocio;
using System.Net;
using Protocolo;
using Encabezado = Protocolo.Encabezado;

namespace Cliente
{
    public class Conexion
    {
        int port = 9000;

        Transferencia transferencia;
        IPEndPoint ipEndPoint;
        Socket sender;

        public Conexion()
        {
            port = 9000;

            ipEndPoint = new IPEndPoint(IPAddress.Loopback, port);
            sender = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            transferencia = new Transferencia(sender);

            // Conectarse desde un socket client
            sender.Connect(ipEndPoint);
            Mensaje.Conectado(sender.RemoteEndPoint.ToString());
        }

        public void DesconectarUsuario(Usuario usuario)
        {
            Controlador.Desconectar(transferencia);
        }

        public string EsperarPorRespuesta()
        {
            string respuesta = Controlador.RecibirMensajeGenerico
                (transferencia, Constante.largoEncabezado);
            string[] respuestas = respuesta.Split("#");

            return respuestas[0];
        }

        public void EnvioEncabezado(int largoMensaje, string accion)
        {
            Encabezado encabezado = new Encabezado(largoMensaje, accion);
            Controlador.EnviarEncabezado(transferencia, encabezado);
        }

        public Encabezado RecibirEncabezado()
        {
            return Controlador.RecibirEncabezado(transferencia);
        }

        public void EnvioDeMensaje(string mensaje)
        {
            Controlador.EnviarDatos(transferencia, mensaje);
        }

        public string RecibirMensaje(int largoMensaje)
        {
            return Controlador.RecibirMensajeGenerico(transferencia, largoMensaje);
        }

        public Juego RecibirUnJuegoPorTitulo(string titulo)
        {
            return Controlador.RecibirUnJuegoPorTitulo(transferencia, titulo);
        }

        public List<string> RecibirListaDeJuegos()
        {
            EnvioEncabezado(0, Accion.ListaJuegos);

            string juegos = Controlador.RecibirEncabezadoYMensaje
                (transferencia, Accion.ListaJuegos);

            return Mapper.StringAListaJuegosString(juegos);
        }
    }
}
