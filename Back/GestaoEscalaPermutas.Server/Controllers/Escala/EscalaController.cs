using AutoMapper;
using GestaoEscalaPermutas.Dominio.Interfaces.Escala;
using GestaoEscalaPermutas.Server.Models;
using Microsoft.AspNetCore.Mvc;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Server.Models.Escala;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Dominio.Interfaces.TipoEscala;
using GestaoEscalaPermutas.Dominio.DTO.EscalaPronta;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaPronta;
using GestaoEscalaPermutas.Server.Models.EscalaPronta;

namespace GestaoEscalaPermutas.Server.Controllers.Escala
{
    [ApiController]
    [Route("escala")]
    public class EscalaController : ControllerBase
    {
        private readonly IEscalaService _escalaService;
        private readonly IEscalaProntaService _escalaProntaService;
        private readonly IPostoTrabalhoService _postoTrabalhoService;
        private readonly IFuncionarioService _funcionarioService;
        private readonly ITipoEscalaService _tipoEscalaService;
        private readonly IMapper _mapper;

        public EscalaController(IEscalaService escalaService, IEscalaProntaService escalaProntaService, IMapper mapper, IPostoTrabalhoService postoTrabalhoService, IFuncionarioService funcionarioService, ITipoEscalaService tipoEscalaService) 
        {
            _escalaService = escalaService;
            _escalaProntaService = escalaProntaService;
            _mapper = mapper;
            _postoTrabalhoService = postoTrabalhoService;
            _funcionarioService = funcionarioService;
            _tipoEscalaService = tipoEscalaService;
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
        [Route("Atualizar/{id:Guid}")]
        public async Task<ActionResult> AtualizarTipoEscala(Guid id, [FromBody] EscalaDTO escala)
        {
            escala.IdEscala = id;
            var EscalaDTO = await _escalaService.Alterar(id, _mapper.Map<EscalaDTO>(escala));
            var escalaModel = _mapper.Map<EscalaModel>(EscalaDTO);
            return (escalaModel.Valido) ? Ok(escalaModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = escalaModel.Mensagem });
        }

        [HttpGet]
        [Route("buscarPorId/{id:Guid}")]
        public async Task<ActionResult<EscalaDTO>> BuscarPorIdEscalas(Guid id)
        {
            var escala = await _escalaService.BuscarPorId(id);
            if (!escala.valido)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = escala.mensagem });
            }
            return Ok(escala);
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
        [Route("Deletar/{id:Guid}")]
        public async Task<ActionResult> DeletarEscala(Guid id)
        {
            var escalasDTO = await _escalaService.Deletar(id);
            var escalasModel = _mapper.Map<EscalaModel>(escalasDTO);
            return (escalasModel.Valido) ? Ok(escalasModel.Mensagem) : BadRequest(new RetornoModel { Valido = false, Mensagem = escalasModel.Mensagem });
        }

        [HttpPost]
        [Route("montarEscala/")]
        public async Task<ActionResult> MontarEscala([FromBody] Guid idEscala)
        {
            try
            {
                // O Controller agora só chama o método do serviço que faz todo o trabalho.
                var resultado = await _escalaService.MontarEscalaAsync(idEscala);

                if (!resultado.valido)
                {
                    return BadRequest(resultado);
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RetornoModel { Valido = false, Mensagem = $"Erro inesperado ao montar escala: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("SalvarEscalaAlterada")]
        public async Task<ActionResult> AlterarEscalaPronta([FromBody] EscalaProntaDTO[] escalaProntaDTO)
        {
            try
            {
                if (escalaProntaDTO == null || escalaProntaDTO.Length == 0)
                {
                    return BadRequest(new RetornoModel
                    {
                        Valido = false,
                        Mensagem = "Nenhuma escala foi enviada para alteração."
                    });
                }

                // Extrai o ID da escala para remover os registros antigos
                var idEscala = escalaProntaDTO.First().IdEscala;

                // Chama a service para processar a alteração
                var resultado = await _escalaProntaService.AlterarEscalaPronta(idEscala, escalaProntaDTO);

                // Verifica o retorno da service
                if (!resultado.Any() || resultado.Any(e => !e.valido))
                {
                    var erro = resultado.FirstOrDefault(e => !e.valido);
                    return BadRequest(new RetornoModel
                    {
                        Valido = false,
                        Mensagem = erro?.mensagem ?? "Erro desconhecido ao salvar a escala."
                    });
                }

                // Retorna sucesso
                return Ok(new RetornoModel
                {
                    Valido = true,
                    Mensagem = "Escala alterada com sucesso."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RetornoModel
                {
                    Valido = false,
                    Mensagem = $"Erro interno ao salvar a escala: {ex.Message}"
                });
            }
        }
    }
}
