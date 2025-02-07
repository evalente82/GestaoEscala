using System.Threading.Tasks;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Repository.Interfaces;

namespace GestaoEscalaPermutas.Repository.UnitOfWork
{
    public class UnitOfWorkService : IUnitOfWork
    {
        private readonly DefesaCivilMaricaContext _context;
        public IEscalaRepository Escalas { get; }
        public IUsuarioRepository Usuarios { get; }

        public UnitOfWorkService(DefesaCivilMaricaContext context, IEscalaRepository escalaRepository, IUsuarioRepository usuarioRepository)
        {
            _context = context;
            Escalas = escalaRepository;
            Usuarios = usuarioRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
