using GestaoEscalaPermutas.Repository.Interfaces;
using System;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IEscalaRepository Escalas { get; }
        IUsuarioRepository Usuarios { get; }
        Task<int> SaveChangesAsync();
    }
}
