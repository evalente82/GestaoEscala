using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class FeriadoRepository : IFeriadoRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public FeriadoRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Feriado feriado)
        {
            await _context.Feriados.AddAsync(feriado);
        }

        public async Task<IEnumerable<Feriado>> ObterPorAnoAsync(int ano)
        {
            return await _context.Feriados
                .Where(f => f.Data.Year == ano)
                .ToListAsync();
        }
    }
}