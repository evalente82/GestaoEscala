using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;

namespace GestaoEscalaPermutas.Dominio.Interfaces.Escala
{
    public interface IEscalaService
    {
        Task<EscalaDTO> Incluir(EscalaDTO escalaModel);
        Task<EscalaDTO> Alterar(Guid id, EscalaDTO escalaModel);
        Task<EscalaDTO> Deletar(Guid id);
        Task<List<EscalaDTO>> BuscarTodos();
        Task<EscalaDTO> BuscarPorId(Guid idEscala);

    }
}
