using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface IPostoTrabalhoRepository
    {
        Task<PostoTrabalho> IncluirAsync(PostoTrabalho postoTrabalho);
        Task<PostoTrabalho> AlterarAsync(PostoTrabalho postoTrabalho);
        Task<PostoTrabalho> BuscarPorIdAsync(Guid id);
        Task<List<PostoTrabalho>> BuscarTodosAsync();
        Task<bool> DeletarAsync(Guid id);
        Task<List<PostoTrabalho>> BuscarTodosAtivosAsync();
        Task<PostoTrabalho[]> IncluirListaAsync(PostoTrabalho[] postos);
    }
}
