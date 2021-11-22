using LogServidor.Persistencia;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LogServidor.Controllers
{
    [Route("logs")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private PersistenciaLog persistencia;
        public LogsController()
        {
            this.persistencia = PersistenciaLog.ObtenerPersistencia();
        }

        [HttpGet]
        public ActionResult ObtenerLogs([FromQuery(Name ="usuario")] string usuario, 
            [FromQuery(Name = "juego")] string juego, [FromQuery(Name = "fecha")] string fecha)
        {
            List<string> logs = persistencia.ObtenerLogs(fecha, usuario, juego);
            return Ok(logs);
        }
    }
}
