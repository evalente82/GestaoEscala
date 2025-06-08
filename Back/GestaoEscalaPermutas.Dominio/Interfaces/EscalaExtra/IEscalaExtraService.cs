using GestaoEscalaPermutas.Dominio.DTO.EscalaExtra;
using GestaoEscalaPermutas.Dominio.DTO.EscalaPronta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra
{
    public interface IEscalaExtraService
    {
        Task<List<EscalaExtraDTO>> BuscarTodos();
        Task<EscalaExtraDTO> BuscarPorId(Guid idEscalaExtra);
        Task<EscalaExtraDTO[]> IncluirLista(EscalaExtraDTO[] escalaExtraDTOs);
    }
}
