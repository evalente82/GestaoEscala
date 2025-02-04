using AutoMapper;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaPronta;
using GestaoEscalaPermutas.Server.Models;
using Microsoft.AspNetCore.Mvc;
using GestaoEscalaPermutas.Dominio.DTO.EscalaPronta;
using GestaoEscalaPermutas.Server.Models.EscalaPronta;

namespace GestaoEscalaPermutas.Server.Controllers.EscalaPronta
{
    [ApiController]
    [Route("escalaPronta")]
    public class EscalaProntaController:ControllerBase
    {
        private readonly IEscalaProntaService _escalaProntaService;
        private readonly IMapper _mapper;

        public EscalaProntaController(IEscalaProntaService escalaProntaService, IMapper mapper)
        {
            _escalaProntaService = escalaProntaService;
            _mapper = mapper;
        }

        [HttpPost("RecriarEscalaProximoMes/{idEscala}")]
        public async Task<ActionResult> RecriarEscalaProximoMes(Guid idEscala)
        {
            var resultado = await _escalaProntaService.RecriarEscalaProximoMes(idEscala);
            if (!resultado.valido)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = resultado.mensagem });
            }

            return Ok(new { mensagem = "Escala recriada com sucesso!", dados = resultado });
        }

        [HttpPost("IncluirFuncionario")]
        public async Task<ActionResult> IncluirFuncionarioEscala([FromBody] EscalaProntaDTO escalaPronta)
        {
            var resultado = await _escalaProntaService.IncluirFuncionarioEscala(escalaPronta);

            if (!resultado.valido)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = resultado.mensagem });
            }

            return Ok(new { mensagem = "Funcionário adicionado com sucesso!", dados = resultado });
        }

        [HttpPatch]
        [Route("Atualizar/{id:Guid}")]
        public async Task<ActionResult> AtualizarEscalaPronta(Guid id, [FromBody] EscalaProntaDTO escalaPronta)
        {
            escalaPronta.IdEscalaPronta = id;
            var escalaProntaDTO = await _escalaProntaService.Alterar(id, _mapper.Map<EscalaProntaDTO>(escalaPronta));
            var escalaProntaModel = _mapper.Map<EscalaProntaModel>(escalaProntaDTO);
            return (escalaProntaModel.Valido) ? Ok(escalaProntaModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = escalaProntaModel.Mensagem });
        }

        [HttpDelete("DeletarOcorrenciaFuncionario")]
        public async Task<ActionResult> DeletarOcorrenciaFuncionario([FromBody] DeletarFuncionarioDTO request)
        {
            if (request.IdFuncionario == Guid.Empty || request.IdEscala == Guid.Empty)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = "IDs inválidos." });
            }

            var resultado = await _escalaProntaService.DeletarOcorrenciaFuncionario(request.IdFuncionario, request.IdEscala);

            if (!resultado.valido)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = resultado.mensagem });
            }

            return Ok(new { mensagem = "Ocorrências do funcionário removidas com sucesso!" });
        }

        [HttpGet("buscarPorId/{id}")]
        public async Task<ActionResult<List<EscalaProntaDTO>>> BuscarEscalaProntaPorId(Guid id)
        {
            var escala = await _escalaProntaService.BuscarPorId(id);

            if (escala == null)
            {
                return NotFound();
            }

            return escala;
        }
    }
}
