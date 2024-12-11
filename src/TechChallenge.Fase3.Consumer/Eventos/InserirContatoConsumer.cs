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

            try
            {
                Contato contato = mapper.Map<Contato>(context.Message);
                Contato response = await contatosRepositorio.InserirContatoAsync(contato, context.CancellationToken);
                Console.WriteLine($"Contato Inserido: Email:{response.Email}, RID:{context.RequestId}");
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
