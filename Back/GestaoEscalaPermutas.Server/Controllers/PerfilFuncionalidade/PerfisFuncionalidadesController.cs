using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfilFuncionalidades;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfisFuncionalidades;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("perfisFuncionalidades")]
public class PerfisFuncionalidadesController : ControllerBase
{
    private readonly IPerfisFuncionalidadesService _servicePerfilFuncionalidade;
    private readonly IPerfilService _servicePerfil;
    private readonly IFuncionalidadeService _serviceFuncionalidade;

    public PerfisFuncionalidadesController(IPerfisFuncionalidadesService service, IPerfilService servicePerfil, IFuncionalidadeService serviceFuncionalidade)
    {
        _servicePerfilFuncionalidade = service;
        _servicePerfil = servicePerfil;
        _serviceFuncionalidade = serviceFuncionalidade;
    }

    [HttpGet("buscarTodas")]
    public async Task<IEnumerable<PerfisFuncionalidadesDTO>> BuscarTodas()
    {
        var perfisFuncionalidades = await _servicePerfilFuncionalidade.BuscarTodas();
        var perfis = await _servicePerfil.BuscarTodos();
        var funcionalidades = await _serviceFuncionalidade.BuscarTodas();
        // Mapeia os dados para o DTO
        var perfisFuncionalidadesDTO = perfisFuncionalidades.Select(pf => new PerfisFuncionalidadesDTO
        {
            IdPerfil = pf.IdPerfil,
            NomePerfil = perfis.FirstOrDefault(x => x.IdPerfil == pf.IdPerfil)?.Nome ?? "Perfil não encontrado",
            IdFuncionalidade = pf.IdFuncionalidade,
            NomeFuncionalidade = funcionalidades.FirstOrDefault(x => x.IdFuncionalidade == pf.IdFuncionalidade)?.Nome ?? "Funcionalidade não encontrada"
        });

        return perfisFuncionalidadesDTO;
    }
    

    [HttpPost("incluir")]
    public async Task<IActionResult> AtribuirFuncionalidadeAoPerfil([FromBody] PerfisFuncionalidadesDTO perfisFuncionalidadesDTO)
    {
        if (perfisFuncionalidadesDTO == null || perfisFuncionalidadesDTO.IdPerfil == Guid.Empty || perfisFuncionalidadesDTO.IdFuncionalidade == Guid.Empty)
        {
            return BadRequest(new { mensagem = "Os dados enviados são inválidos." });
        }

        var resultado = await _servicePerfilFuncionalidade.AtribuirFuncionalidadeAoPerfil(perfisFuncionalidadesDTO.IdPerfil, perfisFuncionalidadesDTO.IdFuncionalidade);
        return resultado ? Ok(new { mensagem = "Funcionalidade atribuída com sucesso!" }) : BadRequest("Já existe essa associação.");
    }

    [HttpDelete("deletar")]
    public async Task<IActionResult> RemoverFuncionalidadeDoPerfil(PerfisFuncionalidadesDTO perfisFuncionalidadesDTO)
    {
        var resultado = await _servicePerfilFuncionalidade.RemoverFuncionalidadeDoPerfil(perfisFuncionalidadesDTO.IdPerfil, perfisFuncionalidadesDTO.IdFuncionalidade);
        return resultado ? Ok(new { mensagem = "Funcionalidade removida com sucesso!" }) : NotFound("Associação não encontrada.");
    }

    [HttpGet("buscarPorId/{idPerfil:Guid}")]
    public async Task<IActionResult> BuscarFuncionalidadesPorPerfil(Guid idPerfil)
    {
        var funcionalidades = await _servicePerfilFuncionalidade.BuscarFuncionalidadesPorPerfil(idPerfil);
        return Ok(funcionalidades);
    }
}
