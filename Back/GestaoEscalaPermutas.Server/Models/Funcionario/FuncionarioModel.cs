using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Server.Models.Funcionarios
{
    public class FuncionarioModel: RetornoModel
    {
        public Guid IdFuncionario { get; set; }

        public string NmNome { get; set; } = null!;

        public int NrMatricula { get; set; }

        public long? NrTelefone { get; set; }

        public string NmEndereco { get; set; } = null!;

        public Guid IdCargos { get; set; }

        public bool IsAtivo { get; set; }

        public string? NmEmail { get; set; }

        public string? NmSenha { get; set; }
    }
}
