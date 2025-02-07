using AutoMapper;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Dominio.DTO.Cargo;
using Microsoft.EntityFrameworkCore;
using GestaoEscalaPermutas.Dominio.Interfaces.Cargos;
using GestaoEscalaPermutas.Repository.Interfaces;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.Cargos
{
    public class CargoService(ICargoRepository cargoRepository, IMapper mapper) : ICargoService
    {
        private readonly ICargoRepository _cargoRepository = cargoRepository;
        private readonly IMapper _mapper = mapper;


        public async Task<CargoDTO> Incluir(CargoDTO cargoDTO)
        {
            try
            {
                if (cargoDTO is null)
                {
                    return new CargoDTO { valido = false, mensagem = "Objeto não preenchido." };
                }
                else
                {
                    //var departamento = _mapper.Map<DepInfra.Departamento>(departamentoDTO);
                    var cargo = _mapper.Map<Cargo>(cargoDTO);

                    var novoCargo = await _cargoRepository.AdicionarAsync(cargo);
                    return _mapper.Map<CargoDTO>(novoCargo);
                }
            }
            catch (Exception e)
            {
                return new CargoDTO { valido = false, mensagem = $"Erro ao receber o Objeto: {e.Message}" };
            }
        }
        public async Task<CargoDTO> Alterar(Guid id, CargoDTO cargoModel)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new CargoDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var cargoExistente = await _cargoRepository.ObterPorIdAsync(id);
                    if (cargoExistente == null)
                    {
                        return new CargoDTO { valido = false, mensagem = "Cargo não encontrado." };
                    }

                    // Mapeia os dados do DTO para o modelo existente (apenas as propriedades que você deseja atualizar)
                    _mapper.Map(cargoModel, cargoExistente);
                    await _cargoRepository.AtualizarAsync(cargoExistente);

                    // Retorna o DTO atualizado (opcionalmente, você pode mapear de volta se quiser devolver os dados atualizados)
                    return _mapper.Map<CargoDTO>(cargoExistente);
                }
            }
            catch (Exception e)
            {
                // Considerar usar um logger para registrar a exceção
                throw new Exception($"Erro ao alterar o objeto: {e.Message}");
            }
        }
        public async Task<List<CargoDTO>> BuscarTodos()
        {
            try
            {
                var cargos = await _cargoRepository.ObterTodosAsync();
                var cargosDTO = _mapper.Map<List<CargoDTO>>(cargos);
                cargosDTO.ForEach(c => c.DefinirValidade(!string.IsNullOrWhiteSpace(c.NmNome), "Nome do cargo não pode ser vazio."));
                return cargosDTO;
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar cargos: {e.Message}");
            }
        }

        public async Task<CargoDTO> Deletar(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new CargoDTO { valido = false, mensagem = "Id fora do Range." };
                }

                var cargoExistente = await _cargoRepository.ObterPorIdAsync(id);
                if (cargoExistente == null)
                {
                    return new CargoDTO { valido = false, mensagem = "Cargo não encontrado." };
                }

                await _cargoRepository.RemoverAsync(id);

                return new CargoDTO { valido = true, mensagem = "Cargo deletado com sucesso." };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao deletar o objeto: {e.Message}");
            }
        }
    }
}
