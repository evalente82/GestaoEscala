using GestaoEscalaPermutas.Dominio.DTO.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Interfaces.Departamento
{
    public interface IDepartamentoService
    {
        Task<DepartamentoDTO> Incluir(DepartamentoDTO departamentoModel);
        Task<List<DepartamentoDTO>> BuscarTodos();
    }
}
