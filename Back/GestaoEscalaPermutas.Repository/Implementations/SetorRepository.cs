using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class SetorRepository : ISetorRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public SetorRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task<Setor> IncluirAsync(Setor setor)
        {
            await _context.Setor.AddAsync(setor);
            await _context.SaveChangesAsync();
            return setor;
        }

        public async Task<Setor> AlterarAsync(Setor setor)
        {
            _context.Setor.Update(setor);
            await _context.SaveChangesAsync();
            return setor;
        }

        public async Task<Setor> BuscarPorIdAsync(Guid id)
        {
            return await _context.Setor.FindAsync(id);
        }

        public async Task<List<Setor>> BuscarTodosAsync()
        {
            return await _context.Setor.ToListAsync();
        }

        public async Task<bool> DeletarAsync(Guid id)
        {
            var setorExistente = await _context.Setor.FindAsync(id);
            if (setorExistente == null)
                return false;

            _context.Setor.Remove(setorExistente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
