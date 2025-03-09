using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.DTO.Permutas
{
    public class PermutaMensagemDTO
    {
        public Guid IdPermuta { get; set; }
        public Guid IdFuncionarioSolicitante { get; set; }
        public string NmNomeSolicitante { get; set; }
        public Guid IdFuncionarioSolicitado { get; set; }
        public string NmNomeSolicitado { get; set; }
        public DateTime DtDataSolicitadaTroca { get; set; }
        public string? NmStatus { get; set; } // "Solicitada", "AprovadaSolicitado", "Aprovada", "Recusada"
    }
}
