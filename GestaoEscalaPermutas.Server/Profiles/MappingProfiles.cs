using AutoMapper;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Dominio.DTO.Departamento;
using GestaoEscalaPermutas.Server.Models.Departamento;
using GestaoEscalaPermutas.Server.Models.Cargos;
using GestaoEscalaPermutas.Dominio.DTO.Cargo;
using GestaoEscalaPermutas.Server.Models.Funcionarios;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;

namespace GestaoEscalaPermutas.Server.Profiles
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {           

            CreateMap<DepartamentoModel, DepartamentoDTO>()
            .ReverseMap();
            CreateMap<DepartamentoDTO, Departamento>().ForMember(x => x.DtCriacao, opt => opt.MapFrom(src => src.DtCriacao));            
            CreateMap<Departamento, DepartamentoDTO>()
            .ForMember(x => x.valido, opt => opt.MapFrom(src => true))
            .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"));
                       
            
            CreateMap<CargoModel, CargoDTO>()
            .ReverseMap();
            CreateMap<CargoDTO, Cargo>().ForMember(x => x.DtCriacao, opt => opt.MapFrom(src => src.DtCriacao));
            CreateMap<Cargo, CargoDTO>()
            .ForMember(x => x.valido, opt => opt.MapFrom(src => true))
            .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"));


            CreateMap<FuncionarioModel, FuncionarioDTO>()
            .ReverseMap();
            CreateMap<Funcionario, FuncionarioDTO>()
             .ReverseMap();
            CreateMap<Funcionario, FuncionarioDTO>()
            .ForMember(x => x.valido, opt => opt.MapFrom(src => true))
            .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"));
        }
    }
}
