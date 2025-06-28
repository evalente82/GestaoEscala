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

        public async Task AdicionarListaAsync(EscalaExtra escalaExtra)
        {
            await _context.EscalaExtra.AddRangeAsync(escalaExtra);            
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

        public async Task<EscalaExtra?> BuscarPorIdAsync(Guid id)
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
            return await _context.EscalaExtra
                                 .Where(e => e.IdFuncionario == idFuncionario)
                                 .ToListAsync();
        }

        public Task<EscalaExtra?> BuscarPorIdEscalaExtra(Guid id)
        {
            return _context.EscalaExtra.FirstOrDefaultAsync(e => e.IdEscalaExtra == id);
        }

        public async Task<EscalaExtra?> ObterProximoDaFilaAsync(Guid idCriacaoEscalaExtra)
        {
            return await _context.EscalaExtra
                .Where(e => e.IdCriacaoEscalaExtra == idCriacaoEscalaExtra && e.StatusInscricao == "FilaDeEspera")
                .OrderBy(e => e.DtCriacao)
                .FirstOrDefaultAsync();
        }

        public async Task<List<EscalaExtra>> ObterInscricoesPorFuncionarioEData(Guid idFuncionario, DateTime data)
        {
            // A comparação .Date garante que estamos comparando apenas o dia, mês e ano, ignorando a hora.
            return await _context.EscalaExtra
                .Where(i => i.IdFuncionario == idFuncionario && i.DtCriacao.Date == data.Date)
                .ToListAsync();
        }
    }
}
