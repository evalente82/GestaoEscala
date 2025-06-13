using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.EscalaExtra;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Dominio.Services.PostoTrabalho;
using GestaoEscalaPermutas.Server.Models;
using GestaoEscalaPermutas.Server.Models.EscalaExtra;
using GestaoEscalaPermutas.Server.Models.EscalaPronta;
using GestaoEscalaPermutas.Server.Models.PostoTrabalho;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEscalaPermutas.Server.Controllers.EscalaExtra
{
    [ApiController]
    [Route("escalaExtra")]
    public class CriacaoEscalaExtra : ControllerBase
    {
        private readonly IEscalaExtraService _escalaExtraService;
        private readonly IFuncionarioService _funcionarioService;
        private readonly IMapper _mapper;

        public CriacaoEscalaExtra(IEscalaExtraService escalaExtraService, IMapper mapper, IFuncionarioService funcionarioService)
        {
            _escalaExtraService = escalaExtraService;
            _funcionarioService = funcionarioService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("buscarExtras")]
        public async Task<ActionResult> BuscarExtras()
        {
            var extras = await _escalaExtraService.BuscarTodos();

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
        public async Task<ActionResult> IncluirListaEscalaExtra([FromBody] EscalaExtraDTO escalaExtra)
        {
            try
            {
                // Verifica se o nome do funcionário foi informado
                if (string.IsNullOrEmpty(escalaExtra.NomeFuncionario))
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = "Nome do funcionário ADM não informado." });
                }

                // Busca o ID do funcionário através do nome
                var idFuncionario = await _funcionarioService.BuscarPorNomeFuncionario(escalaExtra.NomeFuncionario);

                // Verificação se o idFuncionario é inválido
                if (idFuncionario == Guid.Empty)
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = "Funcionário não encontrado." });
                }

                // Atribui o ID do funcionário à escala extra
                escalaExtra.IdFuncionario = idFuncionario;

                var escalaExtraDTOs = await _escalaExtraService.IncluirLista(_mapper.Map<EscalaExtraDTO>(escalaExtra));
                var escalaExtraModels = _mapper.Map<EscalaExtraModel>(escalaExtraDTOs);

                // Retorna o resultado correto
                return Ok(escalaExtraModels);  // Certifique-se de que Ok() está retornando a estrutura correta
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
            var escalaExtraDTO = await _escalaExtraService.Deletar(id);
            var escalaExtraModel = _mapper.Map<EscalaExtraModel>(escalaExtraDTO);
            return (escalaExtraModel.Valido) ? Ok(escalaExtraModel.Mensagem) : BadRequest(new RetornoModel { Valido = false, Mensagem = escalaExtraModel.Mensagem });
        }

        [HttpPatch]
        [Route("Atualizar/{id:Guid}")]
        public async Task<ActionResult> AtualizarEscalaExtra(Guid id, [FromBody] EscalaExtraDTO escalaExtra)
        {
            try
            {
                // Verifica se o nome do funcionário foi informado
                if (string.IsNullOrEmpty(escalaExtra.NomeFuncionario))
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = "Nome do funcionário ADM não informado." });
                }

                // Busca o ID do funcionário através do nome
                var idFuncionario = await _funcionarioService.BuscarPorNomeFuncionario(escalaExtra.NomeFuncionario);

                // Verificação se o idFuncionario é inválido
                if (idFuncionario == Guid.Empty)
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = "Funcionário não encontrado." });
                }

                // Atribui o ID do funcionário à escala extra
                escalaExtra.IdFuncionario = idFuncionario;
                escalaExtra.IdCriacaoEscalaExtra = id;

                // Tenta alterar a escala extra
                var escalaExtraDTO = await _escalaExtraService.Alterar(id, _mapper.Map<EscalaExtraDTO>(escalaExtra));

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
