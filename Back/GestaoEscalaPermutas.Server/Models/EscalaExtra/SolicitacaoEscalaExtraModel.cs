namespace GestaoEscalaPermutas.Server.Models.EscalaExtra
{
    public class SolicitacaoEscalaExtraModel : RetornoModel
    {
        public Guid IdEscalaExtra { get; set; }
        public Guid IdCriacaoEscalaExtra { get; set; }
        public Guid IdFuncionario { get; set; }
        public DateTime DtCriacao { get; set; } = DateTime.Now;
        public string NmEscalaExtra { get; set; }
        public string NmSetor { get; set; }
        public DateTime DtEscalaExtra { get; set; }
        public string StatusInscricao { get; set; } = string.Empty;
    }
}
