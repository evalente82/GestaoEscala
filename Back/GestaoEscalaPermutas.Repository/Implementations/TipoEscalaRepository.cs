using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class TipoEscalaRepository : ITipoEscalaRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public TipoEscalaRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task<TipoEscala> IncluirAsync(TipoEscala tipoEscala)
        {
            await _context.TipoEscalas.AddAsync(tipoEscala);
            await _context.SaveChangesAsync();
            return tipoEscala;
        }

        public async Task<TipoEscala> AlterarAsync(TipoEscala tipoEscala)
        {
            _context.TipoEscalas.Update(tipoEscala);
            await _context.SaveChangesAsync();
            return tipoEscala;
        }

        public async Task<TipoEscala> BuscarPorIdAsync(Guid id)
        {
            return await _context.TipoEscalas.FindAsync(id);
        }

        public async Task<List<TipoEscala>> BuscarTodosAsync()
        {
            return await _context.TipoEscalas.ToListAsync();
        }

        public async Task<bool> DeletarAsync(Guid id)
        {
            var tipoEscalaExistente = await _context.TipoEscalas.FindAsync(id);
            if (tipoEscalaExistente == null)
                return false;

            _context.TipoEscalas.Remove(tipoEscalaExistente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
