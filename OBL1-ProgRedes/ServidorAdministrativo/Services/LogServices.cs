using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.IO;
using System.Text;

namespace ServidorAdministrativo.Services
{
    public class LogServices
    {
        private string _queueName;
        private IModel _canal;
        private string hostName;

        public LogServices()
        {
            IConfiguration configuracion = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false).Build();

            this.hostName = configuracion.GetSection("Queue:Hostname").Value;
            this._queueName = configuracion.GetSection("Queue:Name").Value;
            _canal = new ConnectionFactory { HostName = this.hostName }.CreateConnection().CreateModel();
            QueueDeclare(_canal);
        }

        public void SendMessages(string log)
        {
            string logAEnviar = log + " " + DateTime.Now.ToString("dd-MM-yyyy");

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
