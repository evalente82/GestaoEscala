using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace GestaoEscalaPermutas.Server.Models.Login
{
    public class LoginModel : RetornoModel
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string Usuario { get; set; }
        [JsonIgnore]
        public string SenhaHash { get; set; } = string.Empty;
        [JsonIgnore]
        public string Perfil { get; set; }
    }
}
