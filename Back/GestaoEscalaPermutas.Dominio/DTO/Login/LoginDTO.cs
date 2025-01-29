using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.DTO.Login
{
    public class LoginDTO : RetornoDTO
    {
        public Guid Id { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; } // A senha enviada pelo cliente
        public string SenhaHash { get; set; } // Hash da senha para persistência
        public string Perfil { get; set; }
    }
}
