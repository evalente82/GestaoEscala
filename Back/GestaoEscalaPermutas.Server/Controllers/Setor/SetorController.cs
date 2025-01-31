using AutoMapper;
using GestaoEscalaPermutas.Dominio.Interfaces.Setor;
using GestaoEscalaPermutas.Server.Models;
using Microsoft.AspNetCore.Mvc;
using GestaoEscalaPermutas.Server.Models.Setor;
using GestaoEscalaPermutas.Dominio.DTO.Setor;

namespace GestaoEscalaPermutas.Server.Controllers.Setor
{
    [ApiController]
    [Route("setor")]
    public class SetorController : ControllerBase
    {
        private readonly ISetorService _setorService;
        private readonly IMapper _mapper;

        public SetorController(ISetorService setorService, IMapper mapper)
        {
            _setorService = setorService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("Incluir/")]
        public async Task<ActionResult> IncluirSetorTrabalho([FromBody] SetorDTO setor)
        {
            var setorDTO = await _setorService.Incluir(_mapper.Map<SetorDTO>(setor));
            var setorModel = _mapper.Map<SetorModel>(setorDTO);

            return (setorModel.Valido) ? Ok(setorModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = setorModel.Mensagem });
        }

        [HttpPatch]
        [Route("Atualizar/{id:Guid}")]
        public async Task<ActionResult> AtualizarSetor(Guid id, [FromBody] SetorDTO setor)
        {
            setor.IdSetor = id;
            var setorDTO = await _setorService.Alterar(id, _mapper.Map<SetorDTO>(setor));
            var setorModel = _mapper.Map<SetorModel>(setorDTO);
            return (setorModel.Valido) ? Ok(setorModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = setorModel.Mensagem });
        }

        [HttpGet]
        [Route("buscarTodos")]
        public async Task<ActionResult> BuscarSetor()
        {
            var setor = await _setorService.BuscarTodos();

            foreach (var set in setor)
            {
                if (!set.valido)
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = set.mensagem });
                }
            }

            return Ok(setor);
        }

        [HttpDelete]
        [Route("Deletar/{id:Guid}")]
        public async Task<ActionResult> DeletarSetor(Guid id)
        {
            var setorDTO = await _setorService.Deletar(id);
            var setorModel = _mapper.Map<SetorModel>(setorDTO);
            return (setorModel.Valido) ? Ok(setorModel.Mensagem) : BadRequest(new RetornoModel { Valido = false, Mensagem = setorModel.Mensagem });
        }
    }
}
