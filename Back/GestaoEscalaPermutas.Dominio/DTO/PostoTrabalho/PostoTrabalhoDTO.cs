
namespace GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho
{
    public class PostoTrabalhoDTO : RetornoDTO
    {
        public PostoTrabalhoDTO()
        {
            IdPostoTrabalho = Guid.NewGuid();
            DtCriacao = DateTime.UtcNow;
        }
        public Guid IdPostoTrabalho { get; set; }

        public string NmNome { get; set; } = null!;

        public string? NmEnderco { get; set; }

        public bool IsAtivo { get; set; }

        public DateTime DtCriacao { get; set; }

        public Guid IdDepartamento { get; set; }
        public Guid? IdSetor { get; set; } // Novo campo
        public string? NomeSetor { get; set; } // Adicionado para exibição do nome do setor

    }
}
