using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.DTO.EscalaPronta
{
    public class EscalaProntaDTO: RetornoDTO
    {
        public EscalaProntaDTO()
        {
            IdEscalaPronta = Guid.NewGuid();
            DtCriacao = DateTime.UtcNow;
        }
        public Guid IdEscalaPronta { get; set; }
        public Guid IdEscala { get; set; }
        public Guid IdPostoTrabalho { get; set; }
        public Guid IdFuncionario { get; set; }
        public DateTime DtDataServico { get; set; }
        public DateTime DtCriacao { get; set; }
    }
}
