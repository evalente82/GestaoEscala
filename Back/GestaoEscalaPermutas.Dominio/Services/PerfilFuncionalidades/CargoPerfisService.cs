using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfilFuncionalidades;
using GestaoEscalaPermutas.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.CargoPerfis
{
    public class CargoPerfisService : ICargoPerfisService
    {
        private readonly DefesaCivilMaricaContext _context;
        private readonly IMapper _mapper;

        public CargoPerfisService(DefesaCivilMaricaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CargoPerfilDTO>> BuscarTodos()
        {
            var cargoPerfis = await _context.CargoPerfis
                .Include(cp => cp.Cargo)
                .Include(cp => cp.Perfil)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CargoPerfilDTO>>(cargoPerfis);
        }

        public async Task<bool> AtribuirPerfilAoCargo(Guid idCargo, Guid idPerfil)
        {
            var existeRelacionamento = await _context.CargoPerfis
                .AnyAsync(cp => cp.IdCargo == idCargo && cp.IdPerfil == idPerfil);

            if (existeRelacionamento)
                return false;

            var novoRelacionamento = new DepInfra.CargoPerfis
            {
                IdCargo = idCargo,
                IdPerfil = idPerfil
            };

            _context.CargoPerfis.Add(novoRelacionamento);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoverPerfilDoCargo(Guid idCargo, Guid idPerfil)
        {
            var relacionamento = await _context.CargoPerfis
                .FirstOrDefaultAsync(cp => cp.IdCargo == idCargo && cp.IdPerfil == idPerfil);

            if (relacionamento == null)
                return false;

            _context.CargoPerfis.Remove(relacionamento);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CargoPerfilDTO>> BuscarPerfisPorCargo(Guid idCargo)
        {
            var perfis = await _context.CargoPerfis
                .Include(cp => cp.Perfil)
                .Where(cp => cp.IdCargo == idCargo)
                .Select(cp => cp.Perfil)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CargoPerfilDTO>>(perfis);
        }
    }
}
