using AutoMapper;
using TechChallenge.Fase3.Domain.Contatos.Comandos;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;

namespace TechChallenge.Fase3.Consumer.ContatoServices
{
    public class EditarContato(ILogger<EditarContato> logger, IContatosRepositorio contatosRepositorio, IMapper mapper, IMensageriaBus mensageriaBus)
    {
        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // return mensageriaBus.Bus.PubSub.SubscribeAsync<ContatoComando>("QueueEditar", EditarContatoAsync, x => x.WithTopic(TopicosRabbit.Editar), cancellationToken);
            }
            return Task.CompletedTask;
        }

        private async Task EditarContatoAsync(ContatoComando comando, CancellationToken cancellationToken)
        {
            Contato contatoEdicao = mapper.Map<Contato>(comando);
            await contatosRepositorio.AtualizarContatoAsync(contatoEdicao, cancellationToken);
            logger.LogInformation("Contato Editado ID:" + contatoEdicao.Id.Value);
        }
    }
}
