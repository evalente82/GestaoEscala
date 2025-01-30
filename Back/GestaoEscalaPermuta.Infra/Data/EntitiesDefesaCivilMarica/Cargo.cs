using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

public partial class Cargo
{
    [Key]
    public Guid IdCargo { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string NmNome { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string? NmDescricao { get; set; }
    public bool IsAtivo { get; set; }
    public DateTime DtCriacao { get; set; }


    // Relacionamento com Perfis
    public ICollection<CargoPerfis> CargoPerfis { get; set; } = new List<CargoPerfis>();

}
