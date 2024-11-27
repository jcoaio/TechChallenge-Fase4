using AutoMapper;
using TechChallenge.Fase3.Application.Contatos.Interfaces;
using TechChallenge.Fase3.DataTransfer.Contatos.Reponses;
using TechChallenge.Fase3.DataTransfer.Contatos.Requests;
using TechChallenge.Fase3.DataTransfer.Utils;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios.Filtros;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;
using TechChallenge.Fase3.Domain.Regioes.Entidades;
using TechChallenge.Fase3.Domain.Regioes.Repositorios;

namespace TechChallenge.Fase3.Application.Contatos.Servicos
{
    public class ContatosAppServico(IContatosServico contatosServico, IRegioesRepositorio regioesRepositorio, IMapper mapper) : IContatosAppServico
    {
        public async Task<PaginacaoConsulta<ContatoResponse>> ListarContatosComPaginacaoAsync(ContatoPaginacaoRequest request)
        {
            ContatosPaginadosFiltro contatosFiltro = mapper.Map<ContatosPaginadosFiltro>(request);

            PaginacaoConsulta<Contato> consulta = await contatosServico.ListarPaginacaoContatosAsync(contatosFiltro);

            PaginacaoConsulta<ContatoResponse> response = mapper.Map<PaginacaoConsulta<ContatoResponse>>(consulta);

            return response;
        }

        public Task InserirContatoAsync(ContatoCrudRequest request)
        {
            ContatoFiltro contatoFiltro = mapper.Map<ContatoFiltro>(request);

            return contatosServico.InserirContatoAsync(contatoFiltro);
        }

        public async Task AtualizarContatoAsync(ContatoCrudRequest request, int id)
        {
            if (!request.DDD.HasValue || request.DDD <= 0)
                throw new Exception("Código de DDD inválido.");
            List<Regiao> result = await regioesRepositorio.ListarRegioesAsync((int)request.DDD!);
            if (result.Count == 0)
                throw new Exception("Região não encontrada.");

            Contato contatoAtualizado = await contatosServico.RecuperarContatoAsync(id) ?? throw new Exception("Usuário não encontrado.");

            contatoAtualizado.SetDDD((int)request.DDD!);
            contatoAtualizado.SetEmail(request.Email!);
            contatoAtualizado.SetNome(request.Nome!);
            contatoAtualizado.SetTelefone(request.Telefone!);
            await contatosServico.AtualizarContatoAsync(contatoAtualizado);
        }

        public async Task RemoverContatoAsync(int id)
        {
            await contatosServico.RemoverContatoAsync(id);
        }

        public async Task<List<ContatoResponse>> ListarContatosSemPaginacaoAsync(ContatoRequest request)
        {
            ContatoFiltro contatosFiltro = mapper.Map<ContatoFiltro>(request);

            List<Contato> consulta = await contatosServico.ListarContatosAsync(contatosFiltro);

            List<ContatoResponse> response = mapper.Map<List<ContatoResponse>>(consulta);

            return response;
        }

        public async Task<ContatoResponse> RecuperarContatoAsync(int id)
        {
            Contato consulta = await contatosServico.RecuperarContatoAsync(id) ?? throw new Exception("Registro não encontrado!");

            List<Regiao> regiao = await regioesRepositorio.ListarRegioesAsync((int)consulta.DDD!);

            consulta.SetRegiao(regiao.FirstOrDefault());
            return mapper.Map<ContatoResponse>(consulta);
        }
    }
}
