using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.EscalaExtra;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Server.Models;
using GestaoEscalaPermutas.Server.Models.EscalaExtra;
using GestaoEscalaPermutas.Server.Models.EscalaPronta;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEscalaPermutas.Server.Controllers.EscalaExtra
{
    [ApiController]
    [Route("solicitacaoEscalaExtra")]
    public class SolicitacoesEscalaExtra : ControllerBase
    {
        private readonly ISolicitacaoEscalaExtraService _SolicitacaoEscalaExtraService;
        private readonly IFuncionarioService _funcionarioService;
        private readonly IMapper _mapper;

        public SolicitacoesEscalaExtra(ISolicitacaoEscalaExtraService escalaExtraService, IMapper mapper, IFuncionarioService funcionarioService)
        {
            _SolicitacaoEscalaExtraService = escalaExtraService;
            _funcionarioService = funcionarioService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("buscar")]
        public async Task<ActionResult> BuscarExtras()
        {
            var extras = await _SolicitacaoEscalaExtraService.BuscarTodos();

            foreach (var extra in extras)
            {
                if (!extra.valido)
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = extra.mensagem });
                }
            }

            return Ok(extras);
        }

        [HttpPost]
        [Route("Incluir")]
        public async Task<ActionResult> IncluirSolicitacaoEscalaExtra([FromBody] SolicitacaoEscalaExtraDTO escalaExtra)
        {
            try
            {
                // Mapeia o DTO para o model e realiza a operação
                var escalaExtraDTOs = await _SolicitacaoEscalaExtraService.Incluir(_mapper.Map<SolicitacaoEscalaExtraDTO>(escalaExtra));
                var escalaExtraModels = _mapper.Map<SolicitacaoEscalaExtraModel>(escalaExtraDTOs);

                return Ok(escalaExtraModels);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao incluir escala extra", error = ex.Message });
            }
        }


        [HttpDelete]
        [Route("Deletar/{id:Guid}")]
        public async Task<ActionResult> DeletarEscalaExtra(Guid id)
        {
            var escalaExtraDTO = await _SolicitacaoEscalaExtraService.Deletar(id);
            var escalaExtraModel = _mapper.Map<EscalaExtraModel>(escalaExtraDTO);
            return (escalaExtraModel.Valido) ? Ok(escalaExtraModel.Mensagem) : BadRequest(new RetornoModel { Valido = false, Mensagem = escalaExtraModel.Mensagem });
        }

        [HttpPatch]
        [Route("Atualizar/{id:Guid}")]
        public async Task<ActionResult> AtualizarEscalaExtra(Guid id, [FromBody] SolicitacaoEscalaExtraDTO escalaExtra)
        {
            try
            {
               
                escalaExtra.IdCriacaoEscalaExtra = id;

                // Tenta alterar a escala extra
                var escalaExtraDTO = await _SolicitacaoEscalaExtraService.Alterar(id, _mapper.Map<SolicitacaoEscalaExtraDTO>(escalaExtra));

                // Mapeia para o modelo final
                var escalaExtraModel = _mapper.Map<CriacaoEscalaExtraModel>(escalaExtraDTO);

                // Verifica se a operação foi válida e retorna a resposta adequada
                if (escalaExtraModel.Valido)
                {
                    return Ok(escalaExtraModel);
                }
                else
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = escalaExtraModel.Mensagem });
                }
            }
            catch (ArgumentException argEx)
            {
                // Log de erro de argumento
                Console.WriteLine($"ArgumentException: {argEx.Message}");
                return BadRequest(new RetornoModel { Valido = false, Mensagem = $"Erro de argumento: {argEx.Message}" });
            }
            catch (InvalidOperationException invOpEx)
            {
                // Log de erro de operação inválida
                Console.WriteLine($"InvalidOperationException: {invOpEx.Message}");
                return BadRequest(new RetornoModel { Valido = false, Mensagem = $"Erro na operação: {invOpEx.Message}" });
            }
            catch (Exception ex)
            {
                // Log do erro geral
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new RetornoModel { Valido = false, Mensagem = $"Erro interno do servidor: {ex.Message}" });
            }
        }


    }
}
