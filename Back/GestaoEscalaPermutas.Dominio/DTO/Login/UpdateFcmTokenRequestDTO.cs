using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.DTO.Login
{
    public class UpdateFcmTokenRequestDTO
    {
        public string IdFuncionario { get; set; }
        public string FcmToken { get; set; }
    }
}
