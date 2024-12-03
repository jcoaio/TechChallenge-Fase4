using AutoMapper;
using FluentAssertions;
using NSubstitute;
using TechChallenge.Fase3.DataTransfer.Utils;
using TechChallenge.Fase3.DataTransfer.Utils.Enumeradores;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios;
using TechChallenge.Fase3.Domain.Contatos.Repositorios.Filtros;
using TechChallenge.Fase3.Domain.Contatos.Servicos;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;

namespace TechChallenge.Fase3.Teste.Contatos.Servicos;

public class ContatoServicoTestes
{
    private readonly ContatosServico contatoServico;
    private readonly IContatosRepositorio contatoRepositorio;
    private readonly IMensageriaBus mensageriaBus;
    private readonly IMapper mapper;
    public ContatoServicoTestes()
    {
        contatoRepositorio = Substitute.For<IContatosRepositorio>();
        mapper = Substitute.For<IMapper>();
        mensageriaBus = Substitute.For<IMensageriaBus>();
        contatoServico = new ContatosServico(contatoRepositorio, mensageriaBus, mapper);
    }


    public class ListarPaginacaoContatosAsyncMetodo : ContatoServicoTestes
    {
        [Fact]
        public async Task Quando_ListoContatosComPaginacao_Espero_ListaDeContatosValidos()
        {
            ContatosPaginadosFiltro contatoFiltro = new()
            {
                DDD = 11,
                CpOrd = "",
                Email = "fiap@contato.com.br",
                Nome = "Fiap Contato",
                Pg = 1,
                Qt = 10,
                Regiao = "SP",
                Telefone = "912345678",
                TpOrd = TipoOrdernacao.Desc
            };


            string nome = "Fiap Contato";
            string email = "fiap@contato.com.br";
            int ddd = 11;
            string telefone = "912345678";

            Contato contato = new(nome, email, ddd, telefone);

            PaginacaoConsulta<Contato> contatoPaginacaoResponse = new()
            {
                Registros = [contato],
                Total = 1
            };


            contatoRepositorio.ListarPaginacaoContatosAsync(Arg.Any<ContatosPaginadosFiltro>())
                .Returns(Task.FromResult(contatoPaginacaoResponse));

            PaginacaoConsulta<Contato> contatos = await contatoServico.ListarPaginacaoContatosAsync(contatoFiltro);

            Assert.True(contatos.Total == 1);

        }
    }

    public class ListarContatosAsyncMetodo : ContatoServicoTestes
    {
        [Fact]
        public async Task Quando_ListoContato_Espero_ContatoValidos()
        {
            ContatoFiltro contatoFiltro = new()
            {
                DDD = 11,
                Email = "fiap@contato.com.br",
                Nome = "Fiap Contato",
                Regiao = "SP",
                Telefone = "912345678",
            };

            string nome = "Fiap Contato";
            string email = "fiap@contato.com.br";
            int ddd = 11;
            string telefone = "912345678";

            Contato contato = new(nome, email, ddd, telefone);

            List<Contato> contatoLista = [contato];

            contatoRepositorio.ListarContatosAsync(Arg.Any<ContatoFiltro>())
                .Returns(Task.FromResult(contatoLista));

            List<Contato> contatos = await contatoServico.ListarContatosAsync(contatoFiltro);

            Assert.Equal(contatoLista.First().Email, contatos.First().Email);
            Assert.Equal(contatoLista.First().Nome, contatos.First().Nome);
            contatos.First().Email.Should().Be(contatoLista.First().Email);
            contatos.First().Nome.Should().Be(contatoLista.First().Nome);

            await contatoRepositorio.Received().ListarContatosAsync(contatoFiltro);

        }
    }

    public class InserirContatoAsyncMetodo : ContatoServicoTestes
    {
        [Theory]
        [InlineData(11, "fiap@contato.com.br", null, "Sudeste", "912345678")]
        [InlineData(11, null, "Fiap Contato", "Sudeste", "912345678")]
        [InlineData(11, "fiap@contato.com.br", "Fiap Contato", "Sudeste", null)]
        [InlineData(11, "fiap@contato.com.br", "Fiap Contato", "Sudeste", "A12345678")]
        [InlineData(null, "fiap@contato.com.br", "Fiap Contato", "Sudeste", "912345678")]
        [InlineData(11, "confrontationasd.com.br", "Flap Contact", "Sudeste", "912345678")]
        [InlineData(0, "fiap@contato.com.br", "Fiap Contato", "Sudeste", "912345678")]
        [InlineData(-1, "fiap@contato.com.br", "Fiap Contato", "Sudeste", "912345678")]
        [InlineData(11, "", "Fiap Contato", "Sudeste", "912345678")]
        public async Task Quando_ListoContatosComPaginacao_Espero_ListaDeContatosValidos(int? ddd, string email, string nome, string regiao, string telefone)
        {
            ContatoFiltro contatoFiltro = new()
            {
                DDD = ddd,
                Email = email,
                Nome = nome,
                Regiao = regiao,
                Telefone = telefone,
            };

            await contatoServico.Invoking(x => x.InserirContatoAsync(contatoFiltro)).Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task Quando_ListoContatosComPaginacao_Espero_ListaDeContatosValidosOk()
        {
            ContatoFiltro contatoFiltro = new()
            {
                DDD = 27,
                Email = "email@asd.com",
                Nome = "nome",
                Regiao = "Sudeste",
                Telefone = "23423432",
            };

            await contatoServico.Invoking(x => x.InserirContatoAsync(contatoFiltro)).Should().NotThrowAsync();
        }

        [Fact]
        public async Task Quando_ListoContatosComPaginacao_Espero_AtualizarDeContatosValidosOk()
        {
            Contato contato = new("27", "email@asd.com", 27, "23423432");
            await contatoServico.Invoking(x => x.AtualizarContatoAsync(contato)).Should().NotThrowAsync();
        }

        [Fact]
        public async Task Quando_ListoContatosComPaginacao_Espero_RecuperarDeContatosValidosOk()
        {
            await contatoServico.Invoking(x => x.RecuperarContatoAsync(1)).Should().NotThrowAsync();
        }

        [Fact]
        public async Task Quando_ListoContatosComPaginacao_Espero_ExcluirDeContatosValidosOk()
        {
            Contato contato = new("27", "email@asd.com", 27, "23423432");
            contato.SetId(1);
            contatoRepositorio.RecuperarContatoAsync(1).Returns(contato);
            await contatoServico.Invoking(x => x.RemoverContatoAsync(1)).Should().NotThrowAsync();
        }
    }
}

