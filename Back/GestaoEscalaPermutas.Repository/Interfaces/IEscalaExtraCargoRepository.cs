using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface IEscalaExtraCargoRepository
    {
        Task AdicionarListaExtraCargosAsync(List<CriacaoEscalaExtraCargo> listaDeCargos);
        void DeletarAsync(CriacaoEscalaExtraCargo escalaExtra);
        Task AlterarAsync(CriacaoEscalaExtraCargo escalaExtra);
        Task<CriacaoEscalaExtraCargo?> ObterPorIdAsync(Guid id);
        Task<List<CriacaoEscalaExtraCargo>> ObterTodosAsync();
        Task<IEnumerable<Guid>> ObterCargosPorEscalaExtraIdAsync(Guid idCriacaoEscalaExtra);
    }
}
