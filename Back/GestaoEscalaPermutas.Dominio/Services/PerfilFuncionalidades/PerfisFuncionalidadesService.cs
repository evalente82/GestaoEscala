using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfisFuncionalidades;
using GestaoEscalaPermutas.Repository.Interfaces;
namespace GestaoEscalaPermutas.Dominio.Services.PerfisFuncionalidades
{
    public class PerfisFuncionalidadesService : IPerfisFuncionalidadesService
    {
        private readonly IPerfisFuncionalidadesRepository _perfisFuncionalidadesRepository;
        private readonly IMapper _mapper;

        public PerfisFuncionalidadesService(IPerfisFuncionalidadesRepository perfisFuncionalidadesRepository, IMapper mapper)
        {
            _perfisFuncionalidadesRepository = perfisFuncionalidadesRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PerfisFuncionalidadesDTO>> BuscarTodas()
        {
            var perfisFuncionalidades = await _perfisFuncionalidadesRepository.BuscarTodasAsync();
            return _mapper.Map<IEnumerable<PerfisFuncionalidadesDTO>>(perfisFuncionalidades);
        }

        public async Task<bool> AtribuirFuncionalidadeAoPerfil(Guid idPerfil, Guid idFuncionalidade)
        {
            return await _perfisFuncionalidadesRepository.AtribuirFuncionalidadeAoPerfilAsync(idPerfil, idFuncionalidade);
        }

        public async Task<bool> RemoverFuncionalidadeDoPerfil(Guid idPerfil, Guid idFuncionalidade)
        {
            return await _perfisFuncionalidadesRepository.RemoverFuncionalidadeDoPerfilAsync(idPerfil, idFuncionalidade);
        }

        public async Task<IEnumerable<FuncionalidadeDTO>> BuscarFuncionalidadesPorPerfil(Guid idPerfil)
        {
            var funcionalidades = await _perfisFuncionalidadesRepository.BuscarFuncionalidadesPorPerfilAsync(idPerfil);
            return _mapper.Map<IEnumerable<FuncionalidadeDTO>>(funcionalidades);
        }
    }
}
