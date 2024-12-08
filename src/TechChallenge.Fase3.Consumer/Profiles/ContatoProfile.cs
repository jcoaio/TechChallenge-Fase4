using AutoMapper;
using TechChallenge.Fase3.Domain.Contatos.Comandos;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios.Filtros;

namespace TechChallenge.Fase3.Consumer.Profiles
{
    public class ContatoProfile : Profile
    {
        public ContatoProfile()
        {
            CreateMap<Contato, ContatoComando>().ReverseMap();
            CreateMap<ContatoFiltro, ContatoComando>().ReverseMap();
        }
    }
}
