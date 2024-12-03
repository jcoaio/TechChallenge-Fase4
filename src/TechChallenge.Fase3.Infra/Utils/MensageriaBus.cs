using EasyNetQ;
using Microsoft.Extensions.Configuration;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;
namespace TechChallenge.Fase3.Infra.Utils
{
    public class MensageriaBus : IMensageriaBus
    {
        private readonly string connectionString;
        public IBus Bus { get; set; }

        public MensageriaBus(string connectionString)
        {
            this.connectionString = connectionString;
            Bus = RabbitHutch.CreateBus(connectionString);
        }
        public MensageriaBus(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("rabbitmq")!;
            Bus = RabbitHutch.CreateBus(connectionString);
        }
    }
}
