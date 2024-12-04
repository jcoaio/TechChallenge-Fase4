using AutoMapper;
using TechChallenge.Fase3.Domain.Contatos.Comandos;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;
using TechChallenge.Fase3.Domain.Utils;

namespace TechChallange.Fase3.Consumer.ContatoServices
{
    public class InserirContato(ILogger<InserirContato> logger, IContatosRepositorio contatosRepositorio, IMapper mapper, IMensageriaBus mensageriaBus)
    {
        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                return mensageriaBus.Bus.PubSub.SubscribeAsync<ContatoComando>("QueueInserir", InserirContatoAsync, x => x.WithTopic(TopicosRabbit.Inserir), cancellationToken);
            }
            return Task.CompletedTask;
        }

        public async Task InserirContatoAsync(ContatoComando comando, CancellationToken cancellationToken)
        {
            Contato contatoInserir = mapper.Map<Contato>(comando);
            Contato contato = await contatosRepositorio.InserirContatoAsync(contatoInserir, cancellationToken);
            logger.LogInformation("Contato Inserido: ID:" + contato.Id);
        }
    }
}
