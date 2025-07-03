using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuarios> CriarAsync(Usuarios usuario);
        Task<Usuarios> BuscarPorIdAsync(Guid id);
        Task<IEnumerable<Usuarios>> BuscarTodosAsync();
        Task<Usuarios> AtualizarAsync(Usuarios usuario);
        Task<bool> DeletarAsync(Guid id);
        Task<Usuarios?> VerificarUsuarioPorFuncionarioAsync(Guid idFuncionario);
    }
}
