using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade
{
    public class CargoPerfilDTO : RetornoDTO
    {
        public Guid IdCargo { get; set; }
        public string? NomeCargo { get; set; } = null!;

        public Guid IdPerfil { get; set; }
        public string? NomePerfil { get; set; } = null!;
    }
}
