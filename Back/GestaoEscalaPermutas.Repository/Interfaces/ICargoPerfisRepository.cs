using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface ICargoPerfisRepository
    {
        Task<IEnumerable<CargoPerfis>> BuscarTodosAsync();
        Task<bool> ExisteRelacionamentoAsync(Guid idCargo, Guid idPerfil);
        Task<bool> AtribuirPerfilAoCargoAsync(Guid idCargo, Guid idPerfil);
        Task<bool> RemoverPerfilDoCargoAsync(Guid idCargo, Guid idPerfil);
        Task<IEnumerable<Perfil>> BuscarPerfisPorCargoAsync(Guid idCargo);
    }
}
