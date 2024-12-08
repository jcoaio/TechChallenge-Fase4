using AutoMapper;
using MassTransit;
using TechChallenge.Fase3.Domain.Contatos.Comandos;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;

namespace TechChallenge.Fase3.Consumer.Eventos
{
    public class RemoverContatoConsumer(IContatosServico contatosServico, IMapper mapper) : IConsumer<ContatoComando>
    {
        public async Task Consume(ConsumeContext<ContatoComando> context)
        {
            Contato contatoRemover = mapper.Map<Contato>(context.Message);
            await contatosServico.RemoverContatoAsync(contatoRemover.Id.Value);
            Console.WriteLine($"Contato Removido: ID:{context.Message.Id}, RID:{context.RequestId}");
        }
    }
}
