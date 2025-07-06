using GestaoEscalaPermutas.Dominio.DTO.Funcionario;

namespace GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios
{
    public interface IFuncionarioService
    {
        Task<FuncionarioDTO> Incluir(FuncionarioDTO funcionarioDTO);
        Task<FuncionarioDTO> Alterar(Guid id, FuncionarioDTO funcionarioDTO);
        Task<FuncionarioDTO> Deletar(Guid id);
        Task<List<FuncionarioDTO>> BuscarTodos();
        Task<FuncionarioDTO[]> IncluirLista(FuncionarioDTO[] funcionarioDTOs);
        Task<List<FuncionarioDTO>> BuscarTodosAtivos();
        Task<string> GetFcmTokenAsync(Guid idFuncionario);
        Task SaveFcmTokenAsync(Guid idFuncionario, string fcmToken);
        Task<List<FuncionarioDTO>> GetAdministradoresAsync();
        Task<Guid> BuscarPorNomeFuncionario(string nome);
    }
}
