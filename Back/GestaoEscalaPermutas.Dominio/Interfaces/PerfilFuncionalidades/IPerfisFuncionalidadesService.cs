using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;

namespace GestaoEscalaPermutas.Dominio.Interfaces.PerfisFuncionalidades
{
    public interface IPerfisFuncionalidadesService
    {
        Task<bool> AtribuirFuncionalidadeAoPerfil(Guid idPerfil, Guid idFuncionalidade);
        Task<bool> RemoverFuncionalidadeDoPerfil(Guid idPerfil, Guid idFuncionalidade);
        Task<IEnumerable<FuncionalidadeDTO>> BuscarFuncionalidadesPorPerfil(Guid idPerfil);
        Task<IEnumerable<PerfisFuncionalidadesDTO>> BuscarTodas();
    }
}
