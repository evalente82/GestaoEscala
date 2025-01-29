using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfilFuncionalidades;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class CargoPerfisController : ControllerBase
{
    private readonly ICargoPerfisService _service;
    private readonly IPerfilService _servicePerfil;
    private readonly IFuncionarioService _serviceFuncionario;

    public CargoPerfisController(ICargoPerfisService service, IPerfilService servicePerfil, IFuncionarioService serviceFuncionario)
    {
        _service = service;
        _servicePerfil = servicePerfil;
        _serviceFuncionario = serviceFuncionario;

    }

    [HttpGet("buscarTodos")]
    public async Task<IEnumerable<CargoPerfisDTO>> BuscarTodas()
    {
        var funcionariosPerfis = await _service.BuscarTodos();
        var perfis = await _servicePerfil.BuscarTodos();
        var funcionarios = await _serviceFuncionario.BuscarTodos();
        // Mapeia os dados para o DTO
        var perfisFuncionalidadesDTO = funcionariosPerfis.Select(pf => new CargoPerfisDTO
        {
            IdPerfil = pf.IdPerfil,
            NomePerfil = perfis.FirstOrDefault(x => x.IdPerfil == pf.IdPerfil)?.Nome?? "Nome Perfil não encontrado",
            IdFuncionario = pf.IdFuncionario,
            NomeFuncionario = funcionarios.FirstOrDefault(x => x.IdFuncionario == pf.IdFuncionario)?.NmNome?? " Nome Funcionário não encontrada"
        });

        return perfisFuncionalidadesDTO;
    }

    [HttpPost("incluir")]
    public async Task<IActionResult> AtribuirPerfilAoFuncionario([FromBody] CargoPerfisDTO funcionariosPerfisDTO)
    {
        if (funcionariosPerfisDTO.IdFuncionario == Guid.Empty || funcionariosPerfisDTO.IdPerfil == Guid.Empty)
        {
            return BadRequest(new { mensagem = "Dados inválidos." });
        }

        var resultado = await _service.AtribuirPerfilAoFuncionario(funcionariosPerfisDTO.IdFuncionario, funcionariosPerfisDTO.IdPerfil);
        return resultado ? Ok(new { mensagem = "Perfil atribuído ao funcionário com sucesso!" }) : BadRequest("Já existe essa associação.");
    }

    [HttpDelete("deletar")]
    public async Task<IActionResult> RemoverPerfilDoFuncionario([FromBody] CargoPerfisDTO funcionariosPerfisDTO)
    {
        if (funcionariosPerfisDTO.IdFuncionario == Guid.Empty || funcionariosPerfisDTO.IdPerfil == Guid.Empty)
        {
            return BadRequest(new { mensagem = "Dados inválidos." });
        }

        var resultado = await _service.RemoverPerfilDoFuncionario(funcionariosPerfisDTO.IdFuncionario, funcionariosPerfisDTO.IdPerfil);
        return resultado ? Ok(new { mensagem = "Perfil desvinculado do funcionário com sucesso!" }) : BadRequest("Erro ao desvincular o perfil.");
    }

    [HttpGet("buscarPorFuncionario/{idFuncionario:guid}")]
    public async Task<IActionResult> BuscarPerfisPorFuncionario(Guid idFuncionario)
    {
        var perfis = await _service.BuscarPerfisPorFuncionario(idFuncionario);
        return Ok(perfis);
    }
}
