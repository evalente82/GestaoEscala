using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class FuncionalidadeRepository : IFuncionalidadeRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public FuncionalidadeRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task<Funcionalidade> CriarAsync(Funcionalidade funcionalidade)
        {
            await _context.Funcionalidades.AddAsync(funcionalidade);
            await _context.SaveChangesAsync();
            return funcionalidade;
        }

        public async Task<Funcionalidade> AtualizarAsync(Funcionalidade funcionalidade)
        {
            _context.Funcionalidades.Update(funcionalidade);
            await _context.SaveChangesAsync();
            return funcionalidade;
        }

        public async Task<bool> DeletarAsync(Guid id)
        {
            var funcionalidade = await _context.Funcionalidades.FindAsync(id);
            if (funcionalidade == null)
                return false;

            _context.Funcionalidades.Remove(funcionalidade);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Funcionalidade>> BuscarTodasAsync()
        {
            return await _context.Funcionalidades.OrderBy(x => x.Nome).ToListAsync();
        }

        public async Task<Funcionalidade?> BuscarPorIdAsync(Guid id)
        {
            return await _context.Funcionalidades.FindAsync(id);
        }
    }
}
