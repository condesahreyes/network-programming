using System.Text;
using System.IO;
using System;
using System.Threading.Tasks;

namespace Protocolo.Transferencia_de_datos
{
    public class ControladorDeArchivos
    {
        public static async Task EnviarArchivoAsync(string ruta, Transferencia transferencia)
        {
            var informacionArchivo = new FileInfo(ruta);

            string nombreArchivo = informacionArchivo.Name;
            int largoMensaje = nombreArchivo.Length;

            Encabezado encabezado = new Encabezado(largoMensaje, Accion.EnviarCaratula);

            await Controlador.EnviarEncabezado(transferencia, encabezado);

            await transferencia.EnvioDeDatosAsync(nombreArchivo);

            try
            {
                long tamañoArchivo = informacionArchivo.Length;
                string tamañoArchivoString = informacionArchivo.Length.ToString();
                int largoArchivoEnInt = tamañoArchivoString.Length;

                Encabezado encabezadoArchivo = new Encabezado(largoArchivoEnInt, Accion.EnviarCaratula);
                await Controlador.EnviarEncabezado(transferencia, encabezadoArchivo);

                await transferencia.EnvioDeDatosAsync(tamañoArchivoString);

                await EnviarArchivoPorPartesAsync(transferencia, tamañoArchivo, ruta);
            }
            catch (System.IO.FileNotFoundException)
            {
                Encabezado encabezadoArchivo = new Encabezado(0, Accion.EliminarJuego);
                await Controlador.EnviarEncabezado(transferencia, encabezadoArchivo);
                return;
            }
        }

        private static async Task EnviarArchivoPorPartesAsync(Transferencia transferencia, long largoArchivo, string ruta)
        {
            long partesAEnviar = CalcularPartesAEnviar(largoArchivo);
            long posicionALeer = 0;
            long parteActual = 1;

            while (largoArchivo > posicionALeer)
            {
                byte[] dato;
                if (parteActual != partesAEnviar)
                {
                    dato = await LecturaDeArchivo.LeerArchivoAsync(ruta, Constante.maximoTamañoDePaquete, posicionALeer);
                    posicionALeer += Constante.maximoTamañoDePaquete;
                }
                else
                {
                    int lastPartSize = (int)(largoArchivo - posicionALeer);
                    dato = await LecturaDeArchivo.LeerArchivoAsync(ruta, lastPartSize, posicionALeer);
                    posicionALeer += lastPartSize;
                }

                await transferencia.EnvioDeDatosByteAsync(dato);
                parteActual++;
            }
        }

        public static async Task RecibirArchivosAsync(Transferencia transferencia, string nombreArchivo)
        {
            Encabezado encabezado = await Controlador.RecibirEncabezado(transferencia);

            int tamañoDelArchivo = encabezado.largoMensaje;

            byte[] largoTamañoDeArchivo = await transferencia .RecibirDatosAsync(tamañoDelArchivo);
            string largoArchivoASrting = Encoding.ASCII.GetString(largoTamañoDeArchivo, 0, tamañoDelArchivo);

            long largoArchivo = Convert.ToInt64(largoArchivoASrting);

            await RecbirArchivoAsync(transferencia, largoArchivo, nombreArchivo);
        }

        private static async Task RecbirArchivoAsync(Transferencia transferencia, long tamañoArchivo, string nombreArchivo)
        {
            long partesArchivos = CalcularPartesAEnviar(tamañoArchivo);
            long datosLeidos = 0;
            long parteActual = 1;

            while (datosLeidos < tamañoArchivo)
            {
                byte[] dato;
                if (parteActual != partesArchivos)
                {
                    dato = await transferencia.RecibirDatosAsync(Constante.maximoTamañoDePaquete);
                    datosLeidos += Constante.maximoTamañoDePaquete;
                }
                else
                {
                    int largoUltimaParte = (int)(tamañoArchivo - datosLeidos);
                    dato = await transferencia.RecibirDatosAsync(largoUltimaParte);
                    datosLeidos += largoUltimaParte;
                }
                await LecturaDeArchivo.EscribirArchivoAsync(nombreArchivo, dato);
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
