using AutoMapper;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaPronta;
using GestaoEscalaPermutas.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEscalaPermutas.Server.Controllers.EscalaExtra
{
    [ApiController]
    [Route("escalaExtra")]
    public class EscalaExtra : ControllerBase
    {
        private readonly IEscalaProntaService _escalaProntaService;
        private readonly IMapper _mapper;

        public EscalaExtra(IEscalaProntaService escalaProntaService, IMapper mapper)
        {
            _escalaProntaService = escalaProntaService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("buscarExtras")]
        public async Task<ActionResult> BuscarExtras()
        {
            var extras = await _escalaProntaService.BuscarTodos();

            foreach (var extra in extras)
            {
                if (!extra.valido)
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = extra.mensagem });
                }
            }

            return Ok(extras);
        }
    }
}
