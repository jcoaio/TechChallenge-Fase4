using ContatoWorker.ContatoServices;
using TechChallenge.Fase3.Domain.Contatos.Repositorios;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;
using TechChallenge.Fase3.Infra.Contatos;
using TechChallenge.Fase3.Infra.Utils;
using TechChallenge.Fase3.Infra.Utils.DBContext;

namespace ContatoWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddTransient<DapperContext>();
            builder.Services.AddSingleton<IMensageriaBus, MensageriaBus>();

            builder.Services.AddHostedService<Worker>();
            builder.Services.AddSingleton<EditarContato>();
            builder.Services.AddSingleton<InserirContato>();
            builder.Services.AddSingleton<RemoverContato>();
            builder.Services.AddTransient<IContatosRepositorio, ContatosRepositorio>();

            IHost host = builder.Build();
            host.Run();
        }
    }
}