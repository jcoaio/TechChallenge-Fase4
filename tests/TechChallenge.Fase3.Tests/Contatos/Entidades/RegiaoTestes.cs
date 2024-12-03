using TechChallenge.Fase3.Domain.Regioes.Entidades;

namespace TechChallenge.Fase3.Teste.Contatos.Entidades
{
    public class RegiaoTestes
    {
        [Fact]
        public void Quando_EstanciaRegiao_Espero_Objeto_Integro()
        {
            Regiao regiaoDefault = new();
            //ASSERT
            Assert.Equal(0, regiaoDefault.RegiaoDDD);
        }
    }
}
