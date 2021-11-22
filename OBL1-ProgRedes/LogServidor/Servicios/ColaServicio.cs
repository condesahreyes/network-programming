using LogServidor.Persistencia;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System;

namespace LogServidor
{
    public class ColaServicio
    {
        private readonly IModel _canal;
        private PersistenciaLog _persistenciaLog;
        private string _colaNombre;

        public ColaServicio(IModel canal, PersistenciaLog persisteniaLog, string colaNombre)
        {
            this._canal = canal;
            this._persistenciaLog = persisteniaLog;
            this._colaNombre = colaNombre;
            RecibirLogs();
        }

        private void RecibirLogs()
        {
            var consumidor = new EventingBasicConsumer(this._canal);

            Task.Run(() =>
            {
                consumidor.Received += (model, ea) =>
                {
                    var cuerpoMensaje = ea.Body.ToArray();
                    var mensaje = Encoding.UTF8.GetString(cuerpoMensaje);
                    this._persistenciaLog.AgregarLog(mensaje);
                    Console.WriteLine(mensaje);
                };

                this._canal.BasicConsume(
                                queue: this._colaNombre,
                                autoAck: true,
                                consumer: consumidor);
            });

        }
    }
}
