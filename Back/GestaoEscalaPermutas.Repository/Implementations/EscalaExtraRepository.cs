using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class EscalaExtraRepository: IEscalaExtraRepository
    {
        private readonly DefesaCivilMaricaContext _context;
        public async Task<EscalaExtra[]> AdicionarListaAsync(EscalaExtra[] escalaExtra)
        {
            await _context.EscalaExtras.AddRangeAsync(escalaExtra);
            await _context.SaveChangesAsync();
            return escalaExtra;
        }

        public async Task<EscalaExtra> ObterPorIdAsync(Guid id)
        {
            return await _context.EscalaExtras.FindAsync(id);
        }

        public async Task<List<EscalaExtra>> ObterTodosAsync()
        {
            return await _context.EscalaExtras.OrderBy(x => x.DtServico).ToListAsync();
        }
    }
}
