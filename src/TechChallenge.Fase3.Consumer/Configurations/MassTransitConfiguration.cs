using MassTransit;
using TechChallenge.Fase3.Consumer.Eventos;

namespace TechChallenge.Fase3.Consumer.Configurations
{
    public static class MassTransitConfiguration
    {
        public static void Configure(IServiceCollection services, BusConfiguration busConfig)
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
                    });

                    cfg.ReceiveEndpoint(busConfig.NomeFilaRemover, e =>
                    {
                        e.ConfigureConsumer<RemoverContatoConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(busConfig.NomeFilaEdicao, e =>
                    {
                        e.ConfigureConsumer<EditarContatoConsumer>(context);
                    });
                });
            });
        }
    }
}
