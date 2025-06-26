using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface IEscalaExtraRepository
    {
        Task<CriacaoEscalaExtra> ObterPorIdAsync(Guid id);
        Task<List<CriacaoEscalaExtra>> ObterTodosAsync();
        Task AdicionarListaAsync(CriacaoEscalaExtra escalaExtra);
        void DeletarAsync(CriacaoEscalaExtra escalaExtra);        
        Task<CriacaoEscalaExtra> BuscarListaPorIdAsync(Guid id);
        Task AlterarAsync(CriacaoEscalaExtra escalaExtra);
        Task<List<CriacaoEscalaExtra>> ObterTodosComCargosAsync();
    }
}
