namespace Protocolo
{
    public class Constante
    {
        public static int largoComando = 1;
        public static int largoDato = 4;
        public static int largoEncabezado = largoComando + largoDato + 1;
        public static int maximoTamañoDePaquete = 32768;

        public const string MensajeOk = "Ok";
        public const string MensajeError = "FAIL";

        public static long CalculateParts(long size)
        {
            long parts = size / maximoTamañoDePaquete;
            return parts * maximoTamañoDePaquete == size ? parts : parts + 1;
        }

    }
}