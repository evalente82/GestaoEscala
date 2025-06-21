using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class SolicitacaoEscalaExtraRepository : ISolicitacaoEscalaExtraRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public SolicitacaoEscalaExtraRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task<EscalaExtra> AdicionarListaAsync(EscalaExtra escalaExtra)
        {
            await _context.EscalaExtra.AddRangeAsync(escalaExtra);
            await _context.SaveChangesAsync();
            var dadosDoBanco = await _context.EscalaExtra.FirstOrDefaultAsync(x => x.IdEscalaExtra == escalaExtra.IdEscalaExtra);
            return dadosDoBanco;
        }

        public async Task<List<EscalaExtra>> ObterTodosAsync()
        {
            return await _context.EscalaExtra.ToListAsync();
        }

        public async Task<bool> DeletarAsync(Guid id)
        {
            var EscalaExtraExistente = await _context.EscalaExtra.FindAsync(id);
            if (EscalaExtraExistente == null)
                return false;

            _context.EscalaExtra.Remove(EscalaExtraExistente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<EscalaExtra> BuscarPorIdAsync(Guid id)
        {
            return await _context.EscalaExtra.FindAsync(id);
        }

        public async Task<EscalaExtra> AlterarAsync(EscalaExtra escalaExtra)
        {
            _context.EscalaExtra.Update(escalaExtra);
            await _context.SaveChangesAsync();
            return escalaExtra;
        }

        public async Task<List<EscalaExtra>> ObterListaPorIdFuncionario(Guid idFuncionario)
        {
            // Busca todas as ocorrências da tabela EscalaExtra associadas ao idFuncionario
            return await _context.EscalaExtra
                                 .Where(e => e.IdFuncionario == idFuncionario)
                                 .ToListAsync();
        }        
    }
}
