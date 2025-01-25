namespace GestaoEscalaPermutas.Server.Models.Permuta
{
    public class PermutaModel : RetornoModel
    {
        public Guid IdPermuta { get; set; }
        public Guid IdEscala { get; set; }
        public Guid IdFuncionarioSolicitante { get; set; }
        public required string NmNomeSolicitante { get; set; }
        public Guid IdFuncionarioSolicitado { get; set; }
        public required string NmNomeSolicitado { get; set; }
        public Guid IdFuncionarioAprovador { get; set; }
        public required string NmNomeAprovador { get; set; }
        public DateTime DtSolicitacao { get; set; }
        public DateTime DtDataSolicitadaTroca { get; set; }
        public DateTime DtAprovacao { get; set; }
    }
}
