using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface ISetorRepository
    {
        Task<Setor> IncluirAsync(Setor setor);
        Task<Setor> AlterarAsync(Setor setor);
        Task<Setor> BuscarPorIdAsync(Guid id);
        Task<List<Setor>> BuscarTodosAsync();
        Task<bool> DeletarAsync(Guid id);
    }
}
