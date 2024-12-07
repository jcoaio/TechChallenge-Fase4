using TechChallenge.Fase3.Consumer.Configurations;
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

            builder.Services.Configure<BusConfiguration>(builder.Configuration.GetSection("Mensageria"));
            BusConfiguration mensageriaConfig = builder.Configuration.GetSection("Mensageria").Get<BusConfiguration>() ?? throw new Exception("appSettings:Mensageria - Invalid JSON");

            builder.Services.AddHostedService<Worker>();

            builder.Services.AddScoped<IContatosRepositorio, ContatosRepositorio>();
            builder.Services.AddSingleton<IMensageriaBus, MensageriaBus>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddTransient<DapperContext>();

            MassTransitConfiguration.Configure(builder.Services, mensageriaConfig);

            IHost host = builder.Build();
            host.Run();
        }

    }
}

