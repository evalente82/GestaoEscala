using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface ISolicitacaoEscalaExtraRepository
    {
        Task<EscalaExtra> ObterPorIdAsync(Guid id);
        Task<List<EscalaExtra>> ObterTodosAsync();
        Task<EscalaExtra> AdicionarListaAsync(EscalaExtra escalaExtra);
        Task<bool> DeletarAsync(Guid id);
        Task<EscalaExtra> BuscarPorIdAsync(Guid id);
        Task<EscalaExtra> AlterarAsync(EscalaExtra escalaExtra);
    }
}
