using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Expõe os seus repositórios aqui
        IEscalaExtraCargoRepository EscalaExtraCargo { get; }
        //colocar todos que faltam
        ICriacaoEscalaExtraRepository CriacaoEscalaExtra { get; }
        Task<int> CompleteAsync();
    }
}
