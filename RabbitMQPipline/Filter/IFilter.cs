using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQPipline.Filter
{
    interface IFilter
    {
        IServiceProvider service { get; set; }
        FilterSettings Settings { get; set; }
        public bool Run();
    }
}
