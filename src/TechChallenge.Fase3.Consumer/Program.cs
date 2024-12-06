using MassTransit;
using TechChallenge.Fase3.Consumer.ContatoServices;
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
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddHostedService<Service>();
            builder.Services.AddSingleton<EditarContato>();
            builder.Services.AddSingleton<InserirContatoConsumer>();
            builder.Services.AddSingleton<RemoverContato>();
            builder.Services.AddTransient<IContatosRepositorio, ContatosRepositorio>();
            builder.Services.AddTransient<DapperContext>();
            builder.Services.AddSingleton<IMensageriaBus, MensageriaBus>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            ConfigurationManager configurationManager = builder.Configuration;

            string servidor = configurationManager.GetSection("Mensageria")["Servidor"] ?? string.Empty;
            string usuario = configurationManager.GetSection("Mensageria")["Usuario"] ?? string.Empty;
            string senha = configurationManager.GetSection("Mensageria")["Senha"] ?? string.Empty;
            string filaInsert = configurationManager.GetSection("Mensageria")["NomeFilaInsercao"] ?? string.Empty;
            string filaEdicao = configurationManager.GetSection("Mensageria")["NomeFilaEdicao"] ?? string.Empty;
            string filaRemover = configurationManager.GetSection("Mensageria")["NomeFilaRemover"] ?? string.Empty;

            builder.Services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(servidor, "/", h =>
                    {
                        h.Username(usuario);
                        h.Password(senha);
                    });

                    cfg.ReceiveEndpoint(filaInsert, i =>
                    {
                        i.Consumer<InserirContatoConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
                x.AddConsumer<InserirContatoConsumer>();
            });


            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
