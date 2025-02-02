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

            var mensageriaConfig = builder.Configuration.GetSection("Mensageria").Get<BusConfig>() 
                ?? throw new FormatException("Configuração de Mensageria não encontrada ou inválida!");

            if (string.IsNullOrEmpty(mensageriaConfig.Servidor) ||
                string.IsNullOrEmpty(mensageriaConfig.Usuario) ||
                string.IsNullOrEmpty(mensageriaConfig.Senha))
            {
                throw new FormatException("Configuração de Mensageria está incompleta. Verifique os Secrets!");
            }

            builder.Services.Configure<BusConfig>(builder.Configuration.GetSection("Mensageria"));

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

