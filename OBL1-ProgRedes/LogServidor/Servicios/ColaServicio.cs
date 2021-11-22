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
        private string _queueName;

        public ColaServicio(IModel canal, PersistenciaLog persisteniaLog, string queueName)
        {
            this._canal = canal;
            this._persistenciaLog = persisteniaLog;
            this._queueName = queueName;
            RecibirLogs();
        }

        private void RecibirLogs()
        {
            var consumer = new EventingBasicConsumer(this._canal);

            Task.Run(() =>
            {
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    this._persistenciaLog.AgregarLog(message);
                    Console.WriteLine(message);
                };

                this._canal.BasicConsume(
                                queue: this._queueName,
                                autoAck: true,
                                consumer: consumer);
            });

        }
    }
}
