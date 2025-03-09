using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.Interfaces.Mensageria;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEscalaPermutas.Server.Controllers.Mensageria
{
    [ApiController]
    [Route("menssage/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IMessageBus _messageBus;

        public UsuarioController(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [HttpPost]
        [Route("criar")]
        public async Task<IActionResult> CriarUsuario([FromBody] FuncionarioDTO usuarioDto)
        {
            

            return Ok("Usuário criado e mensagem publicada.");
        }
    }
}