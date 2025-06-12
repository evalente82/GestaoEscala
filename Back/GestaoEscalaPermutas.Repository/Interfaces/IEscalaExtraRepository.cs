using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface IEscalaExtraRepository
    {
        Task<EscalaExtra> ObterPorIdAsync(Guid id);
        Task<List<CriacaoEscalaExtra>> ObterTodosAsync();
        Task<CriacaoEscalaExtra[]> AdicionarListaAsync(CriacaoEscalaExtra[] escalaExtra);
        Task<bool> DeletarAsync(Guid id);
        Task<CriacaoEscalaExtra> BuscarPorIdAsync(Guid id);
        Task<CriacaoEscalaExtra> AlterarAsync(CriacaoEscalaExtra escalaExtra);
    }
}
