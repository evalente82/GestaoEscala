using GestaoEscalaPermutas.Dominio.DTO.Departamento;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios
{
    public interface IFuncionarioService
    {
        Task<FuncionarioDTO> Incluir(FuncionarioDTO funcionarioModel);
        Task<FuncionarioDTO> Alterar(int id, FuncionarioDTO funcionarioModel);
        Task<FuncionarioDTO> Deletar(int id);
        Task<List<FuncionarioDTO>> BuscarTodos();
    }
}
