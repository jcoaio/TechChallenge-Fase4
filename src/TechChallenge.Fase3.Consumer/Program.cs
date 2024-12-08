using TechChallenge.Fase3.Consumer.Configurations;
using TechChallenge.Fase3.Domain.Contatos.Servicos;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;
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
            builder.Services.AddSingleton<IMensageriaBus, MensageriaBus>();
            builder.Services.AddTransient<DapperContext>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            MassTransitConfig.Configure(builder.Services, mensageriaConfig);

            IHost host = builder.Build();
            host.Run();
        }

    }
}

