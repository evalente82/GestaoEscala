using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Setor;
using GestaoEscalaPermutas.Dominio.Interfaces.Setor;
using GestaoEscalaPermutas.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.Setor
{
    public class SetorService : ISetorService
    {
        private readonly DefesaCivilMaricaContext _context;
        private readonly IMapper _mapper;
        public SetorService(DefesaCivilMaricaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<SetorDTO> Alterar(Guid id, SetorDTO setorDTO)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new SetorDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var setorExistente = await _context.Setor.FindAsync(id);
                    if (setorExistente == null)
                    {
                        return new SetorDTO { valido = false, mensagem = "Posto não encontrado." };
                    }

                    // Mapeia os dados do DTO para o modelo existente (apenas as propriedades que você deseja atualizar)
                    _mapper.Map(setorDTO, setorExistente);

                    // O EF Core rastreará que o objeto foi modificado
                    _context.Setor.Update(setorExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    // Retorna o DTO atualizado (opcionalmente, você pode mapear de volta se quiser devolver os dados atualizados)
                    return _mapper.Map<SetorDTO>(setorExistente);
                }
            }
            catch (Exception e)
            {
                // Considerar usar um logger para registrar a exceção
                throw new Exception($"Erro ao alterar o objeto: {e.Message}");
            }
        }

        public async Task<SetorDTO> BuscarPorId(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new SetorDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var setorExistente = await _context.Setor.FindAsync(id);
                    if (setorExistente == null)
                    {
                        return new SetorDTO { valido = false, mensagem = "escala não encontrado." };
                    }
                    var setorDTO = _mapper.Map<SetorDTO>(setorExistente);
                    return setorDTO;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }

        public async Task<List<SetorDTO>> BuscarTodos()
        {
            try
            {
                var setor = await _context.Setor.ToListAsync();
                var setorDTO = _mapper.Map<List<SetorDTO>>(setor);
                return setorDTO;
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }

        public async Task<SetorDTO> Deletar(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new SetorDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var setorExistente = await _context.Setor.FindAsync(id);
                    if (setorExistente == null)
                    {
                        return new SetorDTO { valido = false, mensagem = "Posto não encontrado." };
                    }


                    // O EF Core rastreará que o objeto foi modificado
                    _context.Setor.Remove(setorExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    //retornar aviso de deletado
                    return new SetorDTO { valido = true, mensagem = "Setor deletado com sucesso." };
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }

        public async Task<SetorDTO> Incluir(SetorDTO setorDTO)
        {
            try
            {
                if (setorDTO is null)
                {
                    return new SetorDTO { valido = false, mensagem = "Objeto não preenchido." };
                }
                else
                {
                    var setor = _mapper.Map<DepInfra.Setor>(setorDTO);

                    _context.Setor.Add(setor);
                    await _context.SaveChangesAsync();
                    return _mapper.Map<SetorDTO>(setor);
                }
            }
            catch (Exception e)
            {
                return new SetorDTO { valido = false, mensagem = $"Erro ao receber o Objeto: {e.Message}" };
            }
        }
    }
}
