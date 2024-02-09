using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.DTO.Departamento
{
    public class DepartamentoDTO : RetornoDTO
    {
        public DepartamentoDTO()
        {
            DtCriacao = DateTime.Now;
        }
        public string NmNome { get; set; } = null!;
        public string NmDescricao { get; set; } = null!;
        public bool IsAtivo { get; set; }
        [JsonIgnore]
        public DateTime DtCriacao { get; set; }

    }
}
