using AutoMapper;
using MassTransit;
using TechChallenge.Fase3.Domain.Contatos.Comandos;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios;

namespace TechChallenge.Fase3.Consumer.Eventos
{
    public class InserirContatoConsumer(IContatosRepositorio contatosRepositorio, IMapper mapper) : IConsumer<ContatoComando>
    {
        public async Task Consume(ConsumeContext<ContatoComando> context)
        {
            Contato contatoInserir = mapper.Map<Contato>(context.Message);
            Contato contato = await contatosRepositorio.InserirContatoAsync(contatoInserir, context.CancellationToken);
            Console.WriteLine($"Contato Inserido: ID:{contato.Id}, RID:{context.RequestId}");
            Task.CompletedTask.Wait();
        }
    }
}
