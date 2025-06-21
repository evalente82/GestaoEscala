using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface IEscalaExtraRepository
    {
        Task<EscalaExtra> ObterPorIdAsync(Guid id);
        Task<List<CriacaoEscalaExtra>> ObterTodosAsync();
        Task<CriacaoEscalaExtra> AdicionarListaAsync(CriacaoEscalaExtra escalaExtra);
        Task<bool> DeletarAsync(Guid id);
        Task<CriacaoEscalaExtra> BuscarListaPorIdAsync(Guid id);
        Task<CriacaoEscalaExtra> AlterarAsync(CriacaoEscalaExtra escalaExtra);
    }
}
