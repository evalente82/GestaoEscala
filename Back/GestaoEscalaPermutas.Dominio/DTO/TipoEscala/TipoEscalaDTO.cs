using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Dominio.DTO.TipoEscala
{
    public class TipoEscalaDTO:RetornoDTO
    {
        public Guid IdTipoEscala { get; set; }

        public string NmNome { get; set; } = null!;

        public string? NmDescricao { get; set; }

        public bool IsAtivo { get; set; }

        public DateTime DtCriacao { get; set; }

        public int NrHorasTrabalhada { get; set; }

        public int NrHorasFolga { get; set; }

        public bool IsExpediente { get; set; }
    }
}
