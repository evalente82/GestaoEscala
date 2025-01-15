using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Departamento;
using GestaoEscalaPermutas.Dominio.Interfaces.Departamento;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Server.Models;
using GestaoEscalaPermutas.Server.Models.Departamento;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEscalaPermutas.Server.Controllers.Departamento
{
    [ApiController]
    [Route("departamento")]
    public class DepartamentoController :ControllerBase
    {
        private readonly IDepartamentoService _departamentoService;
        private readonly IMapper _mapper;

        public DepartamentoController(IDepartamentoService departamentoService, IMapper mapper) 
        {
            _departamentoService = departamentoService;
            _mapper = mapper;
        }


        [HttpPost]
        [Route("Incluir/")]
        public async Task<ActionResult> IncluirDepartamento([FromBody] DepartamentoDTO departamento)
        {
            var departamentoDTO = await _departamentoService.Incluir(_mapper.Map<DepartamentoDTO>(departamento));
            var departamentoModel = _mapper.Map<DepartamentoModel>(departamentoDTO);

            //return CreatedAtAction(nameof(Get), new { id = departamento.IdDepartamento }, departamento);
            return (departamentoModel.Valido) ? Ok(departamentoModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = departamentoModel.Mensagem });
        }

        [HttpPatch]
        [Route("Atualizar/{id:Guid}")]
        public async Task<ActionResult> AtualizarDepartamento(Guid id, [FromBody] DepartamentoDTO departamento)
        {
            departamento.IdDepartamento = id;
            var departamentoDTO = await _departamentoService.Alterar(id, _mapper.Map<DepartamentoDTO>(departamento));
            var departamentoModel = _mapper.Map<DepartamentoModel>(departamentoDTO);
            return (departamentoModel.Valido) ? Ok(departamentoModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = departamentoModel.Mensagem });
        }

        [HttpGet]
        [Route("buscarTodos")]
        public async Task<ActionResult> BuscarDepartamentos()
        {
            var departamentos = await _departamentoService.BuscarTodos();

            foreach (var departamento in departamentos)
            {
                if (!departamento.valido)
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = departamento.mensagem });
                }
            }

            return Ok(departamentos);
        }

        [HttpDelete]
        [Route("Deletar/{id:Guid}")]
        public async Task<ActionResult> DeletarDepartamento(Guid id)
        {
            var departamentoDTO = await _departamentoService.Deletar(id);
            var departamentoModel = _mapper.Map<DepartamentoModel>(departamentoDTO);
            return (departamentoModel.Valido) ? Ok(departamentoModel.Mensagem) : BadRequest(new RetornoModel { Valido = false, Mensagem = departamentoModel.Mensagem });
        }
    }
}
