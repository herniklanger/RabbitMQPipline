using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace RabbitMQPipline
{
    public class Startup : Start
    {

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<List<IBasicProperties>>();
        }

    }
}
