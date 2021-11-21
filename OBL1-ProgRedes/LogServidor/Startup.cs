using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using RabbitMQ.Client;
using LogServidor.Persistencia;

namespace LogServidor
{
    public class Startup
    {
        PersistenciaLog persistenciaLog;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            persistenciaLog = PersistenciaLog.ObtenerPersistencia();

            var queueHostname = Configuration.GetSection("Queue:Hostname").Value;
            var queuePort = Int32.Parse(Configuration.GetSection("Queue:Port").Value);
            var queueUsername = Configuration.GetSection("Queue:Username").Value;
            var queuePassword = Configuration.GetSection("Queue:Password").Value;
            var queueName = Configuration.GetSection("Queue:Name").Value;

            var factory = new ConnectionFactory() { HostName = queueHostname };

            using IConnection conexion = factory.CreateConnection();
            using IModel canal = conexion.CreateModel();

            canal.QueueDeclare(queueName, false , false, false, null);
            ColaServicio queueService = new ColaServicio(canal, persistenciaLog, queueName);
            services.AddSingleton<ColaServicio>(queueService);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipelinRabbitMQ.Client.Exceptions.BrokerUnreachableException: 'None of the specified endpoints were reachable'e.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
