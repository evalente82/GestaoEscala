using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade
{
    public class PerfilDTO : RetornoDTO
    {
        public Guid IdPerfil { get; set; }
        public string Nome { get; set; } = null!;
        public string? Descricao { get; set; } // Descrição do perfil
    }
}
