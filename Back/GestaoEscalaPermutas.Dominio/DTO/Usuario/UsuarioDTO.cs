using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.DTO.Usuario
{
    public class UsuarioDTO
    {
        public Guid IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; } // Senha em texto simples (não armazenada)
        public string SenhaHash { get; set; } = string.Empty;// Para hash
        public bool Ativo { get; set; }
        public Guid? IdFuncionario { get; set; } // FK para Funcionarios
        public Guid IdPerfil { get; set; } // FK para Perfis
        public DateTime DataCriacao { get; set; }
        public string? TokenRecuperacaoSenha { get; set; }
        public DateTime? TokenExpiracao { get; set; }
    }
}
