using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface IEscalaRepository
    {
        Task<Escala> AdicionarAsync(Escala escala);
        Task<Escala?> ObterPorIdAsync(Guid id);
        Task<List<Escala>> ObterTodasAsync();
        Task AtualizarAsync(Escala escala);
        Task RemoverAsync(Guid id);
        Task<List<EscalaPronta>> ObterEscalasProntasPorEscalaId(Guid idEscala);
        Task RemoverEscalasProntasPorEscalaId(Guid idEscala);
        Task<IDbContextTransaction> IniciarTransacaoAsync();
    }
}
