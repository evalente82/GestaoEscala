

using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestaoEscalaPermutas.Dominio.DTO.Escala
{
    public class EscalaDTO : RetornoDTO
    {
        public EscalaDTO()
        {
            IdEscala = Guid.NewGuid();
            DtCriacao = DateTime.UtcNow;
        }
        public Guid IdEscala { get; set; }

        public Guid IdDepartamento { get; set; }

        public string? NmNomeEscala { get; set; }

        public Guid IdTipoEscala { get; set; }
        [JsonIgnore]
        public DateTime DtCriacao { get; set; }

        public int NrMesReferencia { get; set; }

        public bool IsAtivo { get; set; }

        
        public bool IsGerada{ get; set; }

        public int NrPessoaPorPosto { get; set; }
    }
}
