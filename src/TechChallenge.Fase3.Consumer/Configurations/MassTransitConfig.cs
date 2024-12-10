using MassTransit;
using TechChallenge.Fase3.Consumer.Eventos;

namespace TechChallenge.Fase3.Consumer.Configurations
{
    public static class MassTransitConfig
    {
        public static void Configure(IServiceCollection services, BusConfig busConfig)
        {
            services.AddMassTransit(x =>
            {
                // Configura os consumidores
                x.AddConsumer<InserirContatoConsumer>();
                x.AddConsumer<RemoverContatoConsumer>();
                x.AddConsumer<EditarContatoConsumer>();
                // Configura o RabbitMQ ou outro transporte
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(busConfig.Servidor, h =>
                    {
                        h.Username(busConfig.Usuario);
                        h.Password(busConfig.Senha);
                    });

                    cfg.ReceiveEndpoint(busConfig.NomeFilaInsercao, e =>
                    {
                        e.ConfigureConsumer<InserirContatoConsumer>(context);
                        e.Bind(busConfig.NomeExchange, x =>
                        {
                            x.ExchangeType = "topic";
                            x.RoutingKey = "Contato.Inserir";
                        });
                    });

                    cfg.ReceiveEndpoint(busConfig.NomeFilaRemover, e =>
                    {
                        e.ConfigureConsumer<RemoverContatoConsumer>(context);
                        e.Bind(busConfig.NomeExchange, x =>
                        {
                            x.ExchangeType = "topic";
                            x.RoutingKey = "Contato.Remover";
                        });
                    });

                    cfg.ReceiveEndpoint(busConfig.NomeFilaEdicao, e =>
                    {
                        e.ConfigureConsumer<EditarContatoConsumer>(context);
                        e.Bind(busConfig.NomeExchange, x =>
                        {
                            x.ExchangeType = "topic";
                            x.RoutingKey = "Contato.Editar";
                        });
                    });
                });
            });
        }
    }
}
