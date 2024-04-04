using AutoMapper;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Dominio.DTO.Departamento;
using GestaoEscalaPermutas.Server.Models.Departamento;
using GestaoEscalaPermutas.Server.Models.Cargos;
using GestaoEscalaPermutas.Dominio.DTO.Cargo;
using GestaoEscalaPermutas.Server.Models.Funcionarios;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Server.Models.Escala;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Server.Models.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho;
using GestaoEscalaPermutas.Server.Models.TipoEscala;
using GestaoEscalaPermutas.Dominio.DTO.TipoEscala;
using GestaoEscalaPermutas.Server.Models.EscalaPronta;
using GestaoEscalaPermutas.Dominio.DTO.EscalaPronta;

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

            CreateMap<EscalaModel, EscalaDTO>()
            .ReverseMap();
            CreateMap<Escala, EscalaDTO>()
             .ReverseMap();
            CreateMap<Escala, EscalaDTO>()
            .ForMember(x => x.valido, opt => opt.MapFrom(src => true))
            .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"));
            CreateMap<EscalaDTO, Escala>().ForMember(x => x.DtCriacao, opt => opt.MapFrom(src => src.DtCriacao));

            CreateMap<PostoTrabalhoModel, PostoTrabalhoDTO>()
            .ReverseMap();
            CreateMap<PostoTrabalho, PostoTrabalhoDTO>()
             .ReverseMap();
            CreateMap<PostoTrabalho, PostoTrabalhoDTO>()
            .ForMember(x => x.valido, opt => opt.MapFrom(src => true))
            .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"));

            CreateMap<TipoEscalaModel, TipoEscalaDTO>()
            .ReverseMap();
            CreateMap<TipoEscala, TipoEscalaDTO>()
             .ReverseMap();
            CreateMap<TipoEscala, TipoEscalaDTO>()
            .ForMember(x => x.valido, opt => opt.MapFrom(src => true))
            .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"));

            CreateMap<EscalaProntaModel, EscalaProntaDTO>()
            .ReverseMap();
            CreateMap<EscalaPronta, EscalaProntaDTO>()
             .ReverseMap();
            CreateMap<EscalaPronta, EscalaProntaDTO>()
            .ForMember(x => x.valido, opt => opt.MapFrom(src => true))
            .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"));
            CreateMap<EscalaProntaDTO, EscalaPronta>().ForMember(x => x.DtCriacao, opt => opt.MapFrom(src => src.DtCriacao));


        }
    }
}
