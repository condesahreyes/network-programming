using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorAdministrativo.Services
{
    public class LogServices
    {
        private const string _queueName = "logQueue";
        private IModel _canal;

        public LogServices()
        {
            _canal = new ConnectionFactory { HostName = "localhost" }.CreateConnection().CreateModel();
            QueueDeclare(_canal);
        }

        public void SendMessages(string log)
        {
            string logAEnviar = DateTime.Now.ToString("dd-MM-yyyy") + "/" + log;

            PublishMessage(logAEnviar, this._canal);
        }

        private void QueueDeclare(IModel channel)
        {
            channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void PublishMessage(string message, IModel channel)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: string.Empty, routingKey: _queueName, basicProperties: null, body: body);
            }
        }
    }
}
