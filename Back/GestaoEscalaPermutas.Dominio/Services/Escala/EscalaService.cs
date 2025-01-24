using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.Interfaces.Escala;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.Escala
{
    public class EscalaService : IEscalaService
    {
        private readonly DefesaCivilMaricaContext _context;
        private readonly IMapper _mapper;
        public EscalaService(DefesaCivilMaricaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<EscalaDTO> Incluir(EscalaDTO escalaDTO)
        {
            try
            {
                if (escalaDTO is null)
                {
                    return new EscalaDTO { valido = false, mensagem = "Objeto não preenchido." };
                }
                else
                {
                    var escala = _mapper.Map<DepInfra.Escala>(escalaDTO);

                    _context.Escalas.Add(escala);
                    await _context.SaveChangesAsync();
                    return _mapper.Map<EscalaDTO>(escala);
                }
            }
            catch (Exception e)
            {
                return new EscalaDTO { valido = false, mensagem = $"Erro ao receber o Objeto: {e.Message}" };
            }
        }
        public async Task<EscalaDTO> Alterar(Guid id, EscalaDTO escalaDTO)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new EscalaDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var escalaExistente = await _context.Escalas.FindAsync(id);
                    if (escalaExistente == null)
                    {
                        return new EscalaDTO { valido = false, mensagem = "escala não encontrado." };
                    }

                    // Mapeia os dados do DTO para o modelo existente (apenas as propriedades que você deseja atualizar)
                    _mapper.Map(escalaDTO, escalaExistente);

                    // O EF Core rastreará que o objeto foi modificado
                    _context.Escalas.Update(escalaExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    // Retorna o DTO atualizado (opcionalmente, você pode mapear de volta se quiser devolver os dados atualizados)
                    return _mapper.Map<EscalaDTO>(escalaExistente);
                }
            }
            catch (Exception e)
            {
                // Considerar usar um logger para registrar a exceção
                throw new Exception($"Erro ao alterar o objeto: {e.Message}");
            }
        }
        public async Task<List<EscalaDTO>> BuscarTodos()
        {
            try
            {
                var escalas = await _context.Escalas.ToListAsync();
                var EscalaDTO = _mapper.Map<List<EscalaDTO>>(escalas);
                return EscalaDTO;
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }
        public async Task<EscalaDTO> Deletar(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new EscalaDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var escalaExistente = await _context.Escalas.FindAsync(id);
                    var escalaProntaExistente = _context.EscalaPronta.Where(x => x.IdEscala == id).ToList();
                    if (escalaExistente == null || escalaProntaExistente == null)
                    {
                        return new EscalaDTO { valido = false, mensagem = "Escala não encontrado." };
                    }

                    _context.Escalas.Remove(escalaExistente);
                    _context.EscalaPronta.RemoveRange(escalaProntaExistente);

                    await _context.SaveChangesAsync();

                    return new EscalaDTO { valido = true, mensagem = "Escala deletado com sucesso." };
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }
        public async Task<EscalaDTO> BuscarPorId(Guid idEscala)
        {
            try
            {
                if (idEscala == Guid.Empty)
                {
                    return new EscalaDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var escalaExistente = await _context.Escalas.FindAsync(idEscala);
                    if (escalaExistente == null)
                    {
                        return new EscalaDTO { valido = false, mensagem = "escala não encontrado." };
                    }
                    var EscalaDTO = _mapper.Map<EscalaDTO>(escalaExistente);
                    return EscalaDTO;
                }
            }
            catch (Exception e)
            {
                // Considerar usar um logger para registrar a exceção
                throw new Exception($"Erro ao buscar o objeto: {e.Message}");
            }
        }

    }
}
