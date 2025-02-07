using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface IFuncionalidadeRepository
    {
        Task<Funcionalidade> CriarAsync(Funcionalidade funcionalidade);
        Task<Funcionalidade> AtualizarAsync(Funcionalidade funcionalidade);
        Task<bool> DeletarAsync(Guid id);
        Task<IEnumerable<Funcionalidade>> BuscarTodasAsync();
        Task<Funcionalidade?> BuscarPorIdAsync(Guid id);
    }
}
