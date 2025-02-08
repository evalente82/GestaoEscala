using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface IPerfisFuncionalidadesRepository
    {
        Task<IEnumerable<PerfisFuncionalidades>> BuscarTodasAsync();
        Task<bool> AtribuirFuncionalidadeAoPerfilAsync(Guid idPerfil, Guid idFuncionalidade);
        Task<bool> RemoverFuncionalidadeDoPerfilAsync(Guid idPerfil, Guid idFuncionalidade);
        Task<IEnumerable<Funcionalidade>> BuscarFuncionalidadesPorPerfilAsync(Guid idPerfil);
    }
}
