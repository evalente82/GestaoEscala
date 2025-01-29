using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.Permutas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Interfaces.Permutas
{
    public interface IPermutasService
    {
        Task<PermutasDTO> Incluir(PermutasDTO permutasModel);
        Task<PermutasDTO> Alterar(Guid id, PermutasDTO permutasModel);
        Task<PermutasDTO> Deletar(Guid id);
        Task<List<PermutasDTO>> BuscarTodos();
        Task<PermutasDTO> BuscarPorId(Guid idPermuta);
    }
}
