using MassTransit;
using Prometheus;
using TechChallenge.Fase3.Consumer.Configurations;
using TechChallenge.Fase3.Consumer.Eventos;
using TechChallenge.Fase3.Domain.Contatos.Repositorios;
using TechChallenge.Fase3.Domain.Contatos.Servicos;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;
using TechChallenge.Fase3.Infra.Contatos;
using TechChallenge.Fase3.Infra.Utils;
using TechChallenge.Fase3.Infra.Utils.DBContext;

namespace TechChallenge.Fase3.Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Services.Configure<BusConfig>(builder.Configuration.GetSection("Mensageria"));
            BusConfig mensageriaConfig = builder.Configuration.GetSection("Mensageria").Get<BusConfig>() ?? throw new FormatException("appSettings:Mensageria - Invalid JSON");

            builder.Services.AddHostedService<Worker>();
            builder.Services.AddScoped<IContatosServico, ContatosServico>();
            builder.Services.AddScoped<IContatosRepositorio, ContatosRepositorio>();
            builder.Services.AddSingleton<IMensageriaBus, MensageriaBus>();
            builder.Services.AddTransient<DapperContext>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
                        e.Bind(mensageriaConfig.NomeExchange, x =>
                        {
                            x.ExchangeType = "topic";
                            x.RoutingKey = "Contato.Inserir";
                        });
                    });

                    cfg.ReceiveEndpoint(mensageriaConfig.NomeFilaRemover, e =>
                    {
                        e.ConfigureConsumer<RemoverContatoConsumer>(context);
                        e.Bind(mensageriaConfig.NomeExchange, x =>
                        {
                            x.ExchangeType = "topic";
                            x.RoutingKey = "Contato.Remover";
                        });
                    });

                    cfg.ReceiveEndpoint(mensageriaConfig.NomeFilaEdicao, e =>
                    {
                        e.ConfigureConsumer<EditarContatoConsumer>(context);
                        e.Bind(mensageriaConfig.NomeExchange, x =>
                        {
                            x.ExchangeType = "topic";
                            x.RoutingKey = "Contato.Editar";
                        });
                    });
                });
            });

            using KestrelMetricServer server = new(port: 1234);
            server.Start();

            IHost host = builder.Build();
            host.Run();
        }

    }
}

