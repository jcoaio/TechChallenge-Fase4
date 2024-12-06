namespace TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces
{
    public interface IMensageriaBus
    {
        Task Enviar<T>(T objeto, string nomeFila);
        string GetFilaEdicao();
        string GetFilaInserir();
        string GetFilaRemover();
    }
}
