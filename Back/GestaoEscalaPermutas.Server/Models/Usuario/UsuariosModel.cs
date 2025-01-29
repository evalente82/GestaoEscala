using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Server.Models.Usuario
{
    public class UsuariosModel
    {
        public Guid IdUsuario { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string SenhaHash { get; set; }

        public bool Ativo { get; set; } = true;

        public Guid? IdFuncionario { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public string Perfil { get; set; } // Ex.: "Administrador", "Guarda Vidas"
    }
}
