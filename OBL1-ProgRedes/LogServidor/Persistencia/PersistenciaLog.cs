using System.Collections.Generic;

namespace LogServidor.Persistencia
{
    public class PersistenciaLog
    {
        private static PersistenciaLog persistencia;
        private List<LogModelo> logsModel;
        private List<string> log;

        public PersistenciaLog() {
            this.logsModel = new List<LogModelo>();
            this.log = new List<string>();
        }

        public static PersistenciaLog ObtenerPersistencia()
        {
            if (persistencia == null)
            {
                persistencia = new PersistenciaLog();
            }

            return persistencia;
        }

        public void AgregarLog(string log)
        {
            this.log.Add(log);
        }

        public void AgregarLogAux(string nombreUsuario, string nombreJuego, string log)
        {
            this.logsModel.Add(new LogModelo(nombreUsuario, nombreJuego, log));
        }

        public List<string> ObtenerLogs(string fecha, string usuario, string juego)
        {
            List<string> logsFiltrados = new List<string>();

            if (fecha == "" && usuario == "" && juego == "") {
                this.logsModel.ForEach(x => logsFiltrados.Add(x.Log));
                return logsFiltrados;
            }

            foreach (LogModelo log  in this.logsModel)
                if(log.Fecha == fecha || log.NombreUsuario == usuario || log.NombreJuego == juego)
                    logsFiltrados.Add(log.Log);

            return logsFiltrados;
        }
    }
}
