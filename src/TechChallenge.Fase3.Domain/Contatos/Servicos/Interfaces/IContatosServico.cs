using TechChallenge.Fase3.DataTransfer.Utils;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios.Filtros;

namespace TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces
{
    public interface IContatosServico
    {
        Task<Contato> AtualizarContatoAsync(Contato contato);
        Task<Contato> InserirContatoAsync(ContatoFiltro contato);
        Task<List<Contato>> ListarContatosAsync(ContatoFiltro request);
        Task<PaginacaoConsulta<Contato>> ListarPaginacaoContatosAsync(ContatosPaginadosFiltro request);
        Task<Contato> RecuperarContatoAsync(int id);
        Task RemoverContatoAsync(int id);
    }
}
