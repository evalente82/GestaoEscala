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

        public EscalaExtraRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task<CriacaoEscalaExtra[]> AdicionarListaAsync(CriacaoEscalaExtra[] escalaExtra)
        {
            await _context.CriacaoEscalaExtra.AddRangeAsync(escalaExtra);
            await _context.SaveChangesAsync();
            return escalaExtra;
        }

        public async Task<EscalaExtra> ObterPorIdAsync(Guid id)
        {
            return await _context.EscalaExtras.FindAsync(id);
        }

        public async Task<List<CriacaoEscalaExtra>> ObterTodosAsync()
        {
            return await _context.CriacaoEscalaExtra.ToListAsync();
        }

        public async Task<bool> DeletarAsync(Guid id)
        {
            var EscalaExtraExistente = await _context.CriacaoEscalaExtra.FindAsync(id);
            if (EscalaExtraExistente == null)
                return false;

            _context.CriacaoEscalaExtra.Remove(EscalaExtraExistente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CriacaoEscalaExtra> BuscarPorIdAsync(Guid id)
        {
            return await _context.CriacaoEscalaExtra.FindAsync(id);
        }

        public async Task<CriacaoEscalaExtra> AlterarAsync(CriacaoEscalaExtra escalaExtra)
        {
            _context.CriacaoEscalaExtra.Update(escalaExtra);
            await _context.SaveChangesAsync();
            return escalaExtra;
        }
    }
}
