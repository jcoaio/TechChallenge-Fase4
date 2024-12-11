using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;
using TechChallenge.Fase3.Infra.Utils;
using TechChallenge.Fase3.Infra.Utils.DBContext;

namespace TechChallenge.Fase3.Teste.Integracao
{
    public class TechChallengeConsumerFactory : WebApplicationFactory<Consumer.Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            // Configure the builder to use the same setup as your worker
            builder.ConfigureServices(services =>
            {
                string connectionString = "Server=localhost; Database=techchallenge; Uid=root; Pwd=Teste123;AllowUserVariables=true;";
                string connectionStringBus = "amqp://guest:guest@localhost:5672/";

                // Open a database connection to validate connectivity
                using (MySqlConnection connection = new(connectionString))
                {
                    connection.Open();
                }

                // Replace the existing service descriptors for testing
                ServiceDescriptor? descriptorMySql = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DapperContext));

                ServiceDescriptor? descriptorBus = services.FirstOrDefault(
                    d => d.ServiceType == typeof(IMensageriaBus));

                if (descriptorMySql != null)
                {
                    services.Remove(descriptorMySql);
                }

                if (descriptorBus != null)
                {
                    services.Remove(descriptorBus);
                }

                services.AddSingleton(new DapperContext(connectionString));
                services.AddSingleton<IMensageriaBus>(new MensageriaBus(""));
            });

            return base.CreateHost(builder);
        }
    }

}
