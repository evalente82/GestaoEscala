using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class CargoPerfisRepository : ICargoPerfisRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public CargoPerfisRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CargoPerfis>> BuscarTodosAsync()
        {
            return await _context.CargoPerfis
                .Include(cp => cp.Cargo)
                .Include(cp => cp.Perfil)
                .ToListAsync();
        }

        public async Task<bool> ExisteRelacionamentoAsync(Guid idCargo, Guid idPerfil)
        {
            return await _context.CargoPerfis
                .AnyAsync(cp => cp.IdCargo == idCargo && cp.IdPerfil == idPerfil);
        }

        public async Task<bool> AtribuirPerfilAoCargoAsync(Guid idCargo, Guid idPerfil)
        {
            if (await ExisteRelacionamentoAsync(idCargo, idPerfil))
                return false;

            var novoRelacionamento = new CargoPerfis
            {
                IdCargo = idCargo,
                IdPerfil = idPerfil
            };

            _context.CargoPerfis.Add(novoRelacionamento);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoverPerfilDoCargoAsync(Guid idCargo, Guid idPerfil)
        {
            var relacionamento = await _context.CargoPerfis
                .FirstOrDefaultAsync(cp => cp.IdCargo == idCargo && cp.IdPerfil == idPerfil);

            if (relacionamento == null)
                return false;

            _context.CargoPerfis.Remove(relacionamento);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Perfil>> BuscarPerfisPorCargoAsync(Guid idCargo)
        {
            return await _context.CargoPerfis
                .Include(cp => cp.Perfil)
                .Where(cp => cp.IdCargo == idCargo)
                .Select(cp => cp.Perfil)
                .ToListAsync();
        }
    }
}
