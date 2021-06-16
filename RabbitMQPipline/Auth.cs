using RabbitMQ.Client.Events;
using RabbitMQPipline.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQPipline
{
    class Auth : IFilter
    {
        public FilterSettings Settings { get; set; }
        public IServiceProvider service { get; set; }

        //If it return false it will end
        public void Run()
        {
            Console.WriteLine("Auth message");
        }
    }
}
