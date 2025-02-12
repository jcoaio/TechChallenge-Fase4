﻿using MassTransit;
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

        public MensageriaBus(string filaInsert)
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

        public async Task Enviar<T>(T request, string nomeFila)
        {
            if (request == null)
                throw new ArgumentNullException("Objeto nulo.");

            await _bus.Publish(request, context => 
            {
                context.SetRoutingKey($"{nomeFila}-deployment-{Environment.MachineName}");
            });

        }

        public string GetFilaInserir() => FilaInsert;
        public string GetFilaEdicao() => FilaEdicao;
        public string GetFilaRemover() => FilaRemover;

    }
}
