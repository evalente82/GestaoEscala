using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class DepartamentoRepository : IDepartamentoRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public DepartamentoRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task<Departamento> AdicionarAsync(Departamento departamento)
        {
            _context.Departamentos.Add(departamento);
            await _context.SaveChangesAsync();
            return departamento;
        }

        public async Task<Departamento?> ObterPorIdAsync(Guid id)
        {
            return await _context.Departamentos.FindAsync(id);
        }

        public async Task<List<Departamento>> ObterTodosAsync()
        {
            return await _context.Departamentos.ToListAsync();
        }

        public async Task AtualizarAsync(Departamento departamento)
        {
            _context.Departamentos.Update(departamento);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(Guid id)
        {
            var departamento = await _context.Departamentos.FindAsync(id);
            if (departamento != null)
            {
                _context.Departamentos.Remove(departamento);
                await _context.SaveChangesAsync();
            }
        }
    }
}
