using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace TechChallenge.Fase3.Domain.Utils
{
    public class Rabbit
    {
        public IBus Bus { get; set; }
        public Rabbit()
        {
            Bus = RabbitHutch.CreateBus("amqp://techchallangeapi:123@lhserver:5672/");
        }
    }
}
