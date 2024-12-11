using System.Diagnostics;
using System.Text;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TechChallenge.Fase3.Consumer.Eventos;
using TechChallenge.Fase3.DataTransfer.Contatos.Reponses;
using TechChallenge.Fase3.DataTransfer.Contatos.Requests;
using TechChallenge.Fase3.DataTransfer.Utils;

namespace TechChallenge.Fase3.Teste.Integracao
{

    public class ContatosIntegracaoTeste : IClassFixture<TechChallengeApiFactory>, IAsyncLifetime
    {
        private readonly TechChallengeApiFactory techChallengeApiFactory;
        private readonly HttpClient apiFactoryClient;
        private readonly ServiceProvider _serviceProvider;

        public ContatosIntegracaoTeste(TechChallengeApiFactory techChallengeApiFactory)
        {
            this.techChallengeApiFactory = techChallengeApiFactory;
            apiFactoryClient = techChallengeApiFactory.CreateClient();

            ServiceCollection serviceCollection = new();
            serviceCollection.AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<InserirContatoConsumer>(); // Adicione os consumers necessários
                cfg.AddConsumer<EditarContatoConsumer>(); // Adicione os consumers necessários
                cfg.AddConsumer<RemoverContatoConsumer>(); // Adicione os consumers necessários
            });

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private async Task<HttpResponseMessage> CriaContato()
        {
            ContatoCrudRequest contato = new()
            {
                Nome = "Jorge da Silva Sauro",
                Email = "jorge@dino.com.br",
                DDD = 11,
                Telefone = "912345678"
            };

            StringContent requestContent = new(JsonConvert.SerializeObject(contato), Encoding.UTF8, "application/json");

            HttpResponseMessage result = await apiFactoryClient.PostAsync("api/contatos", requestContent);
            return result;
        }

        [Fact]
        public async Task Cria_Contatos_Corretamente()
        {
            HttpResponseMessage result = await CriaContato();
            ITestHarness harness = _serviceProvider.GetRequiredService<ITestHarness>();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }


        [Fact]
        public async Task Listar_Contatos_Corretamente()
        {
            HttpResponseMessage result = await apiFactoryClient.GetAsync("api/contatos/itens");
            List<ContatoResponse>? contatos = JsonConvert.DeserializeObject<List<ContatoResponse>>(await result.Content.ReadAsStringAsync());
            Assert.NotNull(contatos);
            contatos.Count.Should().BeGreaterThan(0);
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Editar_Contatos_Corretamente()
        {

            _ = await CriaContato();

            int idEdicao = 0;

            HttpResponseMessage httpResponseMessage = await apiFactoryClient.GetAsync($"api/contatos");

            PaginacaoConsulta<ContatoResponse>? contatoPaginados = JsonConvert.DeserializeObject<PaginacaoConsulta<ContatoResponse>>(await httpResponseMessage.Content.ReadAsStringAsync());

            ContatoResponse? contato = contatoPaginados.Registros.FirstOrDefault();

            if (contato!.Id.HasValue)
            {
                idEdicao = contato.Id.Value;
                contato.Nome = "Contato Editado";
            }

            StringContent requestContent = new(JsonConvert.SerializeObject(contato), Encoding.UTF8, "application/json");

            HttpResponseMessage result = await apiFactoryClient.PutAsync($"api/contatos/{idEdicao}", requestContent);

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Criar_Varios_Contatos_Tempo_Limite()
        {
            // Arrange: Envia a criação dos 10 contatos
            for (int i = 0; i < 10; i++)
            {
                _ = await CriaContato();
            }
            // Aguardar um tempo suficiente para os contatos serem processados e enviados para o banco
            int tentativas = 0;
            int maxTentativas = 30; // Máximo de tentativas
            TimeSpan intervalo = TimeSpan.FromSeconds(0.1); // Intervalo de 2 segundos entre as tentativas
            TimeSpan timeout = TimeSpan.FromMinutes(2); // Timeout para o teste (ajuste conforme necessário)
            Stopwatch stopWatch = Stopwatch.StartNew();

            while (tentativas < maxTentativas && stopWatch.Elapsed < timeout)
            {
                HttpResponseMessage httpResponseMessage = await apiFactoryClient.GetAsync($"api/contatos/itens");

                List<ContatoResponse>? contatos = JsonConvert.DeserializeObject<List<ContatoResponse>>(await httpResponseMessage.Content.ReadAsStringAsync());

                if (contatos.Count() == 10)
                {
                    // Assert que os 10 contatos foram criados
                    contatos.Count.Should().Be(10);
                    return;
                }

                tentativas++;
                await Task.Delay(intervalo); // Espera antes de tentar novamente
            }

        }

        [Fact]
        public async Task Deletar_Contatos_Corretamente()
        {
            _ = await CriaContato();
            int idDelecao = 1;

            HttpResponseMessage result = await apiFactoryClient.DeleteAsync($"api/contatos/{idDelecao}");

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        public Task InitializeAsync()
        {
            return _serviceProvider.GetRequiredService<ITestHarness>().Start();
        }

        public Task DisposeAsync()
        {
            return _serviceProvider.DisposeAsync().AsTask();
        }
    }
}
