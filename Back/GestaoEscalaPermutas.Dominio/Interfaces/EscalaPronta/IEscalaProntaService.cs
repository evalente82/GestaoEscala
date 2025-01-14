using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.EscalaPronta;
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
        Task<EscalaProntaDTO> Alterar(int id, EscalaProntaDTO escalaProntaModel);
        Task<EscalaProntaDTO> Deletar(int id);
        Task<List<EscalaProntaDTO>> BuscarTodos();
        Task<EscalaProntaDTO> BuscarPorId(int idEscalaPronta);
        Task<EscalaProntaDTO[]> IncluirLista(EscalaProntaDTO[] escalaProntaDTOs);
    }
}
