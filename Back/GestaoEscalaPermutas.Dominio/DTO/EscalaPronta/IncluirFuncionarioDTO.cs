using System.ComponentModel.DataAnnotations;

public class IncluirFuncionarioDTO
{
    [Required]
    public Guid IdEscala { get; set; }

    [Required]
    public Guid IdPostoTrabalho { get; set; }

    [Required]
    public Guid IdFuncionario { get; set; }

    [Required]
    public DateTime DtDataServico { get; set; }
}
