using LogicaNegocio;
using System.Text;
using System;
using System.IO;

namespace Protocolo
{
    public class Controlador
    {
        public static Encabezado RecibirEncabezado(Transferencia transferencia)
        {
            int largoEncabezado = Constante.largoEncabezado;

            string stringRecibido = RecibirMensajeGenerico(transferencia, largoEncabezado);

            Encabezado encabezadoRecibido = Mapper.StringAEncabezado(stringRecibido);

            return encabezadoRecibido;
        }

        public static void EnviarEncabezado(Transferencia transferencia, Encabezado encabezado)
        {
            string mensajeAEnviar = Mapper.EncabezadoAString(encabezado);

            transferencia.EnvioDeDatos(mensajeAEnviar);
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

        public static void EnviarDatos(Transferencia transferencia, string datos)
        {
            transferencia.EnvioDeDatos(datos);
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
            Encabezado encabezado = new Encabezado(0, Constante.MensajeOk);
            string encabezadoEnString = Mapper.EncabezadoAString(encabezado);

            transferencia.EnvioDeDatos(encabezadoEnString);
        }

        public static void EnviarMensajeClienteError(Transferencia transferencia)
        {
            Encabezado encabezado = new Encabezado(0, Constante.MensajeError);
            string encabezadoEnString = Mapper.EncabezadoAString(encabezado);

            transferencia.EnvioDeDatos(encabezadoEnString);
        }

        public static Juego RecibirUnJuegoPorTitulo(Transferencia transferencia, string tituloJuego)
        {
            Encabezado encabezado = new Encabezado(tituloJuego.Length, Accion.PedirDetalleJuego);
            EnviarEncabezado(transferencia, encabezado);
            EnviarDatos(transferencia, tituloJuego);

            string juego = RecibirEncabezadoYMensaje(transferencia, Accion.EnviarDetalleJuego);

            return Mapper.StringAJuego(juego);
        }

        public static string RecibirEncabezadoYMensaje(Transferencia transferencia, string accionEsperada)
        {
            Encabezado encabezadoRecibido = RecibirEncabezado(transferencia);

            string accion = encabezadoRecibido.accion;

            if (accion != accionEsperada)
                throw new Exception();

            int largoMensaje = encabezadoRecibido.largoMensaje;

            return RecibirMensajeGenerico(transferencia, largoMensaje);
        }

        public static Calificacion PublicarCalificacion(Transferencia transferencia, int largoMensaje)
        {
            byte[] datos = transferencia.RecibirDatos(largoMensaje);

            string stringRecibido = Encoding.ASCII.GetString(datos, 0, largoMensaje);

            Calificacion calificacion = Mapper.StringACalificacion(stringRecibido);

            return calificacion;
        }

        private void ReceiveFile(Transferencia transferencia, long fileSize, string fileName)
        {
            long fileParts = Constante.CalculateParts(fileSize);
            long offset = 0;
            long currentPart = 1;
            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart != fileParts)
                {
                    data = transferencia.RecibirDatos(Constante.maximoTamañoDePaquete);
                    offset += Constante.maximoTamañoDePaquete;
                }
                else
                {
                    int lastPartSize = (int)(fileSize - offset);
                    data = transferencia.RecibirDatos(lastPartSize);
                    offset += lastPartSize;
                }
                
                WriteData(fileName, data);
                currentPart++;
            }
        }

        public void WriteData(string path, byte[] data)
        {
            if (File.Exists(path))
            {
                using FileStream fileStream = new FileStream(path, FileMode.Append);
                fileStream.Write(data, 0, data.Length);
            }
            else
            {
                using FileStream fileStream = new FileStream(path, FileMode.Create);
                fileStream.Write(data, 0, data.Length);
            }
        }
    }
}
