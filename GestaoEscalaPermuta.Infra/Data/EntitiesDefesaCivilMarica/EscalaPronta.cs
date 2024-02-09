using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermuta.Infra.Data.EntitiesDefesaCivilMarica;

public partial class EscalaPronta
{
    [Key]
    public int IdEscalaPronta { get; set; }

    public int IdEscala { get; set; }

    public int IdPostoTrabalho { get; set; }

    public int IdFuncionario { get; set; }

    public bool IsAtivo { get; set; }

    public DateOnly DtDataServico { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? NmEmail { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? NmSenha { get; set; }


}
