using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Server.Models.EscalaPronta
{
    public class EscalaProntaModel: RetornoModel
    {
        public Guid IdEscalaPronta { get; set; }
        public Guid IdEscala { get; set; }
        public Guid IdPostoTrabalho { get; set; }
        public Guid IdFuncionario { get; set; }
        public DateTime DtDataServico { get; set; }
        public DateTime DtCriacao { get; set; }
    }
}
