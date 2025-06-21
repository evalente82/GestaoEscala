using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.DTO.reCAPTCHA
{
    public class RecaptchaVerificationRequest
    {
        public string Secret { get; set; }
        public string Response { get; set; } // Este é o token do frontend
    }
}
