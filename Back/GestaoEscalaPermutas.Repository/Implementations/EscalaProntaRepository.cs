
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class EscalaProntaRepository : IEscalaProntaRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public EscalaProntaRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task<EscalaPronta> AdicionarAsync(EscalaPronta escalaPronta)
        {
            _context.EscalaPronta.Add(escalaPronta);
            await _context.SaveChangesAsync();
            return escalaPronta;
        }

        public async Task<EscalaPronta[]> AdicionarListaAsync(EscalaPronta[] escalasProntas)
        {
            _context.EscalaPronta.AddRange(escalasProntas);
            await _context.SaveChangesAsync();
            return escalasProntas;
        }

        public async Task<EscalaPronta?> ObterPorIdAsync(Guid id)
        {
            return await _context.EscalaPronta.FindAsync(id);
        }

        public async Task<List<EscalaPronta>> ObterPorEscalaIdAsync(Guid idEscalaPronta)
        {
            return await _context.EscalaPronta
                .Where(x => x.IdEscala == idEscalaPronta)
                .OrderBy(x => x.DtDataServico)
                .ThenBy(x => x.IdPostoTrabalho)
                .ThenBy(x => x.IdFuncionario)
                .ToListAsync();
        }

        public async Task<List<EscalaPronta>> ObterTodosAsync()
        {
            return await _context.EscalaPronta.ToListAsync();
        }

        public async Task AtualizarAsync(EscalaPronta escalaPronta)
        {
            _context.EscalaPronta.Update(escalaPronta);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(Guid id)
        {
            var escalaPronta = await _context.EscalaPronta.FindAsync(id);
            if (escalaPronta != null)
            {
                _context.EscalaPronta.Remove(escalaPronta);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoverListaPorEscalaAsync(Guid idEscala)
        {
            var escalasProntas = await ObterPorEscalaIdAsync(idEscala);
            if (escalasProntas.Any())
            {
                _context.EscalaPronta.RemoveRange(escalasProntas);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<EscalaPronta>> BuscarPorIdFuncionario(Guid idFuncionario)
        {
            if (idFuncionario == Guid.Empty)
                return new List<EscalaPronta>(); // Retorna uma lista vazia se o ID for inválido

            return await _context.EscalaPronta
                .Where(ep => ep.IdFuncionario == idFuncionario)
                .Include(ep => ep.Escala) // Inclui os dados da escala associada
                .ToListAsync();
        }

    }
}
