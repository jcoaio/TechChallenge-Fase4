using AutoMapper;
using TechChallenge.Fase3.DataTransfer.Contatos.Reponses;
using TechChallenge.Fase3.DataTransfer.Contatos.Requests;
using TechChallenge.Fase3.DataTransfer.Utils;
using TechChallenge.Fase3.Domain.Contatos.Comandos;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios.Filtros;

namespace TechChallenge.Fase3.Application.Contatos.Profiles
{
    public class ContatoProfile : Profile
    {
        public ContatoProfile()
        {
            CreateMap<ContatoPaginacaoRequest, ContatosPaginadosFiltro>();
            CreateMap<ContatoCrudRequest, ContatoFiltro>();
            CreateMap<ContatoRequest, ContatoFiltro>();
            CreateMap<Contato, ContatoResponse>();
            CreateMap<Contato, ContatoComando>();
            CreateMap<ContatoComando, Contato>();
            CreateMap<PaginacaoConsulta<Contato>, PaginacaoConsulta<ContatoResponse>>();
        }
    }
}
