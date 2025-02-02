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

            builder.Services.Configure<BusConfig>(builder.Configuration.GetSection("Mensageria"));

            Console.WriteLine("DEBUG - Variáveis de ambiente carregadas:");
            Console.WriteLine($"bus-server: {Environment.GetEnvironmentVariable("bus-server")}");
            Console.WriteLine($"bus-user: {Environment.GetEnvironmentVariable("bus-user")}");
            Console.WriteLine($"bus-password: {Environment.GetEnvironmentVariable("bus-password")}");
            
            Console.WriteLine("DEBUG - Configuração carregada do appsettings:");
            Console.WriteLine($"Servidor: {mensageriaConfig.Servidor}");
            Console.WriteLine($"Usuario: {mensageriaConfig.Usuario}");
            Console.WriteLine($"Senha: {mensageriaConfig.Senha}");

            
            var mensageriaConfig = builder.Configuration.GetSection("Mensageria").Get<BusConfig>() 
                ?? throw new FormatException("Configuração de Mensageria não encontrada ou inválida!");

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

