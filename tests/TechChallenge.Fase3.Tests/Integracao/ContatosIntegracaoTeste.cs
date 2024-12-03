using System.Diagnostics;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using TechChallenge.Fase3.DataTransfer.Contatos.Reponses;
using TechChallenge.Fase3.DataTransfer.Contatos.Requests;
using TechChallenge.Fase3.DataTransfer.Utils;

namespace TechChallenge.Fase3.Teste.Integracao
{

    public class ContatosIntegracaoTeste : IClassFixture<TechChallengeApiFactory>, IClassFixture<TechChallengeConsumerFactory>
    {
        private readonly TechChallengeApiFactory techChallengeApiFactory;
        private readonly TechChallengeConsumerFactory techChallengeConsumerFactory;
        private readonly HttpClient apiFactoryClient;

        public ContatosIntegracaoTeste(TechChallengeApiFactory techChallengeApiFactory, TechChallengeConsumerFactory techChallengeConsumerFactory)
        {
            this.techChallengeApiFactory = techChallengeApiFactory;
            apiFactoryClient = techChallengeApiFactory.CreateClient();

            this.techChallengeConsumerFactory = techChallengeConsumerFactory;
            techChallengeConsumerFactory.CreateClient();
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
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }


        [Fact]
        public async Task Listar_Contatos_Corretamente()
        {
            int QUANTIDADE_CRIADA = 10;
            int QUANTIDADE_ESPERADA = QUANTIDADE_CRIADA;
            int MAXIMO_TENTATIVAS = 20;
            int tentativa = 0;
            for (int i = 0; i < QUANTIDADE_CRIADA; i++)
            {
                _ = await CriaContato();
            }

            HttpResponseMessage result = await apiFactoryClient.GetAsync("api/contatos/itens");

            while (QUANTIDADE_ESPERADA != 10)
            {
                List<ContatoResponse>? contatos = JsonConvert.DeserializeObject<List<ContatoResponse>>(await result.Content.ReadAsStringAsync());
                if (contatos == null)
                    break;

                if (contatos.Count < 10)
                    continue;

                if (contatos.Count == 10)
                    contatos.Count.Should().Be(QUANTIDADE_CRIADA);

                tentativa++;

                if (tentativa >= MAXIMO_TENTATIVAS)
                    break;
            }

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
    }
}
