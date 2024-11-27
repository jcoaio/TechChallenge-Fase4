using TechChallenge.Fase3.Domain.Contatos.Repositorios;
using TechChallenge.Fase3.Infra.Contatos;
using TechChallenge.Fase3.Infra.Utils.DBContext;

namespace ContatoWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddTransient<DapperContext>();
            builder.Services.AddTransient<IContatosRepositorio, ContatosRepositorio>();

            IHost host = builder.Build();
            host.Run();
        }
    }
}