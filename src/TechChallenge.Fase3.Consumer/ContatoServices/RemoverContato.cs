using AutoMapper;
using TechChallenge.Fase3.Domain.Contatos.Comandos;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;
using TechChallenge.Fase3.Domain.Utils;

namespace TechChallange.Fase3.Consumer.ContatoServices
{
    public class RemoverContato(ILogger<RemoverContato> logger, IContatosRepositorio contatosRepositorio, IMapper mapper, IMensageriaBus mensageriaBus)
    {
        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Escutando...");
                return mensageriaBus.Bus.PubSub.SubscribeAsync<ContatoComando>("QueueRemover", RemoverContatoAsync, x => x.WithTopic(TopicosRabbit.Remover), cancellationToken);
            }

            return Task.CompletedTask;
        }

        private async Task RemoverContatoAsync(ContatoComando comando, CancellationToken cancellationToken)
        {
            Contato contatoRemover = mapper.Map<Contato>(comando) ?? throw new InvalidDataException("Tantiva de remover um contato nulo ou inválido.");
            await contatosRepositorio.RemoverContatoAsync(contatoRemover.Id.Value, cancellationToken);
            logger.LogInformation("Contato Removido ID:" + contatoRemover.Id.Value);
        }
    }
}
