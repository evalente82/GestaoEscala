using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class CargoRepository(DefesaCivilMaricaContext context) : ICargoRepository
    {
        private readonly DefesaCivilMaricaContext _context = context;

        public async Task<Cargo> AdicionarAsync(Cargo cargo)
        {
            _context.Cargos.Add(cargo);
            await _context.SaveChangesAsync();
            return cargo;
        }

        public async Task<Cargo?> ObterPorIdAsync(Guid id)
        {
            return await _context.Cargos.FindAsync(id);
        }

        public async Task<List<Cargo>> ObterTodosAsync()
        {
            return await _context.Cargos.ToListAsync();
        }

        public async Task AtualizarAsync(Cargo cargo)
        {
            _context.Cargos.Update(cargo);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(Guid id)
        {
            var cargo = await _context.Cargos.FindAsync(id);
            if (cargo != null)
            {
                _context.Cargos.Remove(cargo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
