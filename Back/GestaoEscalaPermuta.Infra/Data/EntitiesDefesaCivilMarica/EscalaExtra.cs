using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    
    public class EscalaExtra
    {
        [Key]
        public Guid IdEscalaExtra { get; set; }
        [Required]
        public Guid IdCriacaoEscalaExtra { get; set; }        
        [Required]
        public Guid IdFuncionario { get; set; }
        public DateTime DtCriacao { get; set; } = DateTime.Now;
        public string StatusInscricao { get; set; } = string.Empty;
        public DateTime? DtConfirmacao { get; set; }
    }

}
