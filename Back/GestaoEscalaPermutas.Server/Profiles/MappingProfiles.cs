using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Cargo;
using GestaoEscalaPermutas.Dominio.DTO.Departamento;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.EscalaPronta;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.DTO.Login;
using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using GestaoEscalaPermutas.Dominio.DTO.Permutas;
using GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.DTO.Setor;
using GestaoEscalaPermutas.Dominio.DTO.TipoEscala;
using GestaoEscalaPermutas.Dominio.DTO.Usuario;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Server.Models;
using GestaoEscalaPermutas.Server.Models.Cargos;
using GestaoEscalaPermutas.Server.Models.Departamento;
using GestaoEscalaPermutas.Server.Models.Escala;
using GestaoEscalaPermutas.Server.Models.EscalaPronta;
using GestaoEscalaPermutas.Server.Models.Funcionarios;
using GestaoEscalaPermutas.Server.Models.Login;
using GestaoEscalaPermutas.Server.Models.PerfilFuncionalidade;
using GestaoEscalaPermutas.Server.Models.Permuta;
using GestaoEscalaPermutas.Server.Models.PostoTrabalho;
using GestaoEscalaPermutas.Server.Models.Setor;
using GestaoEscalaPermutas.Server.Models.TipoEscala;

namespace GestaoEscalaPermutas.Dominio.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // ======= SETOR =======
            CreateMap<SetorDTO, SetorModel>().ReverseMap();
            CreateMap<SetorDTO, Setor>().ReverseMap();

            // ======= FUNCIONÁRIOS =======
            CreateMap<Funcionario, FuncionarioDTO>()
                .ForMember(dest => dest.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            CreateMap<Funcionario, FuncionarioDTO>()
                .ForMember(dest => dest.IdFuncionario, opt => opt.MapFrom(src => src.IdFuncionario))
                .ForMember(dest => dest.NmNome, opt => opt.MapFrom(src => src.NmNome))
                .ForMember(dest => dest.IdCargo, opt => opt.MapFrom(src => src.IdCargo))
                .ReverseMap();

            CreateMap<FuncionarioModel, FuncionarioDTO>().ReverseMap();

            // ======= CARGOS =======
            CreateMap<CargoModel, CargoDTO>().ReverseMap();
            CreateMap<Cargo, CargoDTO>()
                .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            // ======= PERFIS =======
            CreateMap<Perfil, PerfilDTO>().ReverseMap();
            CreateMap<PerfilDTO, PerfilModel>().ReverseMap();
            CreateMap<Perfil, PerfilDTO>().ReverseMap();

            // ======= PERFIS FUNCIONALIDADES =======
            CreateMap<PerfisFuncionalidades, PerfisFuncionalidadesDTO>()
                .ForMember(dest => dest.NomePerfil, opt => opt.MapFrom(src => src.Perfil.Nome))
                .ForMember(dest => dest.NomeFuncionalidade, opt => opt.MapFrom(src => src.Funcionalidade.Nome))
                .ReverseMap();

            CreateMap<PerfisFuncionalidades, FuncionalidadeDTO>().ReverseMap();
            CreateMap<Funcionalidade, FuncionalidadeDTO>().ReverseMap();
            CreateMap<FuncionalidadeModel, FuncionalidadeDTO>().ReverseMap();

            // ======= CARGOS PERFIS =======
            CreateMap<CargoPerfis, CargoPerfilDTO>()
                .ForMember(dest => dest.IdCargo, opt => opt.MapFrom(src => src.IdCargo))
                .ForMember(dest => dest.NomeCargo, opt => opt.MapFrom(src => src.Cargo.NmNome))
                .ForMember(dest => dest.IdPerfil, opt => opt.MapFrom(src => src.IdPerfil))
                .ForMember(dest => dest.NomePerfil, opt => opt.MapFrom(src => src.Perfil.Nome))
                .ReverseMap();

            // ======= USUÁRIOS E LOGIN =======
            CreateMap<LoginDTO, Usuarios>()
                .ForMember(dest => dest.Nome, opt => opt.Ignore())
                .ForMember(dest => dest.SenhaHash, opt => opt.MapFrom(src => src.SenhaHash))
                .ForMember(dest => dest.Perfil, opt => opt.Ignore());

            CreateMap<Usuarios, LoginDTO>()
                .ForMember(dest => dest.Senha, opt => opt.Ignore())
                .ForMember(dest => dest.SenhaHash, opt => opt.MapFrom(src => src.SenhaHash))
                .ForMember(dest => dest.Perfil, opt => opt.MapFrom(src => src.Perfil.Nome ?? "Sem Perfil"));

            CreateMap<LoginResponseDTO, LoginModel>()
                .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.NomeUsuario));

            CreateMap<LoginDTO, LoginModel>()
                .ForMember(dest => dest.SenhaHash, opt => opt.AllowNull())
                .ForMember(dest => dest.Perfil, opt => opt.AllowNull());

            CreateMap<Usuarios, UsuarioDTO>().ReverseMap();
            CreateMap<Usuarios, UsuarioDTO>().ReverseMap();
            CreateMap<LoginModel, LoginDTO>().ReverseMap();
            CreateMap<Login, LoginDTO>()
                .ForMember(dest => dest.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            // ======= PERMUTAS =======
            CreateMap<PermutaModel, PermutasDTO>().ReverseMap();
            CreateMap<Permuta, PermutasDTO>()
                .ForMember(x => x.DtDataSolicitadaTroca, opt => opt.MapFrom(src => src.DtDataSolicitadaTroca))
                .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            // Mapeamento de PermutasDTO para PermutaMensagemDTO
            CreateMap<PermutasDTO, PermutaMensagemDTO>()
                .ForMember(dest => dest.IdPermuta, opt => opt.MapFrom(src => src.IdPermuta))
                .ForMember(dest => dest.IdFuncionarioSolicitante, opt => opt.MapFrom(src => src.IdFuncionarioSolicitante))
                .ForMember(dest => dest.NmNomeSolicitante, opt => opt.MapFrom(src => src.NmNomeSolicitante))
                .ForMember(dest => dest.IdFuncionarioSolicitado, opt => opt.MapFrom(src => src.IdFuncionarioSolicitado))
                .ForMember(dest => dest.NmNomeSolicitado, opt => opt.MapFrom(src => src.NmNomeSolicitado))
                .ForMember(dest => dest.DtDataSolicitadaTroca, opt => opt.MapFrom(src => src.DtDataSolicitadaTroca.ToString("o")))
                .ForMember(dest => dest.NmStatus, opt => opt.MapFrom(src => src.NmStatus));

            // ======= DEPARTAMENTOS =======
            CreateMap<DepartamentoModel, DepartamentoDTO>().ReverseMap();
            CreateMap<Departamento, DepartamentoDTO>()
                .ForMember(x => x.DtCriacao, opt => opt.MapFrom(src => src.DtCriacao))
                .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            // ======= ESCALA =======
            CreateMap<EscalaModel, EscalaDTO>().ReverseMap();
            CreateMap<Escala, EscalaDTO>()
                .ForMember(x => x.DtCriacao, opt => opt.MapFrom(src => src.DtCriacao))
                .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            // ======= POSTOS DE TRABALHO =======
            CreateMap<PostoTrabalhoModel, PostoTrabalhoDTO>().ReverseMap();
            CreateMap<PostoTrabalho, PostoTrabalhoDTO>()
                .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            // ======= TIPOS DE ESCALA =======
            CreateMap<TipoEscalaModel, TipoEscalaDTO>().ReverseMap();
            CreateMap<TipoEscala, TipoEscalaDTO>()
                .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            // ======= ESCALA PRONTA =======
            CreateMap<EscalaProntaModel, EscalaProntaDTO>().ReverseMap();
            CreateMap<EscalaPronta, EscalaProntaDTO>()
                .ForMember(x => x.mensagem, opt => opt.MapFrom(src => "Registro recebido com sucesso"))
                .ReverseMap();

            CreateMap<EscalaProntaDTO, EscalaPronta>()
                .ForMember(dest => dest.DtCriacao, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.DtCriacao, DateTimeKind.Utc)))
                .ForMember(dest => dest.DtDataServico, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.DtDataServico, DateTimeKind.Utc)))
                .ReverseMap();
        }
    }
}