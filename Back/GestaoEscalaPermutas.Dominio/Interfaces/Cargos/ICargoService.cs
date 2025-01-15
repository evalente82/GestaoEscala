using GestaoEscalaPermutas.Dominio.DTO.Cargo;
using GestaoEscalaPermutas.Dominio.DTO.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Interfaces.Cargos
{
    public interface ICargoService
    {
        Task<CargoDTO> Incluir(CargoDTO departamentoModel);
        Task<CargoDTO> Alterar(Guid id, CargoDTO departamentoModel);
        Task<CargoDTO> Deletar(Guid id);
        Task<List<CargoDTO>> BuscarTodos();
    }
}
