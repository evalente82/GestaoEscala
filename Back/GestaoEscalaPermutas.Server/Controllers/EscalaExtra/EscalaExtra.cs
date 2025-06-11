using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Cargo;
using GestaoEscalaPermutas.Dominio.DTO.EscalaExtra;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaPronta;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Server.Models;
using GestaoEscalaPermutas.Server.Models.Cargos;
using GestaoEscalaPermutas.Server.Models.EscalaExtra;
using GestaoEscalaPermutas.Server.Models.Funcionarios;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEscalaPermutas.Server.Controllers.EscalaExtra
{
    [ApiController]
    [Route("escalaExtra")]
    public class EscalaExtra : ControllerBase
    {
        private readonly IEscalaExtraService _escalaExtraService;
        private readonly IMapper _mapper;

        public EscalaExtra(IEscalaExtraService escalaExtraService, IMapper mapper)
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
        [Route("Incluir/")]
        public async Task<ActionResult> IncluirListaEscalaExtra([FromBody] EscalaExtraDTO[] escalaExtra)
        {
            var escalaExtraDTOs = await _escalaExtraService.IncluirLista(_mapper.Map<EscalaExtraDTO[]>(escalaExtra));
            var escalaExtraModels = _mapper.Map<List<EscalaExtraModel>>(escalaExtraDTOs);

            return Ok(escalaExtraModels);
        }

    }
}
