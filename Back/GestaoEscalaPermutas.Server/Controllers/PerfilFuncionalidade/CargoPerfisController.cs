using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using GestaoEscalaPermutas.Dominio.Interfaces.Cargos;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfilFuncionalidades;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("cargoPerfis")]
public class CargoPerfisController : ControllerBase
{
    private readonly ICargoPerfisService _serviceCargoPerfis;
    private readonly IPerfilService _servicePerfil;
    private readonly ICargoService _serviceCargo;

    public CargoPerfisController(ICargoPerfisService serviceCargoPerfis, IPerfilService perfilService, ICargoService cargoService)
    {
        _serviceCargoPerfis = serviceCargoPerfis;
        _servicePerfil = perfilService;
        _serviceCargo = cargoService;
    }

    [HttpGet("buscarTodos")]
    public async Task<IActionResult> BuscarTodos()
    {
        var cargoPerfil = await _serviceCargoPerfis.BuscarTodos();
        var perfis = await _servicePerfil.BuscarTodos();
        var cargos = await _serviceCargo.BuscarTodos();
        // Mapeia os dados para o DTO
        var cargoPerfilDTO = cargoPerfil.Select(pf => new CargoPerfilDTO
        {
            IdPerfil = pf.IdPerfil,
            NomePerfil = perfis.FirstOrDefault(x => x.IdPerfil == pf.IdPerfil)?.Nome ?? "Perfil não encontrado",
            IdCargo = pf.IdCargo,
            NomeCargo = cargos.FirstOrDefault(x => x.IdCargo == pf.IdCargo)?.NmNome ?? "Cargo não encontrado"
        });
        return Ok(cargoPerfil);
    }

    [HttpGet("buscarPorCargo/{idCargo:Guid}")]
    public async Task<IActionResult> BuscarPerfisPorCargo(Guid idCargo)
    {
        var perfis = await _serviceCargoPerfis.BuscarPerfisPorCargo(idCargo);
        return Ok(perfis);
    }

    [HttpPost("incluir")]
    public async Task<IActionResult> AtribuirPerfilAoCargo([FromBody] CargoPerfilDTO cargoPerfilDTO)
    {
        if (cargoPerfilDTO == null || cargoPerfilDTO.IdCargo == Guid.Empty || cargoPerfilDTO.IdPerfil == Guid.Empty)
        {
            return BadRequest(new { mensagem = "Os dados enviados são inválidos." });
        }

        var resultado = await _serviceCargoPerfis.AtribuirPerfilAoCargo(cargoPerfilDTO.IdCargo, cargoPerfilDTO.IdPerfil);
        return resultado ? Ok(new { mensagem = "Perfil atribuído ao cargo com sucesso!" }) : BadRequest("Já existe essa associação.");
    }

    [HttpDelete("deletar")]
    public async Task<IActionResult> RemoverPerfilDoCargo([FromBody] CargoPerfilDTO cargoPerfilDTO)
    {
        var resultado = await _serviceCargoPerfis.RemoverPerfilDoCargo(cargoPerfilDTO.IdCargo, cargoPerfilDTO.IdPerfil);
        return resultado ? Ok(new { mensagem = "Perfil removido do cargo com sucesso!" }) : NotFound("Associação não encontrada.");
    }
}
