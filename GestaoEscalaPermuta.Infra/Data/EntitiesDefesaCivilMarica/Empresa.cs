using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

[Table("EMPRESA")]
public partial class Empresa
{
    [Key]
    public int IdEmpresa { get; set; }

    [StringLength(200)]
    public string NmNome { get; set; } = null!;

    [StringLength(200)]
    public string NmEndereco { get; set; } = null!;

    [Column("NrCNPJ")]
    public int NrCnpj { get; set; }

    public int NrTelefone { get; set; }

    [StringLength(100)]
    public string NmEmail { get; set; } = null!;
}
