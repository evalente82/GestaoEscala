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
using GestaoEscalaPermutas.Dominio.Interfaces.Mensageria;

namespace GestaoEscalaPermutas.Server.Controllers.Permutas
{
    [ApiController]
    [Route("permutas")]
    public class PermutasController : ControllerBase
    {
        private readonly IPermutasService _permutasService;
        private readonly IMapper _mapper;
        private readonly IMessageBus _messageBus;
        public PermutasController(IPermutasService permutasService, IMapper mapper, IMessageBus messageBus)
        {
            _permutasService = permutasService;
            _mapper = mapper;
            _messageBus = messageBus;
        }

        [HttpPost]
        [Route("Incluir/")]
        public async Task<ActionResult> IncluirPermuta([FromBody] PermutasDTO escala)
        {
            var permutasDTO = await _permutasService.Incluir(_mapper.Map<PermutasDTO>(escala));
            var permutaModel = _mapper.Map<PermutaModel>(permutasDTO);

            if (permutaModel.Valido)
            {
                // Publicar mensagem para o funcionário solicitado
                var mensagem = new PermutaMensagemDTO
                {
                    IdPermuta = permutasDTO.IdPermuta,
                    IdFuncionarioSolicitante = permutasDTO.IdFuncionarioSolicitante,
                    NmNomeSolicitante = permutasDTO.NmNomeSolicitante,
                    IdFuncionarioSolicitado = permutasDTO.IdFuncionarioSolicitado,
                    NmNomeSolicitado = permutasDTO.NmNomeSolicitado,
                    DtDataSolicitadaTroca = permutasDTO.DtDataSolicitadaTroca,
                    NmStatus = "Solicitada"
                };
                await _messageBus.PublishAsync("permutas.solicitadas", mensagem);

                return Ok(permutaModel);
            }
            return BadRequest(new RetornoModel { Valido = false, Mensagem = permutaModel.Mensagem });
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

        [HttpGet]
        [Route("PermutaFuncionarioPorId/{idFuncionario:Guid}")]
        public async Task<ActionResult<List<PermutasDTO>>> BuscarPermFuncPorId(Guid idFuncionario)
        {
            var permutas = await _permutasService.BuscarFuncPorId(idFuncionario);

            if (permutas == null) // Verifica se a lista está vazia
            {
                return NotFound(new RetornoModel { Valido = false, Mensagem = "Nenhuma permuta encontrada para este funcionário." });
            }

            return Ok(permutas);
        }

        [HttpPut]
        [Route("AprovarSolicitado/{idPermuta:Guid}")]
        public async Task<ActionResult> AprovarSolicitado(Guid idPermuta)
        {
            var permuta = await _permutasService.BuscarPorId(idPermuta);
            if (!permuta.valido)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = permuta.mensagem });
            }

            // Lógica para marcar como aprovada pelo solicitado (ajuste conforme seu modelo)
            permuta.NmStatus = "AprovadaSolicitado"; // Supondo que você adicione esse campo no DTO
            var permutaAtualizada = await _permutasService.Alterar(idPermuta, permuta);

            if (permutaAtualizada.valido)
            {
                // Publicar mensagem na fila pendentes para o administrador
                var mensagem = _mapper.Map<PermutaMensagemDTO>(permutaAtualizada);
                mensagem.NmStatus = "AprovadaSolicitado";
                await _messageBus.PublishAsync("permutas.pendentes", mensagem);

                return Ok(_mapper.Map<PermutaModel>(permutaAtualizada));
            }
            return BadRequest(new RetornoModel { Valido = false, Mensagem = permutaAtualizada.mensagem });
        }

        [HttpPut]
        [Route("RecusarSolicitado/{idPermuta:Guid}")]
        public async Task<ActionResult> RecusarSolicitado(Guid idPermuta)
        {
            var permuta = await _permutasService.BuscarPorId(idPermuta);
            if (!permuta.valido)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = permuta.mensagem });
            }

            // Lógica para marcar como recusada pelo solicitado
            permuta.NmStatus = "Recusada"; // Supondo que você adicione esse campo no DTO
            var permutaAtualizada = await _permutasService.Alterar(idPermuta, permuta);

            if (permutaAtualizada.valido)
            {
                // Publicar mensagem para ambos os funcionários
                var mensagem = _mapper.Map<PermutaMensagemDTO>(permutaAtualizada);
                mensagem.NmStatus = "Recusada";
                await _messageBus.PublishAsync("permutas.resultado", mensagem);

                return Ok(_mapper.Map<PermutaModel>(permutaAtualizada));
            }
            return BadRequest(new RetornoModel { Valido = false, Mensagem = permutaAtualizada.mensagem });
        }

        [HttpPut] // Substituímos PATCH por PUT para consistência
        [Route("Aprovar/{idPermuta:Guid}")]
        public async Task<ActionResult> AprovarPermuta(Guid idPermuta)
        {
            var permuta = await _permutasService.BuscarPorId(idPermuta);
            if (!permuta.valido)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = permuta.mensagem });
            }

            permuta.NmNomeAprovador = User.Identity.Name; // Supondo que o aprovador vem do JWT
            permuta.DtAprovacao = DateTime.UtcNow;
            var permutaAtualizada = await _permutasService.Alterar(idPermuta, permuta);

            if (permutaAtualizada.valido)
            {
                var mensagem = _mapper.Map<PermutaMensagemDTO>(permutaAtualizada);
                mensagem.NmStatus = "Aprovada";
                await _messageBus.PublishAsync("permutas.resultado", mensagem);

                return Ok(_mapper.Map<PermutaModel>(permutaAtualizada));
            }
            return BadRequest(new RetornoModel { Valido = false, Mensagem = permutaAtualizada.mensagem });
        }

        [HttpPut]
        [Route("Recusar/{idPermuta:Guid}")]
        public async Task<ActionResult> RecusarPermuta(Guid idPermuta)
        {
            var permuta = await _permutasService.BuscarPorId(idPermuta);
            if (!permuta.valido)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = permuta.mensagem });
            }

            permuta.NmNomeAprovador = User.Identity.Name; // Pode ser usado como "quem recusou"
            permuta.DtReprovacao = DateTime.UtcNow;
            var permutaAtualizada = await _permutasService.Alterar(idPermuta, permuta);

            if (permutaAtualizada.valido)
            {
                var mensagem = _mapper.Map<PermutaMensagemDTO>(permutaAtualizada);
                mensagem.NmStatus = "Recusada";
                await _messageBus.PublishAsync("permutas.resultado", mensagem);

                return Ok(_mapper.Map<PermutaModel>(permutaAtualizada));
            }
            return BadRequest(new RetornoModel { Valido = false, Mensagem = permutaAtualizada.mensagem });
        }
    }
}
