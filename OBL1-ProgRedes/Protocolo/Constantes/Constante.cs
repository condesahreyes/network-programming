namespace Protocolo
{
    public class Constante
    {
        public static int largoComando = 1;
        public static int largoDato = 4;
        public static int largoEncabezado = largoComando + largoDato + 1;
        public static int largoHash = 32;
        public static int maximoTamañoDePaquete = 32768;

        public const string MensajeOk = "Ok";
        public const string MensajeError = "FAIL";
    }
}
