using LogicaNegocio;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Protocolo
{
    public static class TransferenciaDatos
    {

        public static Encabezado RecibirEncabezado(Transferencia transferencia)
        {
            int largoEncabezado = ConstantesDelProtocolo.largoEncabezado;

            string stringRecibido = RecibirMensajeGenerico(transferencia, largoEncabezado);

            Encabezado encabezadoRecibido = Mapper.StringAEncabezado(stringRecibido);

            return encabezadoRecibido;
        }

        public static Usuario RecibirUsuario(Transferencia transferencia, int largoMensaje)
        {
            string stringRecibido = RecibirMensajeGenerico(transferencia, largoMensaje);

            Usuario usuario = Mapper.StringAUsuario(stringRecibido);

            return usuario;
        }

        public static string RecibirMensajeGenerico(Transferencia transferencia, int largoMensaje)
        {
            byte[] datos = transferencia.RecibirDatos(largoMensaje);

            return Encoding.ASCII.GetString(datos, 0, largoMensaje);
        }

        public static void EnviarEncabezado(Transferencia transferencia, Encabezado encabezado)
        {
            string mensajeAEnviar = Mapper.EncabezadoAString(encabezado);

            transferencia.EnvioDeDatos(mensajeAEnviar);
        }

        public static void EnviarDatos(Transferencia transferencia, string datos)
        {
            transferencia.EnvioDeDatos(datos);
        }

        public static void EnviarDatos()
        {
            throw new NotImplementedException();
        }

        public static void Desconectar(Transferencia transferencia)
        {
            transferencia.Desconectar();
        }

        public static Juego PublicarJuego(Transferencia transferencia, int largoMensaje)
        {
            byte[] datos = transferencia.RecibirDatos(largoMensaje);

            string stringRecibido = Encoding.ASCII.GetString(datos, 0, largoMensaje);

            Juego juego = Mapper.StringAJuego(stringRecibido);

            return juego;
        }

        public static void EnviarMensajeClienteOk(Transferencia transferencia)
        {
            Encabezado encabezado = new Encabezado(0, ConstantesDelProtocolo.MensajeOk);
            string encabezadoEnString = Mapper.EncabezadoAString(encabezado);

            transferencia.EnvioDeDatos(encabezadoEnString);
        }

        public static void EnviarMensajeClienteError(Transferencia transferencia)
        {
            Encabezado encabezado = new Encabezado(0, ConstantesDelProtocolo.MensajeEr);
            string encabezadoEnString = Mapper.EncabezadoAString(encabezado);

            transferencia.EnvioDeDatos(encabezadoEnString);
        }
    }
}
