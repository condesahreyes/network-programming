using System;
using System.Collections.Generic;

namespace LogServidor.Persistencia
{
    public class PersistenciaLog
    {
        private static PersistenciaLog persistencia;
        private List<string> logs;

        public PersistenciaLog() {
            this.logs = new List<string>();
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
            this.logs.Add(log);
        }

        public List<string> ObtenerLogs(string fecha, string usuario, string juego)
        {
            if (fecha == null && usuario == null && juego == null)
                return this.logs;

            if(usuario!= null)
                usuario = usuario.ToLower();

            if(juego!=null)
                juego = juego.ToLower();

            List<string> logsFiltrados = new List<string>();
            

            foreach (string log  in this.logs)
            {
                string logLower = log.ToLower();
                bool filtrarPorUsuario = usuario == null && log.Split(" ")[0].Equals("usuario") && usuario!="" && logLower.Contains(usuario);
                bool filtrarPorJuego = juego != null && log.Split(" ")[0].Equals("juego") && juego!="" && logLower.Contains(juego);
                bool filtrarPorFecha = fecha != "" && fecha != null && logLower.Contains(fecha);

                if (filtrarPorUsuario || filtrarPorJuego || filtrarPorFecha)
                    logsFiltrados.Add(log);
            }

            return logsFiltrados;
        }

    }
}
