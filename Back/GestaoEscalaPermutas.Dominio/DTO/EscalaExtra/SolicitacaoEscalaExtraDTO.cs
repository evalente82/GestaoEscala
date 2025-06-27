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
            NmEscalaExtra = "Null";
            NmSetor = "Null";
            DtEscalaExtra = new DateTime(1999, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }
        [JsonIgnore]
        public Guid IdEscalaExtra { get; set; }
        public Guid IdCriacaoEscalaExtra { get; set; }
        public Guid IdFuncionario { get; set; }
        
        public DateTime DtCriacao { get; set; } = DateTime.Now;
        [JsonIgnore]
        public string NmEscalaExtra { get; set; }
        [JsonIgnore]
        public string NmSetor { get; set; }
        [JsonIgnore]
        public DateTime DtEscalaExtra{ get; set; }
        public string StatusInscricao { get; set; } = string.Empty;

        // --- Nova Propriedade para o reCAPTCHA Token ---
        public string RecaptchaToken { get; set; }

    }
}
