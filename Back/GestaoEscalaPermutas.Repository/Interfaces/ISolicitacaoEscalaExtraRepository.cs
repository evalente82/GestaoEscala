using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface ISolicitacaoEscalaExtraRepository
    {
        Task<List<EscalaExtra>> ObterListaPorIdFuncionario(Guid id);
        Task<List<EscalaExtra>> ObterTodosAsync();
        Task AdicionarListaAsync(EscalaExtra escalaExtra);
        Task<bool> DeletarAsync(Guid id);
        Task<EscalaExtra?> BuscarPorIdAsync(Guid id);
        Task<EscalaExtra> AlterarAsync(EscalaExtra escalaExtra);
        Task<EscalaExtra?> BuscarPorIdEscalaExtra(Guid id);
        Task<EscalaExtra?> ObterProximoDaFilaAsync(Guid idCriacaoEscalaExtra);
        Task<List<EscalaExtra>> ObterInscricoesPorFuncionarioEData(Guid idFuncionario, DateTime data);
    }
}
