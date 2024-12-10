using MassTransit;
using Microsoft.Extensions.Configuration;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;
namespace TechChallenge.Fase3.Infra.Utils
{
    public class MensageriaBus : IMensageriaBus
    {
        public IBus _bus { get; set; }
        public IConfiguration _configuration { get; set; }
        public string FilaInsert { get; protected set; }
        public string FilaEdicao { get; protected set; }
        public string FilaRemover { get; protected set; }
        public MensageriaBus(string connectionString)
        {

        }

        public MensageriaBus(IConfiguration configuration, IBus bus)
        {
            _bus = bus;
            _configuration = configuration;

            FilaInsert = _configuration.GetSection("Mensageria")["NomeFilaInsercao"] ?? string.Empty;
            FilaEdicao = _configuration.GetSection("Mensageria")["NomeFilaEdicao"] ?? string.Empty;
            FilaRemover = _configuration.GetSection("Mensageria")["NomeFilaRemover"] ?? string.Empty;
        }

        public async Task Enviar<T>(T request, string nomeFila, string routingKey)
        {
            if (request == null)
                throw new ArgumentNullException("Objeto nulo.");

            ISendEndpoint endpoint = await _bus.GetSendEndpoint(new Uri($"{nomeFila}"));
            await endpoint.Send(request, x => x.SetRoutingKey(routingKey));
        }

        public string GetFilaInserir() => FilaInsert;
        public string GetFilaEdicao() => FilaEdicao;
        public string GetFilaRemover() => FilaRemover;

    }
}
