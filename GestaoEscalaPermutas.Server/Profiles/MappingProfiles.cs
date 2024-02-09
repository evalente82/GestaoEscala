using AutoMapper;
using GestaoEscalaPermuta.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Dominio.DTO.Departamento;
using GestaoEscalaPermutas.Server.Models.Departamento;

namespace GestaoEscalaPermutas.Server.Profiles
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {           

            CreateMap<DepartamentoModel, DepartamentoDTO>()
            .ReverseMap();

            //CreateMap<DepartamentoDTO, Departamento>()
            //.ReverseMap();

            CreateMap<DepartamentoDTO, Departamento>().ForMember(dest => dest.DtCriacao, opt => opt.MapFrom(src => src.DtCriacao));

            CreateMap<Departamento, DepartamentoDTO>()
            .ForMember(dest => dest.valido, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"));
        }
    }
}
