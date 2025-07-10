using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Interfaces.Feriados
{
    public interface IFeriadoService
    {
        Task<HashSet<DateTime>> ObterDatasFeriadosAsync(int ano);
        // Futuramente, você pode adicionar métodos para criar feriados municipais/estaduais
    }
}