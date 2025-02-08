using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class PerfilRepository : IPerfilRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public PerfilRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Perfil>> BuscarTodosAsync()
        {
            return await _context.Perfil.ToListAsync();
        }

        public async Task<Perfil?> BuscarPorIdAsync(Guid idPerfil)
        {
            return await _context.Perfil.FindAsync(idPerfil);
        }

        public async Task<Perfil> CriarAsync(Perfil perfil)
        {
            perfil.IdPerfil = Guid.NewGuid();
            _context.Perfil.Add(perfil);
            await _context.SaveChangesAsync();
            return perfil;
        }

        public async Task<Perfil> AtualizarAsync(Perfil perfil)
        {
            _context.Perfil.Update(perfil);
            await _context.SaveChangesAsync();
            return perfil;
        }

        public async Task<bool> DeletarAsync(Guid idPerfil)
        {
            var perfil = await _context.Perfil.FindAsync(idPerfil);
            if (perfil == null)
                return false;

            _context.Perfil.Remove(perfil);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
