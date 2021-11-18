using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using LoggServidor.Persistencia;
using Microsoft.Extensions.Logging;

namespace LoggServidor.Services
{
    public class LogService : LoggerManejador.LoggerManejadorBase
    {
        private readonly ILogger<LogService> logger;

        public LogService(ILogger<LogService> logger)
        {
            this.logger = logger;
        }

        public override Task<LogsRespuesta> ObtenerLogs(Filtros request, ServerCallContext context)
        {
            var logsRespuesta = new LogsRespuesta();

            List<string> logs = PersistenciaLog.ObtenerPersistencia().
                ObtenerLogs(request.Fecha, request.Usuario, request.Juego);

            logsRespuesta.Logs.AddRange(logs);

            return Task.FromResult(logsRespuesta);
        }
    }
}
