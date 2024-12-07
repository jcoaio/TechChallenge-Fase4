using MassTransit;
using TechChallenge.Fase3.Domain.Contatos.Comandos;

namespace TechChallenge.Fase3.Consumer.Eventos
{
    public class InserirContatoConsumer : IConsumer<ContatoComando>
    {
        public Task Consume(ConsumeContext<ContatoComando> context)
        {
            Console.WriteLine(context.Message.Email);
            return Task.CompletedTask;
        }
    }
}
