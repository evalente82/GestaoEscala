using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermuta.Infra.Data.EntitiesDefesaCivilMarica;

[Table("Escala")]
public partial class Escala
{
    [Key]
    public int IdEscala { get; set; }

    public int IdDepartamento { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? NmNomeEscala { get; set; }

    public int IdTipoEscala { get; set; }

    public DateOnly? DtCriacao { get; set; }

    public int NrMesReferencia { get; set; }

    public bool IsAtivo { get; set; }

    public int NrPessoaPorPosto { get; set; }

    
}
