using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface IFuncionarioRepository
    {
        Task<Funcionario> AdicionarAsync(Funcionario funcionario);
        Task<Funcionario> AlterarAsync(Funcionario funcionario);
        Task<Funcionario> ObterPorIdAsync(Guid id);
        Task<List<Funcionario>> ObterTodosAsync();
        Task<List<Funcionario>> ObterTodosAtivosAsync();
        Task RemoverAsync(Guid id);
        Task<Funcionario[]> AdicionarListaAsync(Funcionario[] funcionarios);
        Task<bool> MatriculaExisteAsync(int nrMatricula);
        Task<bool> EmailExisteAsync(string email);

        // Métodos para FCM Tokens
        Task<string> GetFcmTokenAsync(Guid idFuncionario);
        Task SaveFcmTokenAsync(Guid idFuncionario, string fcmToken);
        Task<List<Funcionario>> ObterAdministradoresAsync();
    }
}
