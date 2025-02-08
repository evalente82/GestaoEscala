using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface IDepartamentoRepository
    {
        Task<Departamento> AdicionarAsync(Departamento departamento);
        Task<Departamento?> ObterPorIdAsync(Guid id);
        Task<List<Departamento>> ObterTodosAsync();
        Task AtualizarAsync(Departamento departamento);
        Task RemoverAsync(Guid id);
    }
}
