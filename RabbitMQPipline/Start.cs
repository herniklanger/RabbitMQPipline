using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQPipline.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text.Json;
using RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RabbitMQPipline
{
    public abstract class Start
    {
        internal IHost host { get; set; }
        public bool Running { get; set; } = true;
        internal Thread thread { get; set; }
        List<Exchange> _exchanges { get; set; }
        public void Run(string[] args)
        {
            IHostBuilder hostTemp = CreateHostBuilder(args);
            _exchanges = JsonSerializer.Deserialize<List<Exchange>>(File.ReadAllText(Environment.CurrentDirectory + "\\Filter\\PiplineConfig.json"));
            hostTemp.ConfigureServices(services =>
            {
                services.AddScoped<MessageContainer>();
                services.AddSingleton(this);
                services.AddSingleton(_exchanges ?? new List<Exchange>());
                services.AddScoped(sp => {
                    string settings = File.ReadAllText(Environment.CurrentDirectory + "\\Filter\\FilterConfig.json");
                    List<FilterSettings> filterSettings = JsonSerializer.Deserialize<List<FilterSettings>>(settings);
                    List<IFilter> filters = new List<IFilter>();
                    List<Type> typs = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => typeof(IFilter).IsAssignableFrom(p) && !p.IsInterface).ToList();
                    foreach(var type in typs)
                    {
                        IFilter filter = (IFilter)Activator.CreateInstance(type);
                        filter.Settings = filterSettings.Find(x => x.FilterName == type.Name);
                        if (filter.Settings != null)
                        {
                            filters.Add(filter);
                        }
                    }
                    return filters;
                });
                services.TryAddSingleton(x => new ConnectionFactory() { HostName = "localhost" });
                services.AddSingleton<RabbitMQLisener>();
                ConfigureServices(services);

            });
            host = hostTemp.Build();
            StartServices(host.Services);
            while (Running)
            {
                Thread.Sleep(1);
            }
        }
        internal static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args);
        }
        public abstract void ConfigureServices(IServiceCollection services);
        internal virtual void StartServices(IServiceProvider provider)
        {
            var test = provider.GetService<RabbitMQLisener>();
            test.Connect();
        }
    }
}
