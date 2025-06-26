using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DefesaCivilMaricaContext _context;

        // Instancia os repositórios, passando o contexto.
        public ICriacaoEscalaExtraRepository CriacaoEscalaExtra { get; private set; }
        public IEscalaExtraCargoRepository EscalaExtraCargo { get; private set; }


        public UnitOfWork(DefesaCivilMaricaContext context)
        {
            _context = context;
            CriacaoEscalaExtra = new CriacaoEscalaExtraRepository(_context);
            EscalaExtraCargo = new EscalaExtraCargoRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            // A única chamada ao SaveChangesAsync acontece aqui!
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
