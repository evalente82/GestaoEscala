using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Dominio.Interfaces.TipoEscala;
using GestaoEscalaPermutas.Dominio.Services.Funcionario;
using GestaoEscalaPermutas.Dominio.Services.TipoEscala;
using GestaoEscalaPermutas.Server.Models.Funcionarios;
using GestaoEscalaPermutas.Server.Models;
using Microsoft.AspNetCore.Mvc;
using GestaoEscalaPermutas.Dominio.DTO.TipoEscala;
using GestaoEscalaPermutas.Server.Models.TipoEscala;

namespace GestaoEscalaPermutas.Server.Controllers.TipoEscala
{
    [ApiController]
    [Route("tipoEscala")]
    public class TipoEscalaController : ControllerBase
    {
        private readonly ITipoEscalaService _tipoEscalaService;
        private readonly IMapper _mapper;

        public TipoEscalaController(ITipoEscalaService tipoEscalaService, IMapper mapper)
        {
            _tipoEscalaService = tipoEscalaService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("Incluir/")]
        public async Task<ActionResult> IncluirTipoEscala([FromBody] TipoEscalaDTO tipoEscala)
        {
            var tipoEscalaDTO = await _tipoEscalaService.Incluir(_mapper.Map<TipoEscalaDTO>(tipoEscala));
            var tipoEscalaModel = _mapper.Map<TipoEscalaModel>(tipoEscalaDTO);

            return (tipoEscalaModel.Valido) ? Ok(tipoEscalaModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = tipoEscalaModel.Mensagem });
        }

        [HttpPatch]
        [Route("Atualizar/{id:Guid}")]
        public async Task<ActionResult> AtualizarTipoEscala(Guid id, [FromBody] TipoEscalaDTO tipoEscala)
        {
            tipoEscala.IdTipoEscala = id;
            var tipoEscalaDTO = await _tipoEscalaService.Alterar(id, _mapper.Map<TipoEscalaDTO>(tipoEscala));
            var tipoEscalaModel = _mapper.Map<TipoEscalaModel>(tipoEscalaDTO);
            return (tipoEscalaModel.Valido) ? Ok(tipoEscalaModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = tipoEscalaModel.Mensagem });
        }

        [HttpGet]
        [Route("buscarTodos")]
        public async Task<ActionResult> BuscarTipoEscala()
        {
            var TipoEscalas = await _tipoEscalaService.BuscarTodos();

            foreach (var tipoEscala in TipoEscalas)
            {
                if (!tipoEscala.valido)
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = tipoEscala.mensagem });
                }
            }
            return Ok(TipoEscalas);
        }

        [HttpDelete]
        [Route("Deletar/{id:Guid}")]
        public async Task<ActionResult> DeletarTipoEscala(Guid id)
        {
            var tipoEscalaDTO = await _tipoEscalaService.Deletar(id);
            var tipoEscalasModel = _mapper.Map<TipoEscalaModel>(tipoEscalaDTO);
            return (tipoEscalasModel.Valido) ? Ok(tipoEscalasModel.Mensagem) : BadRequest(new RetornoModel { Valido = false, Mensagem = tipoEscalasModel.Mensagem });
        }
    }
}
