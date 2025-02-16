using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

public partial class EscalaPronta
{
    [Key]
    public Guid IdEscalaPronta { get; set; }

    public Guid IdEscala { get; set; }

    public Guid IdPostoTrabalho { get; set; }

    public Guid IdFuncionario { get; set; }

    public DateTime DtDataServico { get; set; }

    public DateTime DtCriacao { get; set; }

    public virtual Escala Escala { get; set; }
}
