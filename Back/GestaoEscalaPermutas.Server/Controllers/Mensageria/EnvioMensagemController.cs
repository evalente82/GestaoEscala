using GestaoEscalaPermutas.Dominio.Interfaces.Mensageria;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEscalaPermutas.Server.Controllers.Mensageria
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnvioMensagemController : ControllerBase
    {
        private readonly IMessageBus _messageBus;

        public EnvioMensagemController(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [HttpPost]
        [Route("enviar")]
        public async Task<IActionResult> EnviarMensagem([FromBody] string mensagem)
        {
            if (string.IsNullOrEmpty(mensagem))
            {
                return BadRequest("Mensagem não pode ser vazia.");
            }

            await _messageBus.PublishAsync("testes.queue", mensagem);
            return Ok("Mensagem enviada com sucesso!");
        }
    }
}