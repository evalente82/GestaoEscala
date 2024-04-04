using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.DTO.TipoEscala;

namespace GestaoEscalaPermutas.Dominio.Interfaces.TipoEscala
{
    public interface ITipoEscalaService
    {
        Task<TipoEscalaDTO> Incluir(TipoEscalaDTO tipoEscalaDTOModel);
        Task<TipoEscalaDTO> Alterar(int id, TipoEscalaDTO tipoEscalaDTOModel);
        Task<TipoEscalaDTO> Deletar(int id);
        Task<List<TipoEscalaDTO>> BuscarTodos();
        Task<TipoEscalaDTO> BuscarPorId(int idEscala);
    }
}
