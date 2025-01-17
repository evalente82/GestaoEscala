using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.EscalaPronta;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaPronta;
using GestaoEscalaPermutas.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
namespace GestaoEscalaPermutas.Dominio.Services.EscalaPronta
{
    public class EscalaProntaService : IEscalaProntaService
    {
        private readonly DefesaCivilMaricaContext _context;
        private readonly IMapper _mapper;
        public EscalaProntaService(DefesaCivilMaricaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<EscalaProntaDTO> Incluir(EscalaProntaDTO escalaProntaDTO)
        {
            try
            {
                if (escalaProntaDTO is null)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Objeto não preenchido." };
                }
                else
                {
                    var escalaPronta = _mapper.Map<DepInfra.EscalaPronta>(escalaProntaDTO);

                    _context.EscalaPronta.Add(escalaPronta);
                    await _context.SaveChangesAsync();
                    return _mapper.Map<EscalaProntaDTO>(escalaPronta);
                }
            }
            catch (Exception e)
            {
                return new EscalaProntaDTO { valido = false, mensagem = $"Erro ao receber o Objeto: {e.Message}" };
            }
        }
        public async Task<EscalaProntaDTO> Alterar(Guid id, EscalaProntaDTO escalaProntaDTO)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var escalaProntaExistente = await _context.EscalaPronta.FindAsync(id);
                    if (escalaProntaExistente == null)
                    {
                        return new EscalaProntaDTO { valido = false, mensagem = "escala não encontrado." };
                    }

                    // Mapeia os dados do DTO para o modelo existente (apenas as propriedades que você deseja atualizar)
                    _mapper.Map(escalaProntaDTO, escalaProntaExistente);

                    // O EF Core rastreará que o objeto foi modificado
                    _context.EscalaPronta.Update(escalaProntaExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    // Retorna o DTO atualizado (opcionalmente, você pode mapear de volta se quiser devolver os dados atualizados)
                    return _mapper.Map<EscalaProntaDTO>(escalaProntaExistente);
                }
            }
            catch (Exception e)
            {
                // Considerar usar um logger para registrar a exceção
                throw new Exception($"Erro ao alterar o objeto: {e.Message}");
            }
        }
        public Task<List<EscalaProntaDTO>> BuscarTodos()
        {
            throw new NotImplementedException();
        }
        public async Task<List<EscalaProntaDTO>> BuscarPorId(Guid idEscalaPronta)
        {
            try
            {
                if (idEscalaPronta == Guid.Empty)
                {
                    return new List<EscalaProntaDTO>
                    {
                        new EscalaProntaDTO
                        {
                            valido = false,
                            mensagem = "Id fora do Range."
                        }
                    };
                }
                else
                {
                    var escalaProntaExistente = await _context.EscalaPronta
                        .Where(x => x.IdEscala == idEscalaPronta)
                        .OrderBy(x => x.DtDataServico.Date)
                        .ToListAsync();
                    if (escalaProntaExistente == null)
                    {
                        return new List<EscalaProntaDTO>
                    {
                        new EscalaProntaDTO
                        {
                            valido = false,
                            mensagem = "Escala Não encontrada."
                        }
                    }; 
                    }
                    return _mapper.Map<List<EscalaProntaDTO>>(escalaProntaExistente);

                }
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }
        public async Task<EscalaProntaDTO> Deletar(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var escalaProntaExistente = await _context.EscalaPronta.FindAsync(id);
                    if (escalaProntaExistente == null)
                    {
                        return new EscalaProntaDTO { valido = false, mensagem = "Escala não encontrado." };
                    }


                    // O EF Core rastreará que o objeto foi modificado
                    _context.EscalaPronta.Remove(escalaProntaExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    //retornar avido de deletado
                    return new EscalaProntaDTO { valido = true, mensagem = "Escala deletado com sucesso." };
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }

        public async Task<EscalaProntaDTO[]> IncluirLista(EscalaProntaDTO[] escalaProntaDTOs)
        {
            try
            {

                if (escalaProntaDTOs is null)
                {
                    return new EscalaProntaDTO[] {
                        new EscalaProntaDTO {
                            valido = false, mensagem = "Lista de escala vazia."
                        }
                    };
                }
                else
                {
                    var escalaPronta = _mapper.Map<DepInfra.EscalaPronta[]>(escalaProntaDTOs);

                    _context.EscalaPronta.AddRange(escalaPronta);
                    await _context.SaveChangesAsync();

                    return _mapper.Map<EscalaProntaDTO[]>(escalaPronta);
                }
            }
            catch (Exception e)
            {
                return new EscalaProntaDTO[] { new EscalaProntaDTO { valido = false, mensagem = $"Erro ao incluir a lista de Escala: {e.Message}" } };
            }
        }
    }
}
