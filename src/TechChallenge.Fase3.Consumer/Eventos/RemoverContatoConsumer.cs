using AutoMapper;
using MassTransit;
using TechChallenge.Fase3.Domain.Contatos.Comandos;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios;

namespace TechChallenge.Fase3.Consumer.Eventos
{
    public class RemoverContatoConsumer(IContatosRepositorio contatosRepositorio, IMapper mapper) : IConsumer<ContatoComando>
    {
        public async Task Consume(ConsumeContext<ContatoComando> context)
        {
            try
            {
                Contato contatoRemover = mapper.Map<Contato>(context.Message);
                await contatosRepositorio.RemoverContatoAsync(contatoRemover.Id.Value, context.CancellationToken);
                Console.WriteLine($"Contato Removido: ID:{context.Message.Id}, RID:{context.RequestId}");
                Task.CompletedTask.Wait();
            }
            catch (Exception)
            {
                Task.CompletedTask.Wait();
                throw;
            }
        }
    }
}
