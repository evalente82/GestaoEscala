using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class EscalaRepository : IEscalaRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public EscalaRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task<Escala> AdicionarAsync(Escala escala)
        {
            _context.Escalas.Add(escala);
            await _context.SaveChangesAsync();
            return escala;
        }

        public async Task<Escala?> ObterPorIdAsync(Guid id)
        {
            return await _context.Escalas.FindAsync(id);
        }

        public async Task<List<Escala>> ObterTodasAsync()
        {
            return await _context.Escalas.ToListAsync();
        }

        public async Task AtualizarAsync(Escala escala)
        {
            _context.Escalas.Update(escala);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(Guid id)
        {
            var escala = await _context.Escalas.FindAsync(id);
            if (escala != null)
            {
                _context.Escalas.Remove(escala);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<EscalaPronta>> ObterEscalasProntasPorEscalaId(Guid idEscala)
        {
            return await _context.EscalaPronta.Where(x => x.IdEscala == idEscala).ToListAsync();
        }

        public async Task RemoverEscalasProntasPorEscalaId(Guid idEscala)
        {
            var escalasProntas = await ObterEscalasProntasPorEscalaId(idEscala);
            if (escalasProntas.Any())
            {
                _context.EscalaPronta.RemoveRange(escalasProntas);
                await _context.SaveChangesAsync();
            }
        }
    }
}
