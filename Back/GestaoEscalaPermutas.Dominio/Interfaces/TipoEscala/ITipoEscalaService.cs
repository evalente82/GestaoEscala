using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.DTO.TipoEscala;

namespace GestaoEscalaPermutas.Dominio.Interfaces.TipoEscala
{
    public interface ITipoEscalaService
    {
        Task<TipoEscalaDTO> Incluir(TipoEscalaDTO tipoEscalaDTOModel);
        Task<TipoEscalaDTO> Alterar(Guid id, TipoEscalaDTO tipoEscalaDTOModel);
        Task<TipoEscalaDTO> Deletar(Guid id);
        Task<List<TipoEscalaDTO>> BuscarTodos();
        Task<TipoEscalaDTO> BuscarPorId(Guid idEscala);
    }
}
