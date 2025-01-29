using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO;
using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using GestaoEscalaPermutas.Dominio.Interfaces;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfilFuncionalidades;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("funcionalidade")]
public class FuncionalidadeController : ControllerBase
{
    private readonly IFuncionalidadeService _funcionalidadeService;
    private readonly IMapper _mapper;

    public FuncionalidadeController(IFuncionalidadeService funcionalidadeService, IMapper mapper)
    {
        _funcionalidadeService = funcionalidadeService;
        _mapper = mapper;
    }

    [HttpPost("incluir")]
    public async Task<IActionResult> Criar([FromBody] FuncionalidadeDTO funcionalidadeDTO)
    {
        var funcionalidade = await _funcionalidadeService.Criar(funcionalidadeDTO);
        return Ok(funcionalidade);
    }

    [HttpPut("atualizar/{id:Guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] FuncionalidadeDTO funcionalidadeDTO)
    {
        funcionalidadeDTO.IdFuncionalidade = id;
        if (id != funcionalidadeDTO.IdFuncionalidade)
            return BadRequest("ID informado não corresponde ao objeto.");

        var funcionalidade = await _funcionalidadeService.Atualizar(funcionalidadeDTO);
        return Ok(funcionalidade);
    }

    [HttpDelete("deletar/{id:Guid}")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        var deletado = await _funcionalidadeService.Deletar(id);
        if (!deletado)
            return NotFound("Funcionalidade não encontrada.");

        return Ok(new { mensagem = "Funcionalidade deletada com sucesso!" });
    }

    [HttpGet("buscarTodas")]
    public async Task<IActionResult> BuscarTodas()
    {
        var funcionalidades = await _funcionalidadeService.BuscarTodas();
        return Ok(funcionalidades);
    }

    [HttpGet("buscarPorId/{id:Guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        var funcionalidade = await _funcionalidadeService.BuscarPorId(id);
        if (funcionalidade == null)
            return NotFound("Funcionalidade não encontrada.");

        return Ok(funcionalidade);
    }
}
