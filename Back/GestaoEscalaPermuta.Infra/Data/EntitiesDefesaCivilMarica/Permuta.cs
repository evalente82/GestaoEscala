using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

public partial class Permuta
{
    [Key]
    public Guid IdPermuta { get; set; }
    public Guid IdEscala { get; set; }
    public Guid IdFuncionarioSolicitante { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string NmNomeSolicitante { get; set; }
    public Guid IdFuncionarioSolicitado { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string NmNomeSolicitado { get; set; }
    public Guid? IdFuncionarioAprovador { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? NmNomeAprovador { get; set; }
    public DateTime DtSolicitacao { get; set; }
    public DateTime DtDataSolicitadaTroca { get; set; }
    public DateTime? DtAprovacao { get; set; }

}
