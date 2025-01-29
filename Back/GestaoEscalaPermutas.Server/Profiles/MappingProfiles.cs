using AutoMapper;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Dominio.DTO;
using GestaoEscalaPermutas.Server.Models;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Dominio.DTO.Cargo;
using GestaoEscalaPermutas.Dominio.DTO.Departamento;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.EscalaPronta;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.DTO.Login;
using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using GestaoEscalaPermutas.Dominio.DTO.Permutas;
using GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.DTO.TipoEscala;
using GestaoEscalaPermutas.Dominio.DTO.Usuario;
using GestaoEscalaPermutas.Server.Models.Cargos;
using GestaoEscalaPermutas.Server.Models.Departamento;
using GestaoEscalaPermutas.Server.Models.Escala;
using GestaoEscalaPermutas.Server.Models.EscalaPronta;
using GestaoEscalaPermutas.Server.Models.Funcionarios;
using GestaoEscalaPermutas.Server.Models.Login;
using GestaoEscalaPermutas.Server.Models.PerfilFuncionalidade;
using GestaoEscalaPermutas.Server.Models.Permuta;
using GestaoEscalaPermutas.Server.Models.PostoTrabalho;
using GestaoEscalaPermutas.Server.Models.TipoEscala;

namespace GestaoEscalaPermutas.Server.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // ======= FUNCIONÁRIOS =======
            CreateMap<Funcionario, FuncionarioDTO>()
                .ForMember(dest => dest.valido, opt => opt.MapFrom(src => true)) // ✅ Sempre define como true
                .ForMember(dest => dest.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso")) // ✅ Mensagem padrão
                .ReverseMap();

            CreateMap<Funcionario, FuncionarioDTO>()
                .ForMember(dest => dest.IdFuncionario, opt => opt.MapFrom(src => src.IdFuncionario))
                .ForMember(dest => dest.NmNome, opt => opt.MapFrom(src => src.NmNome))
                .ForMember(dest => dest.IdCargo, opt => opt.MapFrom(src => src.IdCargo))
                .ReverseMap();

            CreateMap<FuncionarioModel, FuncionarioDTO>().ReverseMap();
            CreateMap<Funcionario, FuncionarioDTO>()
                .ForMember(dest => dest.valido, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            // ======= CARGOS =======
            CreateMap<CargoModel, CargoDTO>().ReverseMap();
            CreateMap<Cargo, CargoDTO>()
                .ForMember(x => x.DtCriacao, opt => opt.MapFrom(src => src.DtCriacao))
                .ForMember(x => x.valido, opt => opt.MapFrom(src => true))
                .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            // ======= PERFIS =======
            CreateMap<Perfil, PerfilDTO>().ReverseMap();
            CreateMap<PerfilDTO, PerfilModel>().ReverseMap();
            CreateMap<DepInfra.Perfil, PerfilDTO>().ReverseMap();

            // ======= PERFIS FUNCIONALIDADES =======
            CreateMap<PerfisFuncionalidades, PerfisFuncionalidadesDTO>()
                .ForMember(dest => dest.NomePerfil, opt => opt.MapFrom(src => src.Perfil.Nome))
                .ForMember(dest => dest.NomeFuncionalidade, opt => opt.MapFrom(src => src.Funcionalidade.Nome))
                .ReverseMap();

            CreateMap<PerfisFuncionalidades, FuncionalidadeDTO>().ReverseMap();
            CreateMap<Funcionalidade, FuncionalidadeDTO>().ReverseMap();
            CreateMap<FuncionalidadeModel, FuncionalidadeDTO>().ReverseMap();

            // ======= FUNCIONÁRIOS PERFIS =======
           

            // ======= USUÁRIOS E LOGIN =======
            CreateMap<DepInfra.Usuarios, UsuarioDTO>().ReverseMap();
            CreateMap<Usuarios, UsuarioDTO>().ReverseMap();
            CreateMap<LoginModel, LoginDTO>().ReverseMap();
            CreateMap<Login, LoginDTO>()
                .ForMember(dest => dest.valido, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            // ======= PERMUTAS =======
            CreateMap<PermutaModel, PermutasDTO>().ReverseMap();
            CreateMap<Permuta, PermutasDTO>()
                .ForMember(x => x.DtDataSolicitadaTroca, opt => opt.MapFrom(src => src.DtDataSolicitadaTroca))
                .ForMember(x => x.valido, opt => opt.MapFrom(src => true))
                .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            // ======= DEPARTAMENTOS =======
            CreateMap<DepartamentoModel, DepartamentoDTO>().ReverseMap();
            CreateMap<Departamento, DepartamentoDTO>()
                .ForMember(x => x.DtCriacao, opt => opt.MapFrom(src => src.DtCriacao))
                .ForMember(x => x.valido, opt => opt.MapFrom(src => true))
                .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            // ======= ESCALA =======
            CreateMap<EscalaModel, EscalaDTO>().ReverseMap();
            CreateMap<Escala, EscalaDTO>()
                .ForMember(x => x.DtCriacao, opt => opt.MapFrom(src => src.DtCriacao))
                .ForMember(x => x.valido, opt => opt.MapFrom(src => true))
                .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            // ======= POSTOS DE TRABALHO =======
            CreateMap<PostoTrabalhoModel, PostoTrabalhoDTO>().ReverseMap();
            CreateMap<PostoTrabalho, PostoTrabalhoDTO>()
                .ForMember(x => x.valido, opt => opt.MapFrom(src => true))
                .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            // ======= TIPOS DE ESCALA =======
            CreateMap<TipoEscalaModel, TipoEscalaDTO>().ReverseMap();
            CreateMap<TipoEscala, TipoEscalaDTO>()
                .ForMember(x => x.valido, opt => opt.MapFrom(src => true))
                .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            // ======= ESCALA PRONTA =======
            CreateMap<EscalaProntaModel, EscalaProntaDTO>().ReverseMap();
            CreateMap<EscalaPronta, EscalaProntaDTO>()
                .ForMember(x => x.valido, opt => opt.MapFrom(src => true))
                .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            CreateMap<EscalaProntaDTO, EscalaPronta>()
                .ForMember(dest => dest.DtCriacao, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.DtCriacao, DateTimeKind.Utc)))
                .ForMember(dest => dest.DtDataServico, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.DtDataServico, DateTimeKind.Utc)))
                .ReverseMap();
        }
    }
}
