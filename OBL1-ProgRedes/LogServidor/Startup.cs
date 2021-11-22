using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using LogServidor.Persistencia;
using RabbitMQ.Client;
using LogServidor.Controllers;

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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            persistenciaLog = PersistenciaLog.ObtenerPersistencia();

            var colaHosname = Configuration.GetSection("Queue:Hostname").Value;
            var nombreCola = Configuration.GetSection("Queue:Name").Value;

            var factory = new ConnectionFactory() { HostName = colaHosname };
            IConnection conexion = factory.CreateConnection();
            IModel canal = conexion.CreateModel();

            canal.QueueDeclare(nombreCola, false , false, false, null);
            ColaServicio colaServicio = new ColaServicio(canal, persistenciaLog, nombreCola);
            services.AddSingleton<ColaServicio>(colaServicio);
        }
        
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
