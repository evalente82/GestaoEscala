using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public UsuarioRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task<Usuarios> CriarAsync(Usuarios usuario)
        {
            await _context.Usuario.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuarios> BuscarPorIdAsync(Guid id)
        {
            return await _context.Usuario.FindAsync(id);
        }

        public async Task<IEnumerable<Usuarios>> BuscarTodosAsync()
        {
            return await _context.Usuario.ToListAsync();
        }

        public async Task<Usuarios> AtualizarAsync(Usuarios usuario)
        {
            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<bool> DeletarAsync(Guid id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
                return false;

            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VerificarUsuarioPorFuncionarioAsync(Guid idFuncionario)
        {
            return await _context.Usuario.AnyAsync(u => u.IdFuncionario == idFuncionario);
        }
    }
}
