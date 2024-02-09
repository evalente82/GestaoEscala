using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermuta.Infra.Data.EntitiesDefesaCivilMarica;

[Table("Funcionario")]
public partial class Funcionario
{
    [Key]
    public int IdFuncionario { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string NmNome { get; set; } = null!;

    public int NrMatricula { get; set; }

    public int? NrTelefone { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string NmEndereco { get; set; } = null!;

    public int IdCargos { get; set; }

    public bool IsAtivo { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? NmEmail { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? NmSenha { get; set; }

}
