using AutoMapper;
using GestaoEscalaPermutas.Server.Models;
using Microsoft.AspNetCore.Mvc;
using GestaoEscalaPermutas.Dominio.Interfaces.Cargos;
using GestaoEscalaPermutas.Dominio.DTO.Cargo;
using GestaoEscalaPermutas.Server.Models.Cargos;

namespace GestaoEscalaPermutas.Server.Controllers.Cargos
{
    [ApiController]
    [Route("cargo")]
    public class CargoController: ControllerBase
    {
        private readonly ICargoService _cargoService;
        private readonly IMapper _mapper;

        public CargoController(ICargoService cargoService, IMapper mapper)
        {
            _cargoService = cargoService;
            _mapper = mapper;
        }


        [HttpPost]
        [Route("Incluir/")]
        public async Task<ActionResult> IncluirCargo([FromBody] CargoDTO cargo)
        {
            var cargoDTO = await _cargoService.Incluir(_mapper.Map<CargoDTO>(cargo));
            var cargoModel = _mapper.Map<CargoModel>(cargoDTO);

            return (cargoModel.Valido) ? Ok(cargoModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = cargoModel.Mensagem });
        }

        [HttpPatch]
        [Route("Atualizar/{id:Guid}")]
        public async Task<ActionResult> AtualizarCargo(Guid id, [FromBody] CargoDTO cargo)
        {
            cargo.IdCargos= id;
            var cargoDTO = await _cargoService.Alterar(id, _mapper.Map<CargoDTO>(cargo));
            var cargoModel = _mapper.Map<CargoModel>(cargoDTO);
            return (cargoModel.Valido) ? Ok(cargoModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = cargoModel.Mensagem });
        }

        [HttpGet]
        [Route("buscarTodos")]
        public async Task<ActionResult> BuscarCargos()
        {
            var cargos = await _cargoService.BuscarTodos();

            foreach (var cargo in cargos)
            {
                if (!cargo.valido)
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = cargo.mensagem });
                }
            }

            return Ok(cargos);
        }

        [HttpDelete]
        [Route("Deletar/{id:Guid}")]
        public async Task<ActionResult> DeletarCargo(Guid id)
        {
            var cargoDTO = await _cargoService.Deletar(id);
            var cargoModel = _mapper.Map<CargoModel>(cargoDTO);
            return (cargoModel.Valido) ? Ok(cargoModel.Mensagem) : BadRequest(new RetornoModel { Valido = false, Mensagem = cargoModel.Mensagem });
        }
    }
}
