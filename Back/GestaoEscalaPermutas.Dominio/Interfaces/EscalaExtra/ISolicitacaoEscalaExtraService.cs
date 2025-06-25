using GestaoEscalaPermutas.Dominio.DTO.EscalaExtra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra
{
    public interface ISolicitacaoEscalaExtraService
    {
        Task<List<SolicitacaoEscalaExtraDTO>> BuscarTodos();
        Task<List<SolicitacaoEscalaExtraDTO>> BuscarPorIdFuncionario(Guid idFuncionario);
        Task<SolicitacaoEscalaExtraDTO> Incluir(SolicitacaoEscalaExtraDTO escalaExtraDTOs);
        Task<SolicitacaoEscalaExtraDTO> Deletar(Guid id);
        Task<SolicitacaoEscalaExtraDTO> Alterar(Guid id, SolicitacaoEscalaExtraDTO escalaExtraModel);
        Task<List<VisualizarSolicitacoesDTO>> ListarTodos();
    }
}
