using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQPipline
{
    public abstract class Start
    {
        internal IHostBuilder host { get; set; }
        public bool Running { get; set; } = true;
        internal Thread thread { get; set; }
        public void Run(string[] args)
        {
            host = CreateHostBuilder(args);
            host.ConfigureServices(services =>
            {
                services.AddSingleton(this);
                ConfigureServices(services);
            });
            
            //while (Running)
            //{
            //    Thread.Sleep(1);
            //}
        }
        internal static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args);
        }
        public abstract void ConfigureServices(IServiceCollection services);
        internal abstract void StartServices(IServiceProvider provider);
    }
}
