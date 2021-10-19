namespace Protocolo
{
    public class Encabezado
    {
        public int largoMensaje;
        public string accion;

        public Encabezado(int largoMensaje, string accion)
        {
            this.largoMensaje = largoMensaje;
            this.accion = accion;
        }
    }
}
