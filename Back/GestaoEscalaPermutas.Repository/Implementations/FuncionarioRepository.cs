using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class FuncionarioRepository : IFuncionarioRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public FuncionarioRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task<Funcionario> AdicionarAsync(Funcionario funcionario)
        {
            await _context.Funcionarios.AddAsync(funcionario);
            await _context.SaveChangesAsync();
            return funcionario;
        }

        public async Task<Funcionario> AlterarAsync(Funcionario funcionario)
        {
            _context.Funcionarios.Update(funcionario);
            await _context.SaveChangesAsync();
            return funcionario;
        }

        public async Task<Funcionario> ObterPorIdAsync(Guid id)
        {
            return await _context.Funcionarios.FindAsync(id);
        }

        public async Task<List<Funcionario>> ObterTodosAsync()
        {
            return await _context.Funcionarios.OrderBy(x => x.NmNome).ToListAsync();
        }

        public async Task<List<Funcionario>> ObterTodosAtivosAsync()
        {
            return await _context.Funcionarios.Where(p => p.IsAtivo).ToListAsync();
        }

        public async Task RemoverAsync(Guid id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario != null)
            {
                _context.Funcionarios.Remove(funcionario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Funcionario[]> AdicionarListaAsync(Funcionario[] funcionarios)
        {
            await _context.Funcionarios.AddRangeAsync(funcionarios);
            await _context.SaveChangesAsync();
            return funcionarios;
        }

        public async Task<bool> MatriculaExisteAsync(int nrMatricula)
        {
            return await _context.Funcionarios.AnyAsync(f => f.NrMatricula == nrMatricula);
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            return await _context.Funcionarios.AnyAsync(f => f.NmEmail == email);
        }
    }
}
