using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Protocolo.Transferencia_de_datos
{
    public class ControladorDeArchivos
    {
        public static void EnviarArchivo(string ruta, Transferencia transferencia)
        {
            var informacionArchivo = new FileInfo(ruta);

            string nombreArchivo = informacionArchivo.Name;
            int largoMensaje = nombreArchivo.Length;

            Encabezado encabezado = new Encabezado(largoMensaje, Accion.EnviarCaratula);

            Controlador.EnviarEncabezado(transferencia, encabezado);

            transferencia.EnvioDeDatos(nombreArchivo);

            long tamañoArchivo = informacionArchivo.Length;
            string tamañoArchivoString = informacionArchivo.Length.ToString();
            int largoArchivoEnInt = tamañoArchivoString.Length;
            Encabezado encabezadoArchivo = new Encabezado(largoArchivoEnInt, Accion.EnviarCaratula);
            Controlador.EnviarEncabezado(transferencia, encabezadoArchivo);

            transferencia.EnvioDeDatos(tamañoArchivoString);

            EnviarArchivoPorPartes(transferencia, tamañoArchivo, ruta);
        }

        private static void EnviarArchivoPorPartes(Transferencia transferencia, long largoArchivo, string ruta)
        {
            long partesAEnviar = CalcularPartesAEnviar(largoArchivo);
            long posicionALeer = 0;
            long parteActual = 1;

            while (largoArchivo > posicionALeer)
            {
                byte[] data;
                if (parteActual != partesAEnviar)
                {
                    data = LecturaDeArchivo.ReadData(ruta, Constante.maximoTamañoDePaquete, posicionALeer);
                    posicionALeer += Constante.maximoTamañoDePaquete;
                }
                else
                {
                    int lastPartSize = (int)(largoArchivo - posicionALeer);
                    data = LecturaDeArchivo.ReadData(ruta, lastPartSize, posicionALeer);
                    posicionALeer += lastPartSize;
                }

                transferencia.EnvioDeDatosByte(data);
                parteActual++;
            }
        }

        public static void RecibirArchivos(Transferencia transferencia, string nombreArchivo)
        {
            Encabezado encabezado = Controlador.RecibirEncabezado(transferencia);

            int tamañoDelArchivo = encabezado.largoMensaje;

            byte[] fileSizeDataLength = transferencia.RecibirDatos(tamañoDelArchivo);
            string fileeeeAlgo = Encoding.ASCII.GetString(fileSizeDataLength, 0, tamañoDelArchivo);

            long fileSize = Convert.ToInt64(fileeeeAlgo);

            ReceiveFile(transferencia, fileSize, nombreArchivo);
        }

        private static void ReceiveFile(Transferencia transferencia, long tamañoArchivo, string fileName)
        {
            long partesArchivos = CalcularPartesAEnviar(tamañoArchivo);
            long offset = 0;
            long parteActual = 1;

            while (tamañoArchivo > offset)
            {
                byte[] data;
                if (parteActual != partesArchivos)
                {
                    data = transferencia.RecibirDatos(Constante.maximoTamañoDePaquete);
                    offset += Constante.maximoTamañoDePaquete;
                }
                else
                {
                    int lastPartSize = (int)(tamañoArchivo - offset);
                    data = transferencia.RecibirDatos(lastPartSize);
                    offset += lastPartSize;
                }
                LecturaDeArchivo.WriteData(fileName, data);
                parteActual++;
            }
        }

        private static long CalcularPartesAEnviar(long tamaño)
        {
            long partes = tamaño / Constante.maximoTamañoDePaquete;
            return partes * Constante.maximoTamañoDePaquete == tamaño ? partes : partes + 1;
        }

    }
}
