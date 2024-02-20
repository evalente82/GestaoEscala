using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

public partial class Permuta
{
    [Key]
    public int IdPermuta { get; set; }

    public int IdEscala { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? NmNomeSolicitante { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? NmNomeSolicitado { get; set; }

    public int NrMatriculaSolicitante { get; set; }

    public int NrMatriculaSolicitado { get; set; }

    public DateOnly? DtSolicitacao { get; set; }

    public DateOnly? DtDataSolicitadaTroca { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? NmNomeAprovador { get; set; }

    public int NrMatriculaAprovador { get; set; }

    public DateOnly? DtAprovacao { get; set; }

}
