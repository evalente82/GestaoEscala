using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class CriacaoEscalaExtraRepository : ICriacaoEscalaExtraRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public CriacaoEscalaExtraRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task AdicionarListaAsync(CriacaoEscalaExtra escalaExtra)
        {
            await _context.CriacaoEscalaExtra.AddRangeAsync(escalaExtra);
        }

        public void DeletarAsync(CriacaoEscalaExtra escalaExtra)
        {
            _context.CriacaoEscalaExtra.Remove(escalaExtra);
        }

        public async Task<CriacaoEscalaExtra?> ObterPorIdAsync(Guid id)
        {
            return await _context.CriacaoEscalaExtra.FindAsync(id);
        }

        public async Task<List<CriacaoEscalaExtra>> ObterTodosComCargosAsync()
        {
            var lista =  await _context.CriacaoEscalaExtra
                .Include(e => e.CriacaoEscalaExtraCargos)
                    .ThenInclude(c => c.Cargo)
                .ToListAsync();
            
            return lista;
        }
    }
}
