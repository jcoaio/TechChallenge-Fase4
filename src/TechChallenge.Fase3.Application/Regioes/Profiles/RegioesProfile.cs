using AutoMapper;
using TechChallenge.Fase3.DataTransfer.Regioes.Responses;
using TechChallenge.Fase3.Domain.Regioes.Entidades;

namespace TechChallenge.Fase3.Application.Regioes.Profiles
{
    public class RegioesProfile : Profile
    {
        public RegioesProfile()
        {
            CreateMap<Regiao, RegiaoResponse>();
        }
    }
}
