namespace GestaoEscalaPermutas.Server.Models.Escala
{
    public class EscalaModel : RetornoModel
    {
        public int IdEscala { get; set; }

        public int IdDepartamento { get; set; }

        public string? NmNomeEscala { get; set; }

        public int IdTipoEscala { get; set; }

        public DateTime DtCriacao { get; set; }

        public int NrMesReferencia { get; set; }

        public bool IsAtivo { get; set; }

        public bool IsGerada{ get; set; }

        public int NrPessoaPorPosto { get; set; }
    }
}
