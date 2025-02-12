using MassTransit;
using TechChallenge.Fase3.Consumer.Eventos;

namespace TechChallenge.Fase3.Consumer.Configurations
{
    public static class MassTransitConfig
    {
        public static void ConfigurarMassTransit(HostApplicationBuilder builder, BusConfig mensageriaConfig)
        {
            builder.Services.AddMassTransit(x =>
            {
                // Configura os consumidores
                x.AddConsumer<InserirContatoConsumer>();
                x.AddConsumer<RemoverContatoConsumer>();
                x.AddConsumer<EditarContatoConsumer>();

                // Configura o RabbitMQ ou outro transporte
                x.UsingRabbitMq((context, cfg) =>
                {

                    cfg.Host(mensageriaConfig.Servidor, h =>
                    {
                        h.Username(mensageriaConfig.Usuario);
                        h.Password(mensageriaConfig.Senha);
                    });

                    cfg.ReceiveEndpoint(mensageriaConfig.NomeFilaInsercao, e =>
                    {
                        e.ConfigureConsumer<InserirContatoConsumer>(context);
                    });
                    cfg.ReceiveEndpoint(mensageriaConfig.NomeFilaRemover, e =>
                    {
                        e.ConfigureConsumer<RemoverContatoConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(mensageriaConfig.NomeFilaEdicao, e =>
                    {
                        e.ConfigureConsumer<EditarContatoConsumer>(context);
                    });
                });
            });
        }
    }
}
