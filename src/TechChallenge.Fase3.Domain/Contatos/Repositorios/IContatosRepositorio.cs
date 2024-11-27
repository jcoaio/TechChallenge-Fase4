using TechChallenge.Fase3.DataTransfer.Utils;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios.Filtros;

namespace TechChallenge.Fase3.Domain.Contatos.Repositorios
{
    public interface IContatosRepositorio
    {
        Task<Contato> AtualizarContatoAsync(Contato contato);
        Task<Contato> InserirContatoAsync(Contato contato);
        Task<List<Contato>> ListarContatosAsync(ContatoFiltro filtro);
        Task<PaginacaoConsulta<Contato>> ListarPaginacaoContatosAsync(ContatosPaginadosFiltro filtro);
        Task<Contato> RecuperarContatoAsync(int id);
        Task RemoverContatoAsync(int id);
    }
}
