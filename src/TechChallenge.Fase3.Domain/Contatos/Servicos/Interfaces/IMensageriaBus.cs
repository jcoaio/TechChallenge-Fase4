using EasyNetQ;

namespace TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces
{
    public interface IMensageriaBus
    {
        public IBus Bus { get; set; }
    }
}
