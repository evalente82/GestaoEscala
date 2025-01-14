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
            DtCriacao = DateTime.Now;
        }
        public int IdEscalaPronta { get; set; }
        public int IdEscala { get; set; }
        public int IdPostoTrabalho { get; set; }
        public int IdFuncionario { get; set; }
        public DateTime DtDataServico { get; set; }
        public DateTime DtCriacao { get; set; }
    }
}
