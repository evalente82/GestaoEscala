using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.DTO.EscalaExtra
{
    public class SolicitacaoEscalaExtraDTO : RetornoDTO
    {
        public SolicitacaoEscalaExtraDTO()
        {
            IdEscalaExtra = Guid.NewGuid();
            DtCriacao = DateTime.UtcNow;
        }
        [JsonIgnore]
        public Guid IdEscalaExtra { get; set; }
        public Guid IdCriacaoEscalaExtra { get; set; }
        public Guid IdFuncionario { get; set; }
        public DateTime DtServico { get; set; }
        [JsonIgnore]
        public DateTime DtCriacao { get; set; } = DateTime.Now;
    }
}
