using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQPipline
{
    class MessageContainer
    {
        public IBasicProperties Heatter { get; set; }
        public BasicDeliverEventArgs Message { get; set; }
    }
}
