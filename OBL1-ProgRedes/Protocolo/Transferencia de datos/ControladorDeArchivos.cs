using System.Text;
using System.IO;
using System;

namespace Protocolo.Transferencia_de_datos
{
    public class ControladorDeArchivos
    {
        public static  void EnviarArchivo(string ruta, Transferencia transferencia)
        {
            var informacionArchivo = new FileInfo(ruta);

            string nombreArchivo = informacionArchivo.Name;
            int largoMensaje = nombreArchivo.Length;

            Encabezado encabezado = new Encabezado(largoMensaje, Accion.EnviarCaratula);

            Controlador.EnviarEncabezado(transferencia, encabezado);

            transferencia.EnvioDeDatos(nombreArchivo);

            try
            {
                long tamañoArchivo = informacionArchivo.Length;
                string tamañoArchivoString = informacionArchivo.Length.ToString();
                int largoArchivoEnInt = tamañoArchivoString.Length;

                Encabezado encabezadoArchivo = new Encabezado(largoArchivoEnInt, Accion.EnviarCaratula);
                Controlador.EnviarEncabezado(transferencia, encabezadoArchivo);

                transferencia.EnvioDeDatos(tamañoArchivoString);

                EnviarArchivoPorPartes(transferencia, tamañoArchivo, ruta);
            }
            catch (System.IO.FileNotFoundException)
            {
                Encabezado encabezadoArchivo = new Encabezado(0, Accion.EliminarJuego);
                Controlador.EnviarEncabezado(transferencia, encabezadoArchivo);
                return;
            }
        }

        private static void EnviarArchivoPorPartes(Transferencia transferencia, long largoArchivo, string ruta)
        {
            long partesAEnviar = CalcularPartesAEnviar(largoArchivo);
            long posicionALeer = 0;
            long parteActual = 1;

            while (largoArchivo > posicionALeer)
            {
                byte[] dato;
                if (parteActual != partesAEnviar)
                {
                    dato = LecturaDeArchivo.LeerArchivo(ruta, Constante.maximoTamañoDePaquete, posicionALeer);
                    posicionALeer += Constante.maximoTamañoDePaquete;
                }
                else
                {
                    int lastPartSize = (int)(largoArchivo - posicionALeer);
                    dato = LecturaDeArchivo.LeerArchivo(ruta, lastPartSize, posicionALeer);
                    posicionALeer += lastPartSize;
                }

                transferencia.EnvioDeDatosByte(dato);
                parteActual++;
            }
        }

        public static void RecibirArchivos(Transferencia transferencia, string nombreArchivo)
        {
            Encabezado encabezado = Controlador.RecibirEncabezado(transferencia);

            int tamañoDelArchivo = encabezado.largoMensaje;

            byte[] largoTamañoDeArchivo = transferencia.RecibirDatos(tamañoDelArchivo);
            string largoArchivoASrting = Encoding.ASCII.GetString(largoTamañoDeArchivo, 0, tamañoDelArchivo);

            long largoArchivo = Convert.ToInt64(largoArchivoASrting);

            RecbirArchivo(transferencia, largoArchivo, nombreArchivo);
        }

        private static void RecbirArchivo(Transferencia transferencia, long tamañoArchivo, string nombreArchivo)
        {
            long partesArchivos = CalcularPartesAEnviar(tamañoArchivo);
            long datosLeidos = 0;
            long parteActual = 1;

            while (datosLeidos < tamañoArchivo)
            {
                byte[] dato;
                if (parteActual != partesArchivos)
                {
                    dato = transferencia.RecibirDatos(Constante.maximoTamañoDePaquete);
                    datosLeidos += Constante.maximoTamañoDePaquete;
                }
                else
                {
                    int largoUltimaParte = (int)(tamañoArchivo - datosLeidos);
                    dato = transferencia.RecibirDatos(largoUltimaParte);
                    datosLeidos += largoUltimaParte;
                }
                LecturaDeArchivo.EscribirArchivo(nombreArchivo, dato);
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
