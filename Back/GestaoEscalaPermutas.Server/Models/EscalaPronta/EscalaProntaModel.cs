using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Server.Models.EscalaPronta
{
    public class EscalaProntaModel: RetornoModel
    {
        public int IdEscalaPronta { get; set; }
        public int IdEscala { get; set; }
        public int IdPostoTrabalho { get; set; }
        public int IdFuncionario { get; set; }
        public DateTime DtDataServico { get; set; }
        public DateTime DtCriacao { get; set; }
    }
}
