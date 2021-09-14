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

            byte[] datos = transferencia.RecibirDatos(largoEncabezado);
            
            string stringRecibido = Encoding.ASCII.GetString(datos, 0, largoEncabezado);

            Encabezado encabezadoRecibido = Mapper.StringAEncabezado(stringRecibido);

            return encabezadoRecibido;
        }

        public static Usuario RecibirUsuario(Transferencia transferencia, int largoMensaje)
        {
            byte[] datos = transferencia.RecibirDatos(largoMensaje);

            string stringRecibido = Encoding.ASCII.GetString(datos, 0, largoMensaje);

            Usuario usuario = Mapper.StringAUsuario(stringRecibido);

            return usuario;
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
    }
}
