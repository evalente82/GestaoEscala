using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.DTO.Login
{
    public class LoginDTO : RetornoDTO
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; } // A senha enviada pelo cliente
        [JsonIgnore]
        public string SenhaHash { get; set; } = string.Empty;
        [JsonIgnore]
        public string Perfil { get; set; } = "Sem Perfil";
    }
}
