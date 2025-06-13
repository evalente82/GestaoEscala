namespace GestaoEscalaPermutas.Server.Models.EscalaPronta
{
    public class CriacaoEscalaExtraModel : RetornoModel
    {
        public Guid IdCriacaoEscalaExtra { get; set; }
        public DateTime DtEscalaExtra { get; set; }
        public DateTime DtAbertura { get; set; }
        public DateTime DtFechamento { get; set; }
        public DateTime DtCriacao { get; set; } = DateTime.Now;
        public Guid IdFuncionario { get; set; }
        public string NmEscalaExtra { get; set; } = null!;
        public string NomeFuncionario { get; set; } = string.Empty;
        public Guid IdSetor { get; set; }
        public bool IsAtivo { get; set; } = true;
    }

}
