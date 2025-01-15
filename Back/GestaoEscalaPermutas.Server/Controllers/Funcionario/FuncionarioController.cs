using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Server.Models;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Server.Models.Funcionarios;


namespace GestaoEscalaPermutas.Server.Controllers.Funcionarios
{
    [ApiController]
    [Route("funcionario")]
    public class FuncionarioController : ControllerBase
    {
        private readonly IFuncionarioService _funcionarioService;
        private readonly IMapper _mapper;

        public FuncionarioController(IFuncionarioService funcionarioService, IMapper mapper)
        {
            _funcionarioService = funcionarioService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("Incluir/")]
        public async Task<ActionResult> IncluirFuncionario([FromBody] FuncionarioDTO funcionario)
        {
            var funcionarioDTO = await _funcionarioService.Incluir(_mapper.Map<FuncionarioDTO>(funcionario));
            var funcionarioModel = _mapper.Map<FuncionarioModel>(funcionarioDTO);

            return (funcionarioModel.Valido) ? Ok(funcionarioModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = funcionarioModel.Mensagem });
        }

        [HttpPatch]
        [Route("Atualizar/{id:Guid}")]
        public async Task<ActionResult> AtualizarFuncionario(Guid id, [FromBody] FuncionarioDTO funcionario)
        {
            funcionario.IdFuncionario = id;
            var funcionarioDTO = await _funcionarioService.Alterar(id, _mapper.Map<FuncionarioDTO>(funcionario));
            var funcionarioModel = _mapper.Map<FuncionarioModel>(funcionarioDTO);
            return (funcionarioModel.Valido) ? Ok(funcionarioModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = funcionarioModel.Mensagem });
        }

        [HttpGet]
        [Route("buscarTodos")]
        public async Task<ActionResult> BuscarFuncionarios()
        {
            var funcionarios = await _funcionarioService.BuscarTodos();

            foreach (var funcionario in funcionarios)
            {
                if (!funcionario.valido)
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = funcionario.mensagem });
                }
            }

            return Ok(funcionarios);
        }

        [HttpDelete]
        [Route("Deletar/{id:Guid}")]
        public async Task<ActionResult> DeletarFuncionario(Guid id)
        {
            var funcionariosDTO = await _funcionarioService.Deletar(id);
            var funcionariosModel = _mapper.Map<FuncionarioModel>(funcionariosDTO);
            return (funcionariosModel.Valido) ? Ok(funcionariosModel.Mensagem) : BadRequest(new RetornoModel { Valido = false, Mensagem = funcionariosModel.Mensagem });
        }

        [HttpPost]
        [Route("IncluirLista/")]
        public async Task<ActionResult> IncluirListaFuncionario([FromBody] FuncionarioDTO[] funcionarios)
        {
            var funcionarioDTOs = await _funcionarioService.IncluirLista(_mapper.Map<FuncionarioDTO[]>(funcionarios));
            var funcionarioModels = _mapper.Map<List<FuncionarioModel>>(funcionarioDTOs);

            var funcionariosInvalidos = funcionarioModels.Where(fm => !fm.Valido).ToList();

            if (funcionariosInvalidos.Any())
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = string.Join(", ", funcionariosInvalidos.Select(fm => fm.Mensagem)) });
            }

            return Ok(funcionarioModels);
        }
    }
}
