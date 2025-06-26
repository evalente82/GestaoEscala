using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DefesaCivilMaricaContext _context;

        // Instancia os repositórios, passando o contexto.
        public IEscalaExtraRepository EscalaExtra { get; private set; }
        public IEscalaExtraCargoRepository EscalaExtraCargo { get; private set; }

        public UnitOfWork(DefesaCivilMaricaContext context)
        {
            _context = context;
            EscalaExtra = new EscalaExtraRepository(_context);
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
