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
        public BasicDeliverEventArgs Message { get; set; }
        public Auth(BasicDeliverEventArgs message)
        {
            Message = message;
        }
        //If it return false it will end
        public bool Run()
        {
            Console.WriteLine("Auth message");
            return true;
        }
    }
}
