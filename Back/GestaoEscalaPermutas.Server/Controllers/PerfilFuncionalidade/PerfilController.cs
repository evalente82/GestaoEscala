using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfilFuncionalidades;

namespace GestaoEscalaPermutas.Server.Controllers.Perfil
{
    [ApiController]
    [Route("perfil")]
    public class PerfilController : ControllerBase
    {
        private readonly IPerfilService _perfilService;
        private readonly IMapper _mapper;

        public PerfilController(IPerfilService perfilService, IMapper mapper)
        {
            _perfilService = perfilService;
            _mapper = mapper;
        }

        [HttpGet("buscarTodos")]
        public async Task<IActionResult> BuscarTodos()
        {
            var perfis = await _perfilService.BuscarTodos();
            return Ok(perfis);
        }

        [HttpGet("buscarPorId/{id:Guid}")]
        public async Task<IActionResult> BuscarPorId(Guid id)
        {
            var perfil = await _perfilService.BuscarPorId(id);
            if (perfil == null)
                return NotFound(new { mensagem = "Perfil não encontrado." });

            return Ok(perfil);
        }

        [HttpPost("incluir")]
        public async Task<IActionResult> Incluir([FromBody] PerfilDTO perfilDTO)
        {
            var novoPerfil = await _perfilService.Criar(perfilDTO);
            return CreatedAtAction(nameof(BuscarPorId), new { id = novoPerfil.IdPerfil }, novoPerfil);
        }

        [HttpPut("atualizar/{id:Guid}")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] PerfilDTO perfilDTO)
        {
            perfilDTO.IdPerfil = id;
            if (id != perfilDTO.IdPerfil)
                return BadRequest(new { mensagem = "Id do perfil não corresponde." });

            var perfilAtualizado = await _perfilService.Atualizar(perfilDTO);
            return Ok(perfilAtualizado);
        }

        [HttpDelete("deletar/{id:Guid}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            var sucesso = await _perfilService.Deletar(id);
            if (!sucesso)
                return NotFound(new { mensagem = "Perfil não encontrado para exclusão." });

            return NoContent();
        }
    }
}
