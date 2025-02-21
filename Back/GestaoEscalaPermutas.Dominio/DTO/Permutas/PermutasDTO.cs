using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.DTO.Permutas
{
    public class PermutasDTO : RetornoDTO
    {
        public PermutasDTO() 
        {
            IdPermuta = Guid.NewGuid();
            DtSolicitacao = DateTime.UtcNow;
        }

        public Guid IdPermuta { get; set; }
        public Guid IdEscala { get; set; }
        public Guid IdFuncionarioSolicitante { get; set; }
        public string NmNomeSolicitante { get; set; }
        public Guid IdFuncionarioSolicitado { get; set; }
        public string NmNomeSolicitado { get; set; }
        public Guid? IdFuncionarioAprovador { get; set; }
        public string? NmNomeAprovador { get; set; }
        public DateTime DtSolicitacao { get; set; }
        public DateTime DtDataSolicitadaTroca { get; set; }
        public DateTime? DtAprovacao { get; set; }
        public DateTime? DtReprovacao { get; set; }
        public string NmStatus { get; set; }
    }
}
