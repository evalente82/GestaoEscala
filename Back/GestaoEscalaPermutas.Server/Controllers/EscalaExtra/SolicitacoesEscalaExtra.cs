using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.EscalaExtra;
using GestaoEscalaPermutas.Dominio.ENUM;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Server.Models;
using GestaoEscalaPermutas.Server.Models.EscalaExtra;
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
                if (escalaExtraModels.Valido)
                {
                    return Ok(escalaExtraModels);
                }
                return BadRequest(escalaExtraModels);

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
            var escalaExtraModel = _mapper.Map<SolicitacaoEscalaExtraModel>(escalaExtraDTO);
            return (escalaExtraModel.Valido) ? Ok(escalaExtraModel.Mensagem) : BadRequest(new RetornoModel { Valido = false, Mensagem = escalaExtraModel.Mensagem });
        }

        [HttpGet]
        [Route("BuscarPorId/{idFuncionario:Guid}")]
        public async Task<ActionResult> BuscarPorIdFuncionario(Guid idFuncionario)
        {
            try
            {
                // Chama o serviço para buscar as solicitações de escala extra para o idFuncionario
                var solicitacaoEscalaExtraDTO = await _SolicitacaoEscalaExtraService.BuscarPorIdFuncionario(idFuncionario);


                // Mapeia a lista de DTOs para a lista de modelos
                var solicitacaoEscalaExtraModels = _mapper.Map<List<SolicitacaoEscalaExtraModel>>(solicitacaoEscalaExtraDTO);


                // Retorna a lista mapeada
                return Ok(solicitacaoEscalaExtraModels);
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna uma mensagem de erro
                return BadRequest(new RetornoModel { Valido = false, Mensagem = $"Erro ao buscar solicitações de escala extra: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("listar")]
        public async Task<ActionResult> ListarExtras()
        {
            var extras = await _SolicitacaoEscalaExtraService.ListarTodos();

            foreach (var extra in extras)
            {
                if (!extra.valido)
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = extra.mensagem });
                }
            }

            return Ok(extras);
        }

        [HttpGet]
        [Route("BuscarPorIdEscalaExtra/{idEscalaExtra:Guid}")]
        public async Task<ActionResult> BuscarPorIdDaEscalaExtra(Guid idEscalaExtra)
        {
            try
            {
                // Chama o serviço para buscar as solicitações de escala extra para o idFuncionario
                var solicitacaoEscalaExtraDTO = await _SolicitacaoEscalaExtraService.BuscarPorIdEscalaExtra(idEscalaExtra);

                // Mapeia a lista de DTOs para a lista de modelos
                var solicitacaoEscalaExtraModels = _mapper.Map<SolicitacaoEscalaExtraModel>(solicitacaoEscalaExtraDTO);

                // Retorna a lista mapeada
                return Ok(solicitacaoEscalaExtraModels);
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna uma mensagem de erro
                return BadRequest(new RetornoModel { Valido = false, Mensagem = $"Erro ao buscar solicitações de escala extra: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("BuscarStatusInscricao")]
        public ActionResult BuscarStatus()
        {
            try
            {
                // Pega todos os nomes do enum como um array de strings
                var listaDeStatus = Enum.GetNames(typeof(StatusInscricaoEnum));

                // Retorna a lista. O ASP.NET Core irá serializar isso para um JSON array.
                return Ok(listaDeStatus);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar os status de inscrição: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("AlterarStatusExtra/{idEscalaExtra:Guid}")]
        public async Task<ActionResult> AlterarStatusExtra(Guid idEscalaExtra, [FromQuery] string statusInscricao)
        {
            try
            {
                var solicitacaoEscalaExtraDTO = await _SolicitacaoEscalaExtraService.AlterarStatusExtra(idEscalaExtra, statusInscricao);

                if (solicitacaoEscalaExtraDTO == null || !solicitacaoEscalaExtraDTO.valido)
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = solicitacaoEscalaExtraDTO?.mensagem ?? "Não foi possível alterar o status." });
                }

                // Mapeia somente o DTO de sucesso para o modelo de retorno
                var solicitacaoEscalaExtraModel = _mapper.Map<SolicitacaoEscalaExtraModel>(solicitacaoEscalaExtraDTO);

                return Ok(solicitacaoEscalaExtraModel);
            }
            catch (Exception ex)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = $"Erro ao alterar o status: {ex.Message}" });
            }
        }
    }
}
