using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface ICriacaoEscalaExtraRepository
    {
        Task<List<CriacaoEscalaExtra>> ObterTodosComCargosAsync();
        Task AdicionarListaAsync(CriacaoEscalaExtra escalaExtra);
        void DeletarAsync(CriacaoEscalaExtra escalaExtra);
        Task<CriacaoEscalaExtra?> ObterPorIdAsync(Guid id);
        Task<CriacaoEscalaExtra?> BuscarComCargosPorIdAsync(Guid id);
    }
}
