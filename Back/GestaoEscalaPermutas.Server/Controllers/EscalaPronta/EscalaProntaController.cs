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
        [HttpPost]
        [Route("Incluir/")]
        public async Task<ActionResult> IncluirEscalaPronta([FromBody] EscalaProntaDTO escalaPronta)
        {
            var EscalaProntaDTO = await _escalaProntaService.Incluir(_mapper.Map<EscalaProntaDTO>(escalaPronta));
            var escalaProntaModel = _mapper.Map<EscalaProntaModel>(EscalaProntaDTO);

            return (escalaProntaModel.Valido) ? Ok(escalaProntaModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = escalaProntaModel.Mensagem });
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

        
        [HttpDelete]
        [Route("Deletar/{id:Guid}")]
        public async Task<ActionResult> DeletarEscalaPronta(Guid id)
        {
            var escalaProntaDTO = await _escalaProntaService.Deletar(id);
            var escalaProntaModel = _mapper.Map<EscalaProntaModel>(escalaProntaDTO);
            return (escalaProntaModel.Valido) ? Ok(escalaProntaModel.Mensagem) : BadRequest(new RetornoModel { Valido = false, Mensagem = escalaProntaModel.Mensagem });
        }
    }
}
