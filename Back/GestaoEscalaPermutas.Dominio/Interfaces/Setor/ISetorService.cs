
using GestaoEscalaPermutas.Dominio.DTO.Setor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Interfaces.Setor
{
    public interface ISetorService
    {
        Task<SetorDTO> Incluir(SetorDTO setorDTO);
        Task<SetorDTO> Alterar(Guid id, SetorDTO setorDTO);
        Task<SetorDTO> Deletar(Guid id);
        Task<SetorDTO> BuscarPorId(Guid id);
        Task<List<SetorDTO>> BuscarTodos();
    }
}
