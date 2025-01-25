using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.Permutas;
using GestaoEscalaPermutas.Dominio.Interfaces.Permutas;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.Permutas
{
    public class PermutasService : IPermutasService
    {
        private readonly DefesaCivilMaricaContext _context;
        private readonly IMapper _mapper;
        public PermutasService(DefesaCivilMaricaContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PermutasDTO> Incluir(PermutasDTO permutasDTO)
        {
            try
            {
                if (permutasDTO is null)
                {
                    return new PermutasDTO { valido = false, mensagem = "Objeto não preenchido." };
                }
                else
                {
                    var permutas = _mapper.Map<DepInfra.Permuta>(permutasDTO);

                    _context.Permuta.Add(permutas);
                    await _context.SaveChangesAsync();
                    return _mapper.Map<PermutasDTO>(permutas);
                }
            }
            catch (Exception e)
            {
                return new PermutasDTO { valido = false, mensagem = $"Erro ao receber o Objeto: {e.Message}" };
            };
        }

        public async Task<PermutasDTO> Alterar(Guid id, PermutasDTO permutaDTO)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new PermutasDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var permutaExistente = await _context.Permuta.FindAsync(id);
                    if (permutaExistente == null)
                    {
                        return new PermutasDTO { valido = false, mensagem = "escala não encontrado." };
                    }

                    // Mapeia os dados do DTO para o modelo existente (apenas as propriedades que você deseja atualizar)
                    _mapper.Map(permutaDTO, permutaExistente);

                    // O EF Core rastreará que o objeto foi modificado
                    _context.Permuta.Update(permutaExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    // Retorna o DTO atualizado (opcionalmente, você pode mapear de volta se quiser devolver os dados atualizados)
                    return _mapper.Map<PermutasDTO>(permutaExistente);
                }
            }
            catch (Exception e)
            {
                // Considerar usar um logger para registrar a exceção
                throw new Exception($"Erro ao alterar o objeto: {e.Message}");
            }
        }

        public async Task<PermutasDTO> BuscarPorId(Guid idPermuta)
        {
            try
            {
                if (idPermuta == Guid.Empty)
                {
                    return new PermutasDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var permutaExistente = await _context.Permuta.FindAsync(idPermuta);
                    if (permutaExistente == null)
                    {
                        return new PermutasDTO { valido = false, mensagem = "escala não encontrado." };
                    }
                    var permutaDTO = _mapper.Map<PermutasDTO>(permutaExistente);
                    return permutaDTO;
                }
            }
            catch (Exception e)
            {
                // Considerar usar um logger para registrar a exceção
                throw new Exception($"Erro ao buscar o objeto: {e.Message}");
            }
        }

        public async Task<List<PermutasDTO>> BuscarTodos()
        {
            try
            {
                var permutas = await _context.Permuta.ToListAsync();
                var permutaDTO = _mapper.Map<List<PermutasDTO>>(permutas);
                return permutaDTO;
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }

        public async Task<PermutasDTO> Deletar(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new PermutasDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var permutaExistente = await _context.Permuta.FindAsync(id);
                    if (permutaExistente == null)
                    {
                        return new PermutasDTO { valido = false, mensagem = "Escala não encontrado." };
                    }

                    _context.Permuta.Remove(permutaExistente);

                    await _context.SaveChangesAsync();

                    return new PermutasDTO { valido = true, mensagem = "Escala deletado com sucesso." };
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }

    }
}
