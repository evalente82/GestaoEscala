using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.DTO.EscalaExtra
{
    public class VisualizarSolicitacoesDTO : RetornoDTO
    {        
        public Guid IdEscalaExtra { get; set; }
        public Guid IdCriacaoEscalaExtra { get; set; }
        public Guid IdFuncionario { get; set; }
        public string NmFuncionario { get; set; }
        public string NmEscalaExtra { get; set; }
        public string NmSetor { get; set; }
        public DateTime DtEscalaExtra { get; set; }
    }
}
