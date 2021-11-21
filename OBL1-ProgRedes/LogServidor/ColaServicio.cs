using LogServidor.Persistencia;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

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

                this._canal.BasicConsume(_queueName, true, consumer);
            });
        }
    }
}
