namespace Protocolo
{
    public class Constante
    {
        public static int largoComando = 1;
        public static int largoDato = 4;
        public static int largoEncabezado = largoComando + largoDato + 1;
        public static int maximoTamañoDePaquete = 32768;

        public const int LargoNombreArchivo = 4;
        public const int LargoArchivo = 8;

        public const string MensajeOk = "Ok";
        public const string MensajeError = "FAIL";
        public const string MensajeObjetoEliminado = "Elim";
    }
}