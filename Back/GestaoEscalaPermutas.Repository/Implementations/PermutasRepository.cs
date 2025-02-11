using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class PermutasRepository : IPermutasRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public PermutasRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task<Permuta> IncluirAsync(Permuta permuta)
        {
            await _context.Permuta.AddAsync(permuta);
            await _context.SaveChangesAsync();
            return permuta;
        }

        public async Task<Permuta> AlterarAsync(Permuta permuta)
        {
            _context.Permuta.Update(permuta);
            await _context.SaveChangesAsync();
            return permuta;
        }

        public async Task<Permuta> BuscarPorIdAsync(Guid idPermuta)
        {
            return await _context.Permuta.FindAsync(idPermuta);
        }

        public async Task<List<Permuta>> BuscarTodosAsync()
        {
            return await _context.Permuta.ToListAsync();
        }

        public async Task<bool> DeletarAsync(Guid idPermuta)
        {
            var permutaExistente = await _context.Permuta.FindAsync(idPermuta);
            if (permutaExistente == null)
                return false;

            _context.Permuta.Remove(permutaExistente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Permuta>> BuscarFuncPorIdAsync(Guid idFuncionario)
        {
            if (idFuncionario == Guid.Empty)
                return new List<Permuta>(); // Retorna uma lista vazia se o ID for inválido

            return await _context.Permuta
                .Where(f => f.IdFuncionarioSolicitante == idFuncionario)
                .ToListAsync();
        }
    }
}
