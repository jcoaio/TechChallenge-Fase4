using AutoMapper;
using MassTransit;
using TechChallenge.Fase3.Domain.Contatos.Comandos;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;

namespace TechChallenge.Fase3.Consumer.Eventos
{
    public class EditarContatoConsumer(IContatosServico contatosServico, IMapper mapper) : IConsumer<ContatoComando>
    {
        public async Task Consume(ConsumeContext<ContatoComando> context)
        {
            Contato contatoEdicao = mapper.Map<Contato>(context.Message);
            await contatosServico.AtualizarContatoAsync(contatoEdicao);
            Console.WriteLine($"Contato Editado: Email:{contatoEdicao.Email}, RID:{context.RequestId}");
            Task.CompletedTask.Wait();
        }
    }
}
