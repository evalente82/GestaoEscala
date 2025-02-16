using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Server.Models.Usuario
{
    public class UsuariosModel
    {
        public Guid IdUsuario { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string SenhaHash { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

        public Guid? IdFuncionario { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public string Perfil { get; set; } = string.Empty; // Ex.: "Administrador", "Guarda Vidas"
        public string? TokenRecuperacaoSenha { get; set; }
        public DateTime? TokenExpiracao { get; set; } = DateTime.UtcNow;
    }
}
