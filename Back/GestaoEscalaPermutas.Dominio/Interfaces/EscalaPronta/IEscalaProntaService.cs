using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.EscalaPronta;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Interfaces.EscalaPronta
{
    public interface IEscalaProntaService
    {
        Task<EscalaProntaDTO> Incluir(EscalaProntaDTO escalaProntaModel);
        Task<EscalaProntaDTO> Alterar(Guid id, EscalaProntaDTO escalaProntaModel);
        Task<EscalaProntaDTO> Deletar(Guid id);
        Task<List<EscalaProntaDTO>> BuscarTodos();
        Task<List<EscalaProntaDTO>> BuscarPorId(Guid idEscalaPronta);
        Task<EscalaProntaDTO[]> IncluirLista(EscalaProntaDTO[] escalaProntaDTOs);
        Task<EscalaProntaDTO[]> AlterarEscalaPronta(Guid IdEscala, EscalaProntaDTO[] escalaProntaDTOs);
    }
}
