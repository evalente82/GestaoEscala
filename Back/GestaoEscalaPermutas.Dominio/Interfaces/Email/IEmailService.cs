using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Interfaces.Email
{
    public interface IEmailService
    {
        Task EnviarEmail(string destinatario, string assunto, string corpo);
    }
}
