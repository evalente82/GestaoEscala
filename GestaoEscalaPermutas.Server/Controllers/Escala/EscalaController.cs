using AutoMapper;
using GestaoEscalaPermutas.Dominio.Interfaces.Escala;
using GestaoEscalaPermutas.Server.Models;
using Microsoft.AspNetCore.Mvc;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Server.Models.Escala;

namespace GestaoEscalaPermutas.Server.Controllers.Escala
{
    [ApiController]
    [Route("escala")]
    public class EscalaController:ControllerBase
    {
        private readonly IEscalaService _escalaService;
        private readonly IMapper _mapper;

        public EscalaController(IEscalaService escalaService, IMapper mapper)
        {
            _escalaService = escalaService;
            _mapper = mapper;
        }
        [HttpPost]
        [Route("Incluir/")]
        public async Task<ActionResult> IncluirEscala([FromBody] EscalaDTO escala)
        {
            var EscalaDTO = await _escalaService.Incluir(_mapper.Map<EscalaDTO>(escala));
            var escalaModel = _mapper.Map<EscalaModel>(EscalaDTO);

            return (escalaModel.Valido) ? Ok(escalaModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = escalaModel.Mensagem });
        }

        [HttpPatch]
        [Route("Atualizar/{id:int}")]
        public async Task<ActionResult> AtualizarTipoEscala(int id, [FromBody] EscalaDTO escala)
        {
            escala.IdEscala = id;
            var EscalaDTO = await _escalaService.Alterar(id, _mapper.Map<EscalaDTO>(escala));
            var escalaModel = _mapper.Map<EscalaModel>(EscalaDTO);
            return (escalaModel.Valido) ? Ok(escalaModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = escalaModel.Mensagem });
        }

        [HttpGet]
        [Route("buscarTodos")]
        public async Task<ActionResult> BuscarEscalas()
        {
            var escalas = await _escalaService.BuscarTodos();

            foreach (var escala in escalas)
            {
                if (!escala.valido)
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = escala.mensagem });
                }
            }
            return Ok(escalas);
        }

        [HttpDelete]
        [Route("Deletar/{id:int}")]
        public async Task<ActionResult> DeletarEscala(int id)
        {
            var escalasDTO = await _escalaService.Deletar(id);
            var escalasModel = _mapper.Map<EscalaModel>(escalasDTO);
            return (escalasModel.Valido) ? Ok(escalasModel.Mensagem) : BadRequest(new RetornoModel { Valido = false, Mensagem = escalasModel.Mensagem });
        }

    }
}
