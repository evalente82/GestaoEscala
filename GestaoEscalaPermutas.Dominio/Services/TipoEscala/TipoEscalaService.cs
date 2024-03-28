using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.TipoEscala;
using GestaoEscalaPermutas.Dominio.Interfaces.TipoEscala;
using GestaoEscalaPermutas.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.TipoEscala
{
    public class TipoEscalaService : ITipoEscalaService
    {
        private readonly DefesaCivilMaricaContext _context;
        private readonly IMapper _mapper;
        public TipoEscalaService(DefesaCivilMaricaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<TipoEscalaDTO> Incluir(TipoEscalaDTO tipoEscalaDTO)
        {
            try
            {
                if (tipoEscalaDTO is null)
                {
                    return new TipoEscalaDTO { valido = false, mensagem = "Objeto não preenchido." };
                }
                else
                {
                    var tipoEscala = _mapper.Map<DepInfra.TipoEscala>(tipoEscalaDTO);

                    _context.TipoEscalas.Add(tipoEscala);
                    await _context.SaveChangesAsync();
                    return _mapper.Map<TipoEscalaDTO>(tipoEscala);
                }
            }
            catch (Exception e)
            {
                return new TipoEscalaDTO { valido = false, mensagem = $"Erro ao receber o Objeto: {e.Message}" };
            }
        }
        public async Task<TipoEscalaDTO> Alterar(int id, TipoEscalaDTO tipoEscalaDTO)
        {
            try
            {
                if (id <= 0)
                {
                    return new TipoEscalaDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var tipoEscalaExistente = await _context.TipoEscalas.FindAsync(id);
                    if (tipoEscalaExistente == null)
                    {
                        return new TipoEscalaDTO { valido = false, mensagem = "tipoEscala não encontrado." };
                    }

                    // Mapeia os dados do DTO para o modelo existente (apenas as propriedades que você deseja atualizar)
                    _mapper.Map(tipoEscalaDTO, tipoEscalaExistente);

                    // O EF Core rastreará que o objeto foi modificado
                    _context.TipoEscalas.Update(tipoEscalaExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    // Retorna o DTO atualizado (opcionalmente, você pode mapear de volta se quiser devolver os dados atualizados)
                    return _mapper.Map<TipoEscalaDTO>(tipoEscalaExistente);
                }
            }
            catch (Exception e)
            {
                // Considerar usar um logger para registrar a exceção
                throw new Exception($"Erro ao alterar o objeto: {e.Message}");
            }
        }

        public async Task<List<TipoEscalaDTO>> BuscarTodos()
        {
            try
            {
                var tipoEscalas = await _context.TipoEscalas.ToListAsync();
                var TipoEscalaDTO = _mapper.Map<List<TipoEscalaDTO>>(tipoEscalas);
                return TipoEscalaDTO;
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }

        public async Task<TipoEscalaDTO> Deletar(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new TipoEscalaDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var tipoEscalaExistente = await _context.TipoEscalas.FindAsync(id);
                    if (tipoEscalaExistente == null)
                    {
                        return new TipoEscalaDTO { valido = false, mensagem = "Funacionário não encontrado." };
                    }


                    // O EF Core rastreará que o objeto foi modificado
                    _context.TipoEscalas.Remove(tipoEscalaExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    //retornar avido de deletado
                    return new TipoEscalaDTO { valido = true, mensagem = "Funcionário deletado com sucesso." };
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }

    }
}
