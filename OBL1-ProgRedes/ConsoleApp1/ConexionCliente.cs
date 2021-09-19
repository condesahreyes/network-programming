using System.Collections.Generic;
using System.Net.Sockets;
using Cliente.Constantes;
using LogicaNegocio;
using System.Net;
using Protocolo;

namespace Cliente
{
    public class ConexionCliente
    {
        int port = 9000;

        Transferencia transferencia;
        IPEndPoint ipEndPoint;
        Socket sender;

        public ConexionCliente()
        {
            port = 9000;

            ipEndPoint = new IPEndPoint(IPAddress.Loopback, port);
            sender = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            transferencia = new Transferencia(sender);

            // Conectarse desde un socket client
            sender.Connect(ipEndPoint);
            Mensaje.Conectado(sender.RemoteEndPoint.ToString());
        }

        //Desconexión
        public void DesconectarUsuario(Usuario usuario)
        {
            ControladorDeTransferencia.Desconectar(transferencia);
        }

        //RespuestasServidor
        public string EsperarPorRespuesta()
        {
            string respuesta = ControladorDeTransferencia.RecibirMensajeGenerico
                (transferencia, ConstantesDelProtocolo.largoEncabezado);
            string[] respuestas = respuesta.Split("#");

            return respuestas[0];
        }

        //Encabezados
        public void EnvioEncabezado(int largoMensaje, string accion)
        {
            Encabezado encabezado = new Encabezado(largoMensaje, accion);
            ControladorDeTransferencia.EnviarEncabezado(transferencia, encabezado);
        }

        public Encabezado RecibirEncabezado()
        {
            return ControladorDeTransferencia.RecibirEncabezado(transferencia);
        }

        //Mensajes
        public void EnvioDeMensaje(string mensaje)
        {
            ControladorDeTransferencia.EnviarDatos(transferencia, mensaje);
        }

        public string RecibirMensaje(int largoMensaje)
        {
            return ControladorDeTransferencia.RecibirMensajeGenerico(transferencia, largoMensaje);
        }

        //Juegos
        public Juego RecibirUnJuegoPorTitulo(string titulo)
        {
            return ControladorDeTransferencia.RecibirUnJuegoPorTitulo(transferencia, titulo);
        }

        public List<string> RecibirListaDeJuegos()
        {
            EnvioEncabezado(0, Accion.ListaJuegos);

            string juegos = ControladorDeTransferencia.RecibirEncabezadoYMensaje
                (transferencia, Accion.ListaJuegos);

            return Mapper.StringAListaJuegosString(juegos);
        }
    }
}
