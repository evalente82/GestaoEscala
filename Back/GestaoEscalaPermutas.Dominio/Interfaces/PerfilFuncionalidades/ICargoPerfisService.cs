using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;

public interface ICargoPerfisService
{
    Task<bool> AtribuirPerfilAoFuncionario(Guid idFuncionario, Guid idPerfil);
    Task<bool> RemoverPerfilDoFuncionario(Guid idFuncionario, Guid idPerfil);
    Task<IEnumerable<CargoPerfisDTO>> BuscarPerfisPorFuncionario(Guid idFuncionario);
    Task<IEnumerable<CargoPerfisDTO>> BuscarTodos();
}
