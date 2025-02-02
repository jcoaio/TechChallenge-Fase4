using Prometheus;
using TechChallenge.Fase3.Consumer.Configurations;
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
            var builder = Host.CreateApplicationBuilder(args);

            builder.Configuration.AddEnvironmentVariables();

            var connectionString = builder.Configuration.GetConnectionString("mysql");

            // Configuração da Mensageria
            var mensageriaConfig = new BusConfig
            {
                Servidor = Environment.GetEnvironmentVariable("bus-server") ?? throw new FormatException("Variável de ambiente 'bus-server' não foi encontrada!"),
                Usuario = Environment.GetEnvironmentVariable("bus-user") ?? throw new FormatException("Variável de ambiente 'bus-user' não foi encontrada!"),
                Senha = Environment.GetEnvironmentVariable("bus-password") ?? throw new FormatException("Variável de ambiente 'bus-password' não foi encontrada!"),
                NomeFilaInsercao = "QueueInsercao",
                NomeFilaEdicao = "QueueEdicao",
                NomeFilaRemover = "QueueRemover",
                NomeExchange = "TechChallenge" 
            };

            // Adiciona as configurações ao DI
            builder.Services.Configure<BusConfig>(options =>
            {
                options.Servidor = mensageriaConfig.Servidor;
                options.Usuario = mensageriaConfig.Usuario;
                options.Senha = mensageriaConfig.Senha;
                options.NomeFilaInsercao = mensageriaConfig.NomeFilaInsercao;
                options.NomeFilaEdicao = mensageriaConfig.NomeFilaEdicao;
                options.NomeFilaRemover = mensageriaConfig.NomeFilaRemover;
                options.NomeExchange = mensageriaConfig.NomeExchange;
            });

            builder.Services.AddHostedService<Worker>();
            builder.Services.AddScoped<IContatosServico, ContatosServico>();
            builder.Services.AddScoped<IContatosRepositorio, ContatosRepositorio>();
            builder.Services.AddSingleton<IMensageriaBus, MensageriaBus>();
            builder.Services.AddTransient<DapperContext>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            MassTransitConfig.ConfigurarMassTransit(builder, mensageriaConfig);

            using KestrelMetricServer server = new(port: 1234);
            server.Start();

            IHost host = builder.Build();
            host.Run();
        }

    }
}

