using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.Interfaces.Escala;
using GestaoEscalaPermutas.Dominio.Interfaces.Permutas;
using GestaoEscalaPermutas.Dominio.Services.Escala;
using GestaoEscalaPermutas.Server.Models.Escala;
using GestaoEscalaPermutas.Server.Models;
using Microsoft.AspNetCore.Mvc;
using GestaoEscalaPermutas.Dominio.DTO.Permutas;
using GestaoEscalaPermutas.Server.Models.Permuta;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Server.Models.Funcionarios;

namespace GestaoEscalaPermutas.Server.Controllers.Permutas
{
    [ApiController]
    [Route("permutas")]
    public class PermutasController : ControllerBase
    {
        private readonly IPermutasService _permutasService;
        private readonly IMapper _mapper;
        public PermutasController(IPermutasService permutasService, IMapper mapper) 
        {
            _permutasService = permutasService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("Incluir/")]
        public async Task<ActionResult> IncluirPermuta([FromBody] PermutasDTO escala)
        {
            var permutasDTO = await _permutasService.Incluir(_mapper.Map<PermutasDTO>(escala));
            var permutaModel = _mapper.Map<PermutaModel>(permutasDTO);

            return (permutaModel.Valido) ? Ok(permutaModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = permutaModel.Mensagem });
        }

        [HttpPatch]
        [Route("Atualizar/{id:Guid}")]
        public async Task<ActionResult> AtualizarPermuta(Guid id, [FromBody] PermutasDTO permuta)
        {
            permuta.IdPermuta = id;
            var permutaDTO = await _permutasService.Alterar(id, _mapper.Map<PermutasDTO>(permuta));
            var pemutaModel = _mapper.Map<PermutaModel>(permutaDTO);
            return (pemutaModel.Valido) ? Ok(pemutaModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = pemutaModel.Mensagem });
        }

        [HttpGet]
        [Route("buscarTodos")]
        public async Task<ActionResult> BuscarFuncionarios()
        {
            var permutas = await _permutasService.BuscarTodos();

            foreach (var permuta in permutas)
            {
                if (!permuta.valido)
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = permuta.mensagem });
                }
            }

            return Ok(permutas);
        }

        [HttpDelete]
        [Route("Deletar/{id:Guid}")]
        public async Task<ActionResult> DeletarFuncionario(Guid id)
        {
            var permutasDTO = await _permutasService.Deletar(id);
            var pemutasModel = _mapper.Map<PermutaModel>(permutasDTO);
            return (pemutasModel.Valido) ? Ok(pemutasModel.Mensagem) : BadRequest(new RetornoModel { Valido = false, Mensagem = pemutasModel.Mensagem });
        }

        [HttpGet]
        [Route("buscarPorId/{id:Guid}")]
        public async Task<ActionResult<PermutasDTO>> BuscarPorIdEscalas(Guid id)
        {
            var pemutas = await _permutasService.BuscarPorId(id);
            if (!pemutas.valido)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = pemutas.mensagem });
            }
            return Ok(pemutas);
        }
    }
}
