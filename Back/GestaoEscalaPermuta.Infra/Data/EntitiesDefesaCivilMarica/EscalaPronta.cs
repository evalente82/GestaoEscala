using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

public partial class EscalaPronta
{
    [Key]
    public int IdEscalaPronta { get; set; }

    public int IdEscala { get; set; }

    public int IdPostoTrabalho { get; set; }

    public int IdFuncionario { get; set; }

    public DateTime DtDataServico { get; set; }

    public DateTime DtCriacao { get; set; }


}
