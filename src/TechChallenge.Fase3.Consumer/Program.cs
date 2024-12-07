using MassTransit;
using TechChallenge.Fase3.Consumer.Eventos;

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

            builder.Services.AddMassTransit(x =>
            {
                // Configura o consumidor
                x.AddConsumer<InserirContatoConsumer>();

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
                    });
                });
            });

            IHost host = builder.Build();
            host.Run();
        }
    }
}