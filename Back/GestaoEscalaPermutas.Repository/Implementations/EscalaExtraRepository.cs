using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class EscalaExtraRepository: IEscalaExtraRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public EscalaExtraRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task AdicionarListaAsync(CriacaoEscalaExtra escalaExtra)
        {
            await _context.CriacaoEscalaExtra.AddRangeAsync(escalaExtra);
        }

        public async Task<CriacaoEscalaExtra> ObterPorIdAsync(Guid id)
        {
            return await _context.CriacaoEscalaExtra.FindAsync(id);
        }

        public async Task<List<CriacaoEscalaExtra>> ObterTodosAsync()
        {
            return await _context.CriacaoEscalaExtra.ToListAsync();
        }

        public void DeletarAsync(CriacaoEscalaExtra escalaExtra)
        {
            // Apenas marca a entidade para ser removida na próxima vez que SaveChanges for chamado.
            _context.CriacaoEscalaExtra.Remove(escalaExtra);
        }

        public Task AlterarAsync(CriacaoEscalaExtra escalaExtra)
        {
            _context.CriacaoEscalaExtra.Update(escalaExtra);
            return Task.CompletedTask;
        }

        public async Task<CriacaoEscalaExtra> BuscarListaPorIdAsync(Guid id)
        {
            return await _context.CriacaoEscalaExtra.FindAsync(id);
        }

        public Task<List<CriacaoEscalaExtra>> ObterTodosComCargosAsync()
        {
            return  _context.CriacaoEscalaExtra // Começa na tabela principal
                             .Include(escala => escala.CriacaoEscalaExtraCargos)
                             .ThenInclude(cec => cec.Cargo)// Inclui os dados relacionados da tabela de junção
                             .ToListAsync(); // Executa uma única query otimizada no banco
        }

        
    }
}
