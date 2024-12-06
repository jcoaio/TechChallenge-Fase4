using EasyNetQ;
using EasyNetQ.Topology;
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
            Exchange exchange = Bus.Advanced.ExchangeDeclare("techchallenge", ExchangeType.Direct, durable: true, autoDelete: false);

            Queue queueInserir = Bus.Advanced.QueueDeclare("QueueInserir", durable: true, exclusive: false, autoDelete: false);
            Queue queueRemover = Bus.Advanced.QueueDeclare("QueueRemover", durable: true, exclusive: false, autoDelete: false);
            Queue queueEditar = Bus.Advanced.QueueDeclare("QueueEditar", durable: true, exclusive: false, autoDelete: false);

            Bus.Advanced.Bind(exchange, queueInserir, "Contato.Inserir");
            Bus.Advanced.Bind(exchange, queueRemover, "Contato.Remover");
            Bus.Advanced.Bind(exchange, queueEditar, "Contato.Editar");

            Console.WriteLine("BUS RABBIT CRIADO: " + connectionString);
        }
    }
}
