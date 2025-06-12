using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.EscalaExtra;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra;
using GestaoEscalaPermutas.Server.Models;
using GestaoEscalaPermutas.Server.Models.EscalaExtra;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEscalaPermutas.Server.Controllers.EscalaExtra
{
    [ApiController]
    [Route("escalaExtra")]
    public class CriacaoEscalaExtra : ControllerBase
    {
        private readonly IEscalaExtraService _escalaExtraService;
        private readonly IMapper _mapper;

        public CriacaoEscalaExtra(IEscalaExtraService escalaExtraService, IMapper mapper)
        {
            _escalaExtraService = escalaExtraService;
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
        public async Task<ActionResult> IncluirListaEscalaExtra([FromBody] EscalaExtraDTO[] escalaExtra)
        {
            try
            {
                var escalaExtraDTOs = await _escalaExtraService.IncluirLista(_mapper.Map<EscalaExtraDTO[]>(escalaExtra));
                var escalaExtraModels = _mapper.Map<List<EscalaExtraModel>>(escalaExtraDTOs);

                // Retorna o resultado correto
                return Ok(escalaExtraModels);  // Certifique-se de que Ok() está retornando a estrutura correta
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao incluir escala extra", error = ex.Message });
            }
        }

    }
}
