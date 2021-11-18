using System.Collections.Generic;

namespace LoggServidor.Persistencia
{
    public class PersistenciaLog
    {
        private static PersistenciaLog persistencia;

        private List<LogModelo> logs;

        public PersistenciaLog() {
            this.logs = new List<LogModelo>();
        }

        public static PersistenciaLog ObtenerPersistencia()
        {
            if (persistencia == null)
            {
                persistencia = new PersistenciaLog();
            }

            return persistencia;
        }

        public void AgregarLog(string nombreUsuario, string nombreJuego, string log)
        {
            this.logs.Add(new LogModelo(nombreUsuario, nombreJuego, log));
        }

        public List<string> ObtenerLogs(string fecha, string usuario, string juego)
        {
            List<string> logsFiltrados = new List<string>();

            if (fecha == "" && usuario == "" && juego == "") {
                this.logs.ForEach(x => logsFiltrados.Add(x.Log));
                return logsFiltrados;
            }

            foreach (LogModelo log  in this.logs)
                if(log.Fecha == fecha || log.NombreUsuario == usuario || log.NombreJuego == juego)
                    logsFiltrados.Add(log.Log);

            return logsFiltrados;
        }
    }
}
