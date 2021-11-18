using System;

namespace LogServidor
{
    public class LogModelo
    {
        public string NombreUsuario { get; set; }
        public string NombreJuego { get; set; }
        public string Fecha { get; set; }
        public string Log { get; set; }

        public LogModelo(string nombreUsuario, string nombreJuego, string log) {
            this.NombreUsuario = nombreUsuario;
            this.NombreJuego = nombreJuego;
            this.Fecha = DateTime.Now.ToString("dd-MM-yyyy");
            this.Log = log;
        }
    }
}
