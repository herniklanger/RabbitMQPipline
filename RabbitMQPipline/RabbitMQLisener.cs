using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQPipline.Filter;
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
        List<Exchange> _exchanges { get; set; }
        IServiceScopeFactory _scopeProvider { get; set; }
        public RabbitMQLisener(ConnectionFactory factory, List<Exchange> exchanges, IServiceScopeFactory serviceProvider)
        {
            Models = new List<IModel>();
            Connection = factory.CreateConnection();
            _exchanges = exchanges;
            _scopeProvider = serviceProvider;
        }
        public void Connect()
        {
            foreach (Exchange exchanges in _exchanges)
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
                        RunFilters(ea, s);
                    }
                    Console.WriteLine("Message resived Event: " + exchanges.FromName);

                };
                channel.BasicConsume(queueName, true, consumer);
            }
        }
        void RunFilters(BasicDeliverEventArgs ea, IServiceScope s)
        {
            var container = s.ServiceProvider.GetService<MessageContainer>();
            container.Message = ea;
            container.Heatter = ea.BasicProperties;
            foreach (IFilter filter in s.ServiceProvider.GetService<List<IFilter>>())
            {
                if (!filter.Settings.TaksRequerd)
                {
                    filter.Run();
                }else if(null != filter.Settings.Tags.Find(y => ((string[])ea.BasicProperties.Headers["Filter"]).Contains(y)))
                {
                    filter.Run();
                }
            }
        }
        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
