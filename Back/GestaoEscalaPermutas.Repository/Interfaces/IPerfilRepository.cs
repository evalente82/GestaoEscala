using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface IPerfilRepository
    {
        Task<IEnumerable<Perfil>> BuscarTodosAsync();
        Task<Perfil?> BuscarPorIdAsync(Guid idPerfil);
        Task<Perfil> CriarAsync(Perfil perfil);
        Task<Perfil> AtualizarAsync(Perfil perfil);
        Task<bool> DeletarAsync(Guid idPerfil);
    }
}
