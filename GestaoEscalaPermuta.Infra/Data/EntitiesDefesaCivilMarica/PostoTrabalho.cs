using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

[Table("PostoTrabalho")]
public partial class PostoTrabalho
{
    [Key]
    public int IdPostoTrabalho { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string NmNome { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string? NmEnderco { get; set; }

    public bool IsAtivo { get; set; }

    public DateTime DtCriacao { get; set; }

    public int IdDepartamento { get; set; }

}
