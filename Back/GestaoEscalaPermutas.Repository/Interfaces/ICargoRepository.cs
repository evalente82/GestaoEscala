using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface ICargoRepository
    {
        Task<Cargo> AdicionarAsync(Cargo cargo);
        Task<Cargo?> ObterPorIdAsync(Guid id);
        Task<List<Cargo>> ObterTodosAsync();
        Task AtualizarAsync(Cargo cargo);
        Task RemoverAsync(Guid id);
    }
}
