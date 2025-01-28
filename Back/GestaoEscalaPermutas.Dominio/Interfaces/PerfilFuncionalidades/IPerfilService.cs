using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Interfaces.PerfilFuncionalidades
{
    public interface IPerfilService
    {
        Task<IEnumerable<PerfilDTO>> BuscarTodos();
        Task<PerfilDTO> BuscarPorId(Guid idPerfil);
        Task<PerfilDTO> Criar(PerfilDTO perfilDTO);
        Task<PerfilDTO> Atualizar(PerfilDTO perfilDTO);
        Task<bool> Deletar(Guid idPerfil);
    }
}
