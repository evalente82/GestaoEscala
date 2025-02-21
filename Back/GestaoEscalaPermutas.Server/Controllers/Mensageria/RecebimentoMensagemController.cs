using GestaoEscalaPermutas.Dominio.Interfaces.Mensageria;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEscalaPermutas.Server.Controllers.Mensageria
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecebimentoMensagemController : ControllerBase
    {
        private readonly IMessageBus _messageBus;
        private static readonly object _lock = new object();
        private static string _ultimaMensagemRecebida = "Nenhuma mensagem recebida ainda.";

        public RecebimentoMensagemController(IMessageBus messageBus)
        {
            _messageBus = messageBus;

            _messageBus.Subscribe<string>("testes.queue", ProcessarMensagem);
        }

        [HttpGet]
        [Route("receber")]
        public IActionResult ReceberMensagem()
        {
            return Ok(new { Mensagem = _ultimaMensagemRecebida });
        }

        private void ProcessarMensagem(string mensagem)
        {
            lock (_lock) // Evita problemas de concorrência
            {
                _ultimaMensagemRecebida = mensagem;
            }
            Console.WriteLine($"Mensagem recebida: {mensagem}");
        }
    }
}