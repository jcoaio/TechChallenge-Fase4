using AutoMapper;
using TechChallenge.Fase3.Domain.Contatos.Comandos;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;
using TechChallenge.Fase3.Domain.Utils;

namespace ContatoWorker.ContatoServices
{
    public class InserirContato(ILogger<InserirContato> logger, IContatosRepositorio contatosRepositorio, IMapper mapper, IMensageriaBus mensageriaBus)
    {
        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                return mensageriaBus.Bus.PubSub.SubscribeAsync<ContatoComando>("QueueInserir", InserirContatoAsync, x => x.WithTopic(TopicosRabbit.Inserir), cancellationToken);
                //await mensageriaBus.Bus.PubSub.SubscribeAsync<ContatoComando>("QueueRemover", RemoverContatoAsync, x => x.WithTopic(TopicosRabbit.Remover), cancellationToken);
                //await mensageriaBus.Bus.PubSub.SubscribeAsync<ContatoComando>("QueueEditar", EditarContatoAsync, x => x.WithTopic(TopicosRabbit.Editar), cancellationToken);
            }
            return Task.CompletedTask;
        }

        private async Task EditarContatoAsync(ContatoComando comando, CancellationToken cancellationToken)
        {
            Contato contatoEdicao = mapper.Map<Contato>(comando);
            await contatosRepositorio.AtualizarContatoAsync(contatoEdicao, cancellationToken);
            logger.LogInformation("Contato Editado ID:" + contatoEdicao.Id.Value);
        }

        private async Task RemoverContatoAsync(ContatoComando comando, CancellationToken cancellationToken)
        {
            Contato contatoRemover = mapper.Map<Contato>(comando) ?? throw new InvalidDataException("Tantiva de remover um contato nulo ou inválido.");
            await contatosRepositorio.RemoverContatoAsync(contatoRemover.Id.Value, cancellationToken);
            logger.LogInformation("Contato Removido ID:" + contatoRemover.Id.Value);
        }

        public async Task InserirContatoAsync(ContatoComando comando, CancellationToken cancellationToken)
        {
            Contato contatoInserir = mapper.Map<Contato>(comando);
            Contato contato = await contatosRepositorio.InserirContatoAsync(contatoInserir, cancellationToken);
            logger.LogInformation("Contato Inserido: ID:" + contato.Id);
        }
    }
}
