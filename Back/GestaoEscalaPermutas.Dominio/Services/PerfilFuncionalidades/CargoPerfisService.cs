using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO;
using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using GestaoEscalaPermutas.Dominio.Interfaces;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfilFuncionalidades;
using GestaoEscalaPermutas.Repository.Interfaces;

namespace GestaoEscalaPermutas.Dominio.Services.CargoPerfis
{
    public class CargoPerfisService : ICargoPerfisService
    {
        private readonly ICargoPerfisRepository _cargoPerfisRepository;
        private readonly IMapper _mapper;

        public CargoPerfisService(ICargoPerfisRepository cargoPerfisRepository, IMapper mapper)
        {
            _cargoPerfisRepository = cargoPerfisRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CargoPerfilDTO>> BuscarTodos()
        {
            var cargoPerfis = await _cargoPerfisRepository.BuscarTodosAsync();
            return _mapper.Map<IEnumerable<CargoPerfilDTO>>(cargoPerfis);
        }

        public async Task<bool> AtribuirPerfilAoCargo(Guid idCargo, Guid idPerfil)
        {
            return await _cargoPerfisRepository.AtribuirPerfilAoCargoAsync(idCargo, idPerfil);
        }

        public async Task<bool> RemoverPerfilDoCargo(Guid idCargo, Guid idPerfil)
        {
            return await _cargoPerfisRepository.RemoverPerfilDoCargoAsync(idCargo, idPerfil);
        }

        public async Task<IEnumerable<CargoPerfilDTO>> BuscarPerfisPorCargo(Guid idCargo)
        {
            var perfis = await _cargoPerfisRepository.BuscarPerfisPorCargoAsync(idCargo);
            return _mapper.Map<IEnumerable<CargoPerfilDTO>>(perfis);
        }
    }
}
