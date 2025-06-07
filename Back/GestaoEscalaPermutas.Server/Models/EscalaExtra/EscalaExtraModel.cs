namespace GestaoEscalaPermutas.Server.Models.EscalaExtra
{
    public class EscalaExtraModel : RetornoModel
    {
        public Guid IdEscalaExtra { get; set; }
        public Guid IdCriacaoEscalaExtra { get; set; }
        public Guid IdPostoTrabalho { get; set; }
        public Guid IdFuncionario { get; set; }
        public DateTime DtServico { get; set; }
        public DateTime DtCriacao { get; set; } = DateTime.Now;
    }
}
