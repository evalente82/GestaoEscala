

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Dominio.DTO.Escala
{
    public class EscalaDTO : RetornoDTO
    {
        public int IdEscala { get; set; }

        public int IdDepartamento { get; set; }

        public string? NmNomeEscala { get; set; }

        public int IdTipoEscala { get; set; }

        public DateTime DtCriacao { get; set; }

        public int NrMesReferencia { get; set; }

        public bool IsAtivo { get; set; }

        public int NrPessoaPorPosto { get; set; }
    }
}
