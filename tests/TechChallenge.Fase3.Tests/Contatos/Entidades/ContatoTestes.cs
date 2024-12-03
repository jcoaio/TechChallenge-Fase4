using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Regioes.Entidades;

namespace TechChallenge.Fase3.Teste.Contatos.Entidades;

public class ContatoTestes
{
    [Fact]
    public void EntidadeContatoTestes()
    {
        //ARRANGE
        string nome = "Fiap Contato";
        string email = "fiap@contato.com.br";
        int ddd = 11;
        string telefone = "912345678";

        //ACT
        Contato contato = new(nome, email, ddd, telefone);


        //ASSERT
        Assert.Equal(nome, contato.Nome);
        Assert.Equal(email, contato.Email);
        Assert.Equal(ddd, contato.DDD);
        Assert.Equal(telefone, contato.Telefone);

        contato = new Contato();
        Assert.Equal(contato.Nome, null);
    }
}

public class ContatoSetersTestes
{
    [Fact]
    public void Quando_DefinirId_Espero_Id()
    {
        //ARRANGE
        int id = 1;
        string nome = "Fiap Contato";
        string email = "fiap@contato.com.br";
        int ddd = 11;
        string telefone = "912345678";

        //ACT
        Contato contato = new(nome, email, ddd, telefone);

        contato.SetId(id);

        //ASSERT
        Assert.Equal(id, contato.Id);
    }

    [Fact]
    public void Quando_DefinirRegiao_Espero_Regiao()
    {
        //ARRANGE
        string nome = "Fiap Contato";
        string email = "fiap@contato.com.br";
        int ddd = 11;
        string telefone = "912345678";

        string estado = "SP";
        string descricao = "Sudeste";

        //ACT
        Contato contato = new(nome, email, ddd, telefone);
        Regiao regiao = new(ddd, estado, descricao);

        contato.SetRegiao(regiao);

        //ASSERT
        Assert.Equal(regiao, contato.Regiao);
    }
}