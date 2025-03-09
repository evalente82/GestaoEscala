using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    public class Usuarios
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IdUsuario { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string SenhaHash { get; set; }

        [Required]
        public bool Ativo { get; set; } = true;

        public Guid? IdFuncionario { get; set; }

        [Required]
        private DateTime _dataCriacao = DateTime.UtcNow;
        public DateTime DataCriacao
        {
            get => _dataCriacao;
            set => _dataCriacao = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        [ForeignKey("Perfil")]
        public Guid IdPerfil { get; set; }
        public Perfil Perfil { get; set; } = null!;

        public string? TokenRecuperacaoSenha { get; set; }
        private DateTime? _tokenExpiracao;
        public DateTime? TokenExpiracao
        {
            get => _tokenExpiracao;
            set => _tokenExpiracao = value.HasValue
                ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
                : null;
        }

        // Campos adicionados para login único e notificações
        public string? RefreshToken { get; set; }
        private DateTime? _refreshTokenExpiry;
        public DateTime? RefreshTokenExpiry
        {
            get => _refreshTokenExpiry;
            set => _refreshTokenExpiry = value.HasValue
                ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
                : null;
        }
        public string? FcmToken { get; set; } // Para notificações Firebase
    }
}