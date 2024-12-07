using MassTransit;
using TechChallenge.Fase3.Consumer.Eventos;
using TechChallenge.Fase3.Domain.Contatos.Repositorios;
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


            ConfigurationManager configurationManager = builder.Configuration;


            string servidor = configurationManager.GetSection("Mensageria")["Servidor"] ?? string.Empty;
            string usuario = configurationManager.GetSection("Mensageria")["Usuario"] ?? string.Empty;
            string senha = configurationManager.GetSection("Mensageria")["Senha"] ?? string.Empty;
            string filaInsert = configurationManager.GetSection("Mensageria")["NomeFilaInsercao"] ?? string.Empty;
            string filaEdicao = configurationManager.GetSection("Mensageria")["NomeFilaEdicao"] ?? string.Empty;
            string filaRemover = configurationManager.GetSection("Mensageria")["NomeFilaRemover"] ?? string.Empty;

            builder.Services.AddHostedService<Worker>();

            builder.Services.AddScoped<IContatosRepositorio, ContatosRepositorio>();
            builder.Services.AddSingleton<IMensageriaBus, MensageriaBus>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddTransient<DapperContext>();

            builder.Services.AddMassTransit(x =>
            {
                // Configura o consumidor
                x.AddConsumer<InserirContatoConsumer>();
                x.AddConsumer<RemoverContatoConsumer>();
                x.AddConsumer<EditarContatoConsumer>();

                // Configura o RabbitMQ ou outro transporte
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(servidor, h =>
                    {
                        h.Username(usuario);
                        h.Password(senha);
                    });

                    cfg.ReceiveEndpoint(filaInsert, e =>
                    {
                        e.ConfigureConsumer<InserirContatoConsumer>(context);
                        e.ConfigureConsumer<RemoverContatoConsumer>(context);
                        e.ConfigureConsumer<EditarContatoConsumer>(context);
                    });
                });
            });

            IHost host = builder.Build();
            host.Run();
        }
    }
}