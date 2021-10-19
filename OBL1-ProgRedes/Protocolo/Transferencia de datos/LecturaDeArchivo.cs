using System.IO;
using System;

namespace Protocolo.Transferencia_de_datos
{
    public class LecturaDeArchivo
    {
        public static byte[] LeerArchivo(string path, int largo, long posicion)
        {
            byte[] respuesta = new byte[largo];
            using FileStream fileStream = new FileStream(path, FileMode.Open) { Position = posicion };
            int datosLeidos = 0;
            while (datosLeidos < largo)
            {
                int leer = fileStream.Read(respuesta, datosLeidos, largo - datosLeidos);
                if (leer == 0)
                {
                    throw new Exception("No se puede leer el archivo");
                }

                datosLeidos += leer;
            }

            return respuesta;
        }

        public static void EscribirArchivo(string path, byte[] datos)
        {
            if (File.Exists(path))
            {
                using FileStream fileStream = new FileStream(path, FileMode.Append);
                fileStream.Write(datos, 0, datos.Length);
            }
            else
            {
                using FileStream fileStream = new FileStream(path, FileMode.Create);
                fileStream.Write(datos, 0, datos.Length);
            }
        }
    }
}
