using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Server.Models.Login
{
    public class LoginModel : RetornoModel
    {
        public Guid Id { get; set; }

        public string Usuario { get; set; }

        public string SenhaHash { get; set; }

        public string Perfil { get; set; } // Exemplo: "Administrador", "Guarda Vidas"
    }
}
