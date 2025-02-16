using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.DTO.Login
{
    public class LoginResponseDTO
    {
        public bool Valido { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NomeUsuario { get; set; } = string.Empty;
        public int Matricula { get; set; }
        public Guid IdFuncionario { get; set; }
        public List<string>? Permissoes { get; set; }

    }
}
