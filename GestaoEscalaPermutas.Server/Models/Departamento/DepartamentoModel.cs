using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Server.Models.Departamento
{

    public class DepartamentoModel : RetornoModel
    {
        //public DepartamentoModel()
        //{
        //    // Configuração da data de criação no construtor
        //    DtCriacao = DateTime.Now;
        //    IsAtivo = true;
        //}

        public int IdDepartamento { get; set; }
        [Required]
        public string NmNome { get; set; } = null!;

        [Required]
        public string? NmDescricao { get; set; }

        [Required]
        public bool IsAtivo { get; set; }

        [Required]
        public DateTime DtCriacao { get; set; }
        //public string DtCriacaoFormatada => DtCriacao.ToString("dd-MM-yyyy HH:mm:ss") ?? string.Empty;

    }
}
