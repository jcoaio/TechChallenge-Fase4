using System.ComponentModel.DataAnnotations;
using AutoMapper;
using EasyNetQ;
using TechChallenge.Fase3.DataTransfer.Utils;
using TechChallenge.Fase3.Domain.Contatos.Comandos;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios;
using TechChallenge.Fase3.Domain.Contatos.Repositorios.Filtros;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;
using TechChallenge.Fase3.Domain.Utils;

namespace TechChallenge.Fase3.Domain.Contatos.Servicos
{
    public class ContatosServico(IContatosRepositorio contatosRepositorio, Rabbit rabbit, IMapper mapper) : IContatosServico
    {

        public async Task<PaginacaoConsulta<Contato>> ListarPaginacaoContatosAsync(ContatosPaginadosFiltro request)
        {
            PaginacaoConsulta<Contato> consultaPaginada = await contatosRepositorio.ListarPaginacaoContatosAsync(request);
            return consultaPaginada;
        }

        public async Task<List<Contato>> ListarContatosAsync(ContatoFiltro request)
            => await contatosRepositorio.ListarContatosAsync(request);


        public Task InserirContatoAsync(ContatoFiltro novoContato)
        {
            ValidarCampos(novoContato);
            Contato contatoInserir = new(novoContato.Nome!, novoContato.Email!, (int)novoContato.DDD!, novoContato.Telefone!);

            ContatoInserirComando contatoComandos = mapper.Map<ContatoInserirComando>(contatoInserir);

            return rabbit.Bus.PubSub.PublishAsync(contatoComandos, TopicosRabbit.Inserir);
        }

        public async Task RemoverContatoAsync(int id)
        {
            Contato contato = await RecuperarContatoAsync(id);
            ContatoInserirComando contatoComandos = mapper.Map<ContatoInserirComando>(contato);
            await rabbit.Bus.PubSub.PublishAsync(contatoComandos, TopicosRabbit.Remover);
        }

        public async Task<Contato> RecuperarContatoAsync(int id)
        {
            return await contatosRepositorio.RecuperarContatoAsync(id);
        }

        public async Task AtualizarContatoAsync(Contato contato)
        {
            await rabbit.Bus.PubSub.PublishAsync(contato, TopicosRabbit.Editar);
        }

        private static void ValidarCampos(ContatoFiltro contatoRequest)
        {
            if (string.IsNullOrEmpty(contatoRequest.Nome))
                throw new ArgumentException("Nome não preenchido.");

            if (!ValidarEmail(contatoRequest.Email))
                throw new ArgumentException($"Email inválido: {contatoRequest.Email}");

            if (string.IsNullOrEmpty(contatoRequest.Telefone))
                throw new ArgumentException("Telefone não preenchido.");

            if (!contatoRequest.Telefone!.All(char.IsDigit))
                throw new ArgumentException($"Telefone inválido: {contatoRequest.Telefone}");

            if (contatoRequest.DDD == null)
                throw new ArgumentException("DDD não preenchido.");

            if (contatoRequest.DDD.Value <= 0)
                throw new ArgumentException("DDD não preenchido.");
        }

        public static bool ValidarEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            if (new EmailAddressAttribute().IsValid(email))
                return true;

            return false;
        }

    }
}
