using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using System.Security.Cryptography;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface IEscalaProntaRepository
    {
        Task<EscalaPronta> AdicionarAsync(EscalaPronta escalaPronta);
        Task<EscalaPronta[]> AdicionarListaAsync(EscalaPronta[] escalasProntas);
        Task<EscalaPronta?> ObterPorIdAsync(Guid id);
        Task<List<EscalaPronta>> ObterPorEscalaIdAsync(Guid idEscalaPronta);
        Task<List<EscalaPronta>> ObterTodosAsync();
        Task AtualizarAsync(EscalaPronta escalaPronta);
        Task AtualizarEscalaProntaAsync(EscalaPronta escalaPronta);
        Task RemoverAsync(Guid id);
        Task RemoverListaPorEscalaAsync(Guid idEscala);
        Task<List<EscalaPronta>> BuscarPorIdFuncionario(Guid idFuncionario);
        Task<EscalaPronta> ObterPorDataEPostoAsync(DateTime dia, Guid idPostoTrabalho);
        Task AdicionarEmLoteAsync(IEnumerable<EscalaPronta> escalasProntas);
    }
}
