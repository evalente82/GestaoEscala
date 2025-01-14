using AutoMapper;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Dominio.DTO.Cargo;
using Microsoft.EntityFrameworkCore;
using GestaoEscalaPermutas.Dominio.Interfaces.Cargos;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.Cargos
{
    public class CargoService: ICargoService
    {
        private readonly DefesaCivilMaricaContext _context;
        private readonly IMapper _mapper;
        
        public CargoService(DefesaCivilMaricaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
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
                    var cargo = _mapper.Map<DepInfra.Cargo>(cargoDTO);

                    _context.Cargos.Add(cargo);
                    await _context.SaveChangesAsync();
                    return _mapper.Map<CargoDTO>(cargo);
                }
            }
            catch (Exception e)
            {
                return new CargoDTO { valido = false, mensagem = $"Erro ao receber o Objeto: {e.Message}" };
            }
        }
        public async Task<CargoDTO> Alterar(int id, CargoDTO cargoModel)
        {
            try
            {
                if (id <= 0)
                {
                    return new CargoDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var cargoExistente = await _context.Cargos.FindAsync(id);
                    if (cargoExistente == null)
                    {
                        return new CargoDTO { valido = false, mensagem = "Cargo não encontrado." };
                    }

                    // Mapeia os dados do DTO para o modelo existente (apenas as propriedades que você deseja atualizar)
                    _mapper.Map(cargoModel, cargoExistente);

                    // O EF Core rastreará que o objeto foi modificado
                    _context.Cargos.Update(cargoExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

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
                var cargos = await _context.Cargos.ToListAsync();
                var cargosDTO = _mapper.Map<List<CargoDTO>>(cargos);
                return cargosDTO;
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }
        public async Task<CargoDTO> Deletar(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new CargoDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var cargoExistente = await _context.Cargos.FindAsync(id);
                    if (cargoExistente == null)
                    {
                        return new CargoDTO { valido = false, mensagem = "Cargo não encontrado." };
                    }


                    // O EF Core rastreará que o objeto foi modificado
                    _context.Cargos.Remove(cargoExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    //retornar avido de deletado
                    return new CargoDTO { valido = true, mensagem = "Cargo deletado com sucesso." };
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }
    }
}
