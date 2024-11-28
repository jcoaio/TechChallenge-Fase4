using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace TechChallenge.Fase3.Infra.Utils.DBContext
{
    public class DapperContext
    {
        private readonly string connectionString;

        public DapperContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public DapperContext(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("mysql")!;
        }

        public IDbConnection CreateConnection() => new MySqlConnection(connectionString);
    }
}
