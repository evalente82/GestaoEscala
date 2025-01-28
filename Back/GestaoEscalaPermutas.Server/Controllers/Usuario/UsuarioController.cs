using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Usuario;
using GestaoEscalaPermutas.Dominio.Interfaces.Usuario;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEscalaPermutas.Server.Controllers.Usuario
{
    [ApiController]
    [Route("usuarios")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IMapper _mapper;

        public UsuarioController(IUsuarioService usuarioService, IMapper mapper)
        {
            _usuarioService = usuarioService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> CriarUsuario([FromBody] UsuarioDTO usuarioDTO)
        {
            var usuario = await _usuarioService.Criar(usuarioDTO);
            return Ok(usuario);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult> BuscarPorId(Guid id)
        {
            var usuario = await _usuarioService.BuscarPorId(id);

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            return Ok(usuario);
        }

        [HttpGet]
        public async Task<ActionResult> BuscarTodos()
        {
            var usuarios = await _usuarioService.BuscarTodos();
            return Ok(usuarios);
        }

        [HttpPut]
        public async Task<ActionResult> AtualizarUsuario([FromBody] UsuarioDTO usuarioDTO)
        {
            var usuario = await _usuarioService.Atualizar(usuarioDTO);
            return Ok(usuario);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeletarUsuario(Guid id)
        {
            var sucesso = await _usuarioService.Deletar(id);

            if (!sucesso)
                return NotFound("Usuário não encontrado.");

            return NoContent();
        }
    }
}
