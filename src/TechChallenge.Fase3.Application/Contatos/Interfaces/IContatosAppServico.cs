using TechChallenge.Fase3.DataTransfer.Contatos.Requests;
using TechChallenge.Fase3.DataTransfer.Utils;
using TechChallenge.Fase3.DataTransfer.Contatos.Reponses;

namespace TechChallenge.Fase3.Application.Contatos.Interfaces
{
    public interface IContatosAppServico
    {
        Task AtualizarContatoAsync(ContatoCrudRequest request, int id);
        Task InserirContatoAsync(ContatoCrudRequest request);
        Task<PaginacaoConsulta<ContatoResponse>> ListarContatosComPaginacaoAsync(ContatoPaginacaoRequest request);
        Task<List<ContatoResponse>> ListarContatosSemPaginacaoAsync(ContatoRequest request);
        Task<ContatoResponse> RecuperarContatoAsync(int id);
        Task RemoverContatoAsync(int id);
    }
}
