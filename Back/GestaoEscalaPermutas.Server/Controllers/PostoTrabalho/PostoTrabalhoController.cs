using AutoMapper;
using GestaoEscalaPermutas.Server.Models;
using Microsoft.AspNetCore.Mvc;
using GestaoEscalaPermutas.Dominio.Interfaces.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho;
using GestaoEscalaPermutas.Server.Models.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Server.Models.Funcionarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace GestaoEscalaPermutas.Server.Controllers.PostoTrabalho
{
    [ApiController]
    [Route("postoTrabalho")]
    public class PostoTrabalhoController: ControllerBase
    {
        private readonly IPostoTrabalhoService _postoTrabalhoService;
        private readonly IMapper _mapper;

        public PostoTrabalhoController(IPostoTrabalhoService postoTrabalhoService, IMapper mapper)
        {
            _postoTrabalhoService = postoTrabalhoService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("Incluir/")]
        public async Task<ActionResult> IncluirPostoTrabalho([FromBody] PostoTrabalhoDTO postoTrabalho)
        {
            var postoTrabalhoDTO = await _postoTrabalhoService.Incluir(_mapper.Map<PostoTrabalhoDTO>(postoTrabalho));
            var postoTrabalhoModel = _mapper.Map<PostoTrabalhoModel>(postoTrabalhoDTO);

            return (postoTrabalhoModel.Valido) ? Ok(postoTrabalhoModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = postoTrabalhoModel.Mensagem });
        }

        [HttpPatch]
        [Route("Atualizar/{id:Guid}")]
        public async Task<ActionResult> AtualizarPostoTrabalho(Guid id, [FromBody] PostoTrabalhoDTO postoTrabalho)
        {
            postoTrabalho.IdPostoTrabalho = id;
            var postoTrabalhoDTO = await _postoTrabalhoService.Alterar(id, _mapper.Map<PostoTrabalhoDTO>(postoTrabalho));
            var postoTrabalhoModel = _mapper.Map<PostoTrabalhoModel>(postoTrabalhoDTO);
            return (postoTrabalhoModel.Valido) ? Ok(postoTrabalhoModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = postoTrabalhoModel.Mensagem });
        }

        [HttpGet]
        [Route("buscarTodos")]
        public async Task<ActionResult> BuscarPostos()
        {
            var postosTrabalhos = await _postoTrabalhoService.BuscarTodos();

            foreach (var postoTrabalho in postosTrabalhos)
            {
                if (!postoTrabalho.valido)
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = postoTrabalho.mensagem });
                }
            }

            return Ok(postosTrabalhos);
        }

        [HttpDelete]
        [Route("Deletar/{id:Guid}")]
        public async Task<ActionResult> DeletarPostoTrabalho(Guid id)
        {
            var postosDTO = await _postoTrabalhoService.Deletar(id);
            var postosModel = _mapper.Map<PostoTrabalhoModel>(postosDTO);
            return (postosModel.Valido) ? Ok(postosModel.Mensagem) : BadRequest(new RetornoModel { Valido = false, Mensagem = postosModel.Mensagem });
        }

        [HttpPost]
        [Route("IncluirLista/")]
        public async Task<ActionResult> IncluirListaPostoTrabalho([FromBody] PostoTrabalhoDTO[] postos)
        {
            var postoDTOs = await _postoTrabalhoService.IncluirLista(_mapper.Map<PostoTrabalhoDTO[]>(postos));
            var postoModels = _mapper.Map<List<PostoTrabalhoModel>>(postoDTOs);

            var postosInvalidos = postoModels.Where(fm => !fm.Valido).ToList();

            if (postosInvalidos.Any())
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = string.Join(", ", postosInvalidos.Select(fm => fm.Mensagem)) });
            }

            return Ok(postoModels);
        }       
    }
}
