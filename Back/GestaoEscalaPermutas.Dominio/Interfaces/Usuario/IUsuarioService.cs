using GestaoEscalaPermutas.Dominio.DTO.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Interfaces.Usuario
{
    public interface IUsuarioService
    {
        Task<UsuarioDTO> Criar(UsuarioDTO usuario);
        Task<UsuarioDTO> BuscarPorId(Guid id);
        Task<IEnumerable<UsuarioDTO>> BuscarTodos();
        Task<UsuarioDTO> Atualizar(UsuarioDTO usuario);
        Task<bool> Deletar(Guid id);
    }
}
