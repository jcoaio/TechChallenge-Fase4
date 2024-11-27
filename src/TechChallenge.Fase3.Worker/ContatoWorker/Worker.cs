using AutoMapper;
using EasyNetQ;
using TechChallenge.Fase3.Domain.Contatos.Comandos;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios;
using TechChallenge.Fase3.Domain.Utils;

namespace ContatoWorker
{
    public class Worker(ILogger<Worker> logger, IContatosRepositorio contatosRepositorio, IMapper mapper) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using IBus bus = RabbitHutch.CreateBus("amqp://techchallangeapi:123@lhserver:5672/");
                await bus.PubSub.SubscribeAsync<ContatoInserirComando>(TopicosRabbit.Inserir, InserirContatoAsync, cancellationToken: stoppingToken);
            }
        }

        public async Task InserirContatoAsync(ContatoInserirComando contatoInserirComando)
        {
            try
            {
                Contato contato = mapper.Map<Contato>(contatoInserirComando);
                Contato response = await contatosRepositorio.InserirContatoAsync(contato);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Erro ao processo item da fila.");
            }
        }
    }
}
