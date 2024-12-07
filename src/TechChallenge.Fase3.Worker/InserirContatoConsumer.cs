using AutoMapper;
using MassTransit;
using TechChallenge.Fase3.Domain.Contatos.Comandos;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios;

namespace TechChallenge.Fase3.Consumer.ContatoServices
{
    public class InserirContatoConsumer(ILogger<InserirContatoConsumer> logger, IContatosRepositorio contatosRepositorio, IMapper mapper) : IConsumer<ContatoComando>
    {
        public async Task Consume(ConsumeContext<ContatoComando> context)
        {
            Contato contatoInserir = mapper.Map<Contato>(context.Message);
            Contato contato = await contatosRepositorio.InserirContatoAsync(contatoInserir, context.CancellationToken);
            logger.LogInformation("Contato Inserido: ID:" + contato.Id);
        }
    }
}
