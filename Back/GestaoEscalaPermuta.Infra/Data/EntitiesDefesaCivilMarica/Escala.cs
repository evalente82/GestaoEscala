using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

[Table("Escala")]
public partial class Escala
{
    [Key]
    public Guid IdEscala { get; set; }

    public Guid IdDepartamento { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? NmNomeEscala { get; set; }

    public Guid IdTipoEscala { get; set; }

    public DateTime DtCriacao { get; set; }

    public int NrMesReferencia { get; set; }

    public bool IsAtivo { get; set; }
    public bool IsGerada { get; set; }

    public int NrPessoaPorPosto { get; set; }

    
}
