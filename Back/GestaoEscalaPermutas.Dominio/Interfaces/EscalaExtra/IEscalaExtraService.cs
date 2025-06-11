using GestaoEscalaPermutas.Dominio.DTO.EscalaExtra;

namespace GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra
{
    public interface IEscalaExtraService
    {
        Task<List<EscalaExtraDTO>> BuscarTodos();
        Task<EscalaExtraDTO> BuscarPorId(Guid idEscalaExtra);
        Task<EscalaExtraDTO[]> IncluirLista(EscalaExtraDTO[] escalaExtraDTOs);
    }
}
