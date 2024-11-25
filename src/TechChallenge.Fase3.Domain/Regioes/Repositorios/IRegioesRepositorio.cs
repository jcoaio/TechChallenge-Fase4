using TechChallenge.Fase3.Domain.Regioes.Entidades;

namespace TechChallenge.Fase3.Domain.Regioes.Repositorios
{
    public interface IRegioesRepositorio
    {
        Task<List<Regiao>> ListarRegioesAsync(int ddd = 0);
    }
}
