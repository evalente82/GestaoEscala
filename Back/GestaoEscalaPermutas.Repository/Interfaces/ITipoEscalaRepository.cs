using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface ITipoEscalaRepository
    {
        Task<TipoEscala> IncluirAsync(TipoEscala tipoEscala);
        Task<TipoEscala> AlterarAsync(TipoEscala tipoEscala);
        Task<TipoEscala> BuscarPorIdAsync(Guid id);
        Task<List<TipoEscala>> BuscarTodosAsync();
        Task<bool> DeletarAsync(Guid id);
    }
}
