namespace GestaoEscalaPermutas.Server.Models.PostoTrabalho
{
    public class PostoTrabalhoModel : RetornoModel
    {
        public Guid IdPostoTrabalho { get; set; }

        public string NmNome { get; set; } = null!;

        public string? NmEnderco { get; set; }

        public bool IsAtivo { get; set; }

        public DateTime DtCriacao { get; set; }

        public Guid IdDepartamento { get; set; }
    }
}
