using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Interfaces.PerfilFuncionalidades
{
    public interface IFuncionalidadeService
    {
        Task<FuncionalidadeDTO> Criar(FuncionalidadeDTO funcionalidadeDTO);
        Task<FuncionalidadeDTO> Atualizar(FuncionalidadeDTO funcionalidadeDTO);
        Task<bool> Deletar(Guid id);
        Task<IEnumerable<FuncionalidadeDTO>> BuscarTodas();
        Task<FuncionalidadeDTO?> BuscarPorId(Guid id);
    }
}
