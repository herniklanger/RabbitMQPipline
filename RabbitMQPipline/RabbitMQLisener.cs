using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQPipline
{
    class RabbitMQLisener : IDisposable
    {
        public IConnection Connection { get; set; }
        public List<IModel> Models { get; set; }
        List<Exchanges> exchanges { get; set; }
        IServiceScopeFactory _scopeProvider { get; set; }
        public RabbitMQLisener(ConnectionFactory factory, List<Exchanges> exchanges, IServiceScopeFactory serviceProvider)
        {
            Connection = factory.CreateConnection();
            Models = new List<IModel>();
            exchanges = exchanges;
        }
        public void Connect()
        {
            foreach (Exchanges exchanges in exchanges)
            {
                IModel channel = Connection.CreateModel();
                Models.Add(channel);

                channel.ExchangeDeclare(exchanges.FromName, ExchangeType.Direct);
                string queueName = channel.QueueDeclare(exchanges.FromName + "-PipLine", false, false, true, null).QueueName;
                channel.QueueBind(queueName, exchanges.FromName, "");
                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    using(IServiceScope s = _scopeProvider.CreateScope())
                    {
                        var Collection = s.ServiceProvider.GetService<IServiceCollection>();
                        Collection.AddSingleton(ea.BasicProperties);
                        Collection.AddSingleton(ea);

                    }
                    Console.WriteLine("Message resived Event: " + exchanges.FromName);

                };
                channel.BasicConsume(queueName, true, consumer);
        }
        }
        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
