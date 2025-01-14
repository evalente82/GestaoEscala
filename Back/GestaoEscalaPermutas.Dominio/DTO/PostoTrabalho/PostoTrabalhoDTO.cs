namespace GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho
{
    public class PostoTrabalhoDTO: RetornoDTO
    {
        public int IdPostoTrabalho { get; set; }

        public string NmNome { get; set; } = null!;

        public string? NmEnderco { get; set; }

        public bool IsAtivo { get; set; }

        public DateTime DtCriacao { get; set; }

        public int IdDepartamento { get; set; }
    }
}
