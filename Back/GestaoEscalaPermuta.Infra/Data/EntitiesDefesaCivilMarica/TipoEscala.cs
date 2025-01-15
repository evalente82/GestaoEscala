using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

[Table("TipoEscala")]
public partial class TipoEscala
{
    [Key]
    public Guid IdTipoEscala { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string NmNome { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string? NmDescricao { get; set; }

    public bool IsAtivo { get; set; }

    public DateTime DtCriacao { get; set; }

    public int NrHorasTrabalhada { get; set; }

    public int NrHorasFolga { get; set; }

    public bool IsExpediente { get; set; }
}
