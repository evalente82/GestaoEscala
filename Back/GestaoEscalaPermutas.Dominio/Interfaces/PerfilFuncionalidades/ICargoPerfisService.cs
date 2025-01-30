using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Interfaces.PerfilFuncionalidades
{
    public interface ICargoPerfisService
    {
        Task<bool> AtribuirPerfilAoCargo(Guid idCargo, Guid idPerfil);
        Task<bool> RemoverPerfilDoCargo(Guid idCargo, Guid idPerfil);
        Task<IEnumerable<CargoPerfilDTO>> BuscarTodos();
        Task<IEnumerable<CargoPerfilDTO>> BuscarPerfisPorCargo(Guid idCargo);
    }
}
