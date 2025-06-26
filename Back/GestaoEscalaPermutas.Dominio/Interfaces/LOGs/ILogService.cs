using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Interfaces.LOGs
{
    public interface ILogService
    {
        Task RegistrarAsync<T>(string acao, string entidade, string entidadeId, T? valorAntigo, T? valorNovo, string usuarioResponsavel);
    }
}
