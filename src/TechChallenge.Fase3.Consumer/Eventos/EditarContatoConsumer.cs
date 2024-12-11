using AutoMapper;
using MassTransit;
using TechChallenge.Fase3.Domain.Contatos.Comandos;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios;

namespace TechChallenge.Fase3.Consumer.Eventos
{
    public class EditarContatoConsumer(IContatosRepositorio contatosRepositorio, IMapper mapper) : IConsumer<ContatoComando>
    {
        public async Task Consume(ConsumeContext<ContatoComando> context)
        {
            try
            {
                Contato contatoEdicao = mapper.Map<Contato>(context.Message);
                await contatosRepositorio.AtualizarContatoAsync(contatoEdicao, context.CancellationToken);
                Console.WriteLine($"Contato Editado: Email:{contatoEdicao.Email}, RID:{context.RequestId}");
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
