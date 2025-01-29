using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

[Table("Funcionario")]
public partial class Funcionario
{
    [Key]
    public Guid IdFuncionario { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string NmNome { get; set; } = null!;

    public int NrMatricula { get; set; }

    public long? NrTelefone { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string NmEndereco { get; set; } = null!;

    public Guid IdCargo { get; set; }

    public bool IsAtivo { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? NmEmail { get; set; }
    public ICollection<CargoPerfis> CargoPerfis { get; set; } = new List<CargoPerfis>();
}
