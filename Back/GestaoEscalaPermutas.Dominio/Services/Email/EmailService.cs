using GestaoEscalaPermutas.Dominio.Interfaces.Email;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService: IEmailService
{
    public async Task EnviarEmail(string destinatario, string assunto, string corpo)
    {
        var smtpClient = new SmtpClient("smtp.seuservidor.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("seuemail@dominio.com", "suaSenha"),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("seuemail@dominio.com"),
            Subject = assunto,
            Body = corpo,
            IsBodyHtml = true
        };

        mailMessage.To.Add(destinatario);

        await smtpClient.SendMailAsync(mailMessage);
    }
}
