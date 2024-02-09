using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Departamento;
using GestaoEscalaPermutas.Dominio.Interfaces.Departamento;
using GestaoEscalaPermutas.Server.Models;
using GestaoEscalaPermutas.Server.Models.Departamento;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEscalaPermutas.Server.Controllers.Departamento
{
    [ApiController]
    [Route("api/departamento")]
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
    }
}
