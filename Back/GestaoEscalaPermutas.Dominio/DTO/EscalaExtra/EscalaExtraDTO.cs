using System.Text.Json.Serialization;

namespace GestaoEscalaPermutas.Dominio.DTO.EscalaExtra
{
    public class EscalaExtraDTO : RetornoDTO
    {
        public EscalaExtraDTO()
        {
            IdCriacaoEscalaExtra = Guid.NewGuid();
            DtCriacao = DateTime.UtcNow;
        }
        public Guid IdCriacaoEscalaExtra { get; set; }
        public string NmEscalaExtra { get; set; } = null!;
        public Guid IdSetor { get; set; }
        public DateTime DtEscalaExtra { get; set; }
        public DateTime DtAbertura { get; set; }
        public DateTime DtFechamento { get; set; }
        public string horaDoServico { get; set; }        
        public string HoraAbertura { get; set; }
        public string HoraFechamento{ get; set; }
        public Guid IdFuncionario { get; set; }
        public string NomeFuncionario { get; set; }
        public bool IsAtivo { get; set; } = true;
        [JsonIgnore]
        public DateTime DtCriacao { get; set; } = DateTime.Now;
        public int QtdVagas { get; set; }
        public List<Guid> IdCargo { get; set; }
    }
}
