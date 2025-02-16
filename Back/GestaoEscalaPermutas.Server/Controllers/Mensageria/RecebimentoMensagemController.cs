using GestaoEscalaPermutas.Dominio.Interfaces.Mensageria;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEscalaPermutas.Server.Controllers.Mensageria
{
    //[ApiController]
    //[Route("api/[controller]")]
    //public class RecebimentoMensagemController : ControllerBase
    //{
    //    private readonly IMessageBus _messageBus;
    //    private static string _ultimaMensagemRecebida = "Nenhuma mensagem recebida ainda."; // Armazenamento compartilhado (estático)


    //    public RecebimentoMensagemController(IMessageBus messageBus)
    //    {
    //        _messageBus = messageBus;

    //        // Configurar o consumidor apenas uma vez, no construtor
    //        _messageBus.Subscribe<string>("testes.queue", ProcessarMensagem);
    //    }

    //    [HttpGet]
    //    [Route("receber")]
    //    public IActionResult ReceberMensagem()
    //    {
    //        // Retorna a última mensagem recebida
    //        return Ok(new
    //        {
    //            Mensagem = _ultimaMensagemRecebida
    //        });
    //    }


    //    private void ProcessarMensagem(string mensagem)
    //    {
    //        // Armazena a mensagem recebida
    //        _ultimaMensagemRecebida = mensagem;

    //        // Opcional: Log no console
    //        Console.WriteLine($"Mensagem recebida: {mensagem}");
    //    }
    //}
}
