using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class PostoTrabalhoRepository : IPostoTrabalhoRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public PostoTrabalhoRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task<PostoTrabalho> IncluirAsync(PostoTrabalho postoTrabalho)
        {
            await _context.PostoTrabalhos.AddAsync(postoTrabalho);
            await _context.SaveChangesAsync();
            return postoTrabalho;
        }

        public async Task<PostoTrabalho> AlterarAsync(PostoTrabalho postoTrabalho)
        {
            _context.PostoTrabalhos.Update(postoTrabalho);
            await _context.SaveChangesAsync();
            return postoTrabalho;
        }

        public async Task<PostoTrabalho> BuscarPorIdAsync(Guid id)
        {
            return await _context.PostoTrabalhos.FindAsync(id);
        }

        public async Task<List<PostoTrabalho>> BuscarTodosAsync()
        {
            return await _context.PostoTrabalhos.ToListAsync();
        }

        public async Task<bool> DeletarAsync(Guid id)
        {
            var postoExistente = await _context.PostoTrabalhos.FindAsync(id);
            if (postoExistente == null)
                return false;

            _context.PostoTrabalhos.Remove(postoExistente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PostoTrabalho>> BuscarTodosAtivosAsync()
        {
            return await _context.PostoTrabalhos.Where(p => p.IsAtivo).ToListAsync();
        }

        public async Task<PostoTrabalho[]> IncluirListaAsync(PostoTrabalho[] postos)
        {
            await _context.PostoTrabalhos.AddRangeAsync(postos);
            await _context.SaveChangesAsync();
            return postos;
        }
    }
}
