using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;

namespace GestaoEscalaPermutas.Dominio.Interfaces.Escala
{
    public interface IEscalaService
    {
        Task<EscalaDTO> Incluir(EscalaDTO escalaModel);
        Task<EscalaDTO> Alterar(int id, EscalaDTO escalaModel);
        Task<EscalaDTO> Deletar(int id);
        Task<List<EscalaDTO>> BuscarTodos();
        Task<EscalaDTO> BuscarPorId(int idEscala);

    }
}
