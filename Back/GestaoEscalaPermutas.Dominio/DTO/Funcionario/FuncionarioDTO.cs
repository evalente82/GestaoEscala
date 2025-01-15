using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.DTO.Funcionario
{
    public class FuncionarioDTO : RetornoDTO
    {
        public FuncionarioDTO()
        {
            IdFuncionario = Guid.NewGuid();
            DtCriacao = DateTime.UtcNow;
        }

        public Guid IdFuncionario { get; set; }
        public string NmNome { get; set; } = null!;

        public int NrMatricula { get; set; }

        public long? NrTelefone { get; set; }

        public string NmEndereco { get; set; } = null!;

        public int IdCargos { get; set; }

        public bool IsAtivo { get; set; }

        public string? NmEmail { get; set; }

        public string? NmSenha { get; set; }
        public DateTime DtCriacao { get; set; }

    }
}
