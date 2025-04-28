using GestaoEscalaPermutas.Dominio.Interfaces.Email;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService: IEmailService
{
    public async Task EnviarEmail(string destinatario, string assunto, string corpo)
    {
        try
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("vcorpsistem@gmail.com", "mgfl auat lbno uclk"),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("vcorpsistem@gmail.com"),
                Subject = assunto,
                Body = corpo,
                IsBodyHtml = true
            };

            mailMessage.To.Add(destinatario);

            // Envia o email assíncronamente
            await smtpClient.SendMailAsync(mailMessage);

            // Se chegar aqui, significa que o email foi enviado com sucesso
            Console.WriteLine("Email enviado com sucesso.");
        }
        catch (SmtpException smtpEx)
        {
            // Caso haja erro com o SMTP
            Console.WriteLine($"Erro ao enviar email: {smtpEx.Message}");
        }
        catch (Exception ex)
        {
            // Captura outros tipos de erros
            Console.WriteLine($"Erro inesperado: {ex.Message}");
        }
    }

}
