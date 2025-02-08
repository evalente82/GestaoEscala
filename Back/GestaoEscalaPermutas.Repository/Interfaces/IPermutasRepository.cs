using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface IPermutasRepository
    {
        Task<Permuta> IncluirAsync(Permuta permuta);
        Task<Permuta> AlterarAsync(Permuta permuta);
        Task<Permuta> BuscarPorIdAsync(Guid idPermuta);
        Task<List<Permuta>> BuscarTodosAsync();
        Task<bool> DeletarAsync(Guid idPermuta);
    }
}
