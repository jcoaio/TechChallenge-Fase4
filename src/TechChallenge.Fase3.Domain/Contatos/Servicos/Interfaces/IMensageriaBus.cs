namespace TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces
{
    public interface IMensageriaBus
    {
        Task Enviar<T>(T objeto, string nomeFila, string routingKey);
        string GetFilaEdicao();
        string GetFilaInserir();
        string GetFilaRemover();
    }
}
