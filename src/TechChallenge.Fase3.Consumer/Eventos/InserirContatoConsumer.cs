using AutoMapper;
using MassTransit;
using TechChallenge.Fase3.Domain.Contatos.Comandos;
using TechChallenge.Fase3.Domain.Contatos.Repositorios.Filtros;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;

namespace TechChallenge.Fase3.Consumer.Eventos
{
    public class InserirContatoConsumer(IContatosServico contatosServico, IMapper mapper) : IConsumer<ContatoComando>
    {
        public async Task Consume(ConsumeContext<ContatoComando> context)
        {
            ContatoFiltro contatoInserir = mapper.Map<ContatoFiltro>(context.Message);
            await contatosServico.InserirContatoAsync(contatoInserir);
            Console.WriteLine($"Contato Inserido: Email:{contatoInserir.Email}, RID:{context.RequestId}");
            Task.CompletedTask.Wait();
        }
    }
}
