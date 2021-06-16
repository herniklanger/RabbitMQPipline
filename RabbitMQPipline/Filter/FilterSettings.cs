using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQPipline.Filter
{
    class FilterSettings
    {
        public string FilterName { get; set; }
        public bool TaksRequerd { get; set; }
        public List<string> Tags { get; set; }
    }
}
