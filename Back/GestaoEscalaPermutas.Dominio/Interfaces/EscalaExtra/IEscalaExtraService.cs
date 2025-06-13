using GestaoEscalaPermutas.Dominio.DTO.EscalaExtra;
using GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho;

namespace GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra
{
    public interface IEscalaExtraService
    {
        Task<List<EscalaExtraDTO>> BuscarTodos();
        Task<EscalaExtraDTO> BuscarPorId(Guid idEscalaExtra);
        Task<EscalaExtraDTO> IncluirLista(EscalaExtraDTO escalaExtraDTOs);
        Task<EscalaExtraDTO> Deletar(Guid id);
        Task<EscalaExtraDTO> Alterar(Guid id, EscalaExtraDTO escalaExtraModel);
    }
}
