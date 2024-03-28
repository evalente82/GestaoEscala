using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.Interfaces.PostoTrabalho;
using GestaoEscalaPermutas.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.PostoTrabalho
{
    public class PostoTrabalhoService : IPostoTrabalhoService
    {
        private readonly DefesaCivilMaricaContext _context;
        private readonly IMapper _mapper;
        public PostoTrabalhoService(DefesaCivilMaricaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PostoTrabalhoDTO> Incluir(PostoTrabalhoDTO postoTrabalhoDTO)
        {
            try
            {
                if (postoTrabalhoDTO is null)
                {
                    return new PostoTrabalhoDTO { valido = false, mensagem = "Objeto não preenchido." };
                }
                else
                {
                    var postoTrabalho = _mapper.Map<DepInfra.PostoTrabalho>(postoTrabalhoDTO);

                    _context.PostoTrabalhos.Add(postoTrabalho);
                    await _context.SaveChangesAsync();
                    return _mapper.Map<PostoTrabalhoDTO>(postoTrabalho);
                }
            }
            catch (Exception e)
            {
                return new PostoTrabalhoDTO { valido = false, mensagem = $"Erro ao receber o Objeto: {e.Message}" };
            }
        }

        public async Task<PostoTrabalhoDTO> Alterar(int id, PostoTrabalhoDTO postoTrabalhoModel)
        {
            try
            {
                if (id <= 0)
                {
                    return new PostoTrabalhoDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var postoTrabalhoExistente = await _context.PostoTrabalhos.FindAsync(id);
                    if (postoTrabalhoExistente == null)
                    {
                        return new PostoTrabalhoDTO { valido = false, mensagem = "Posto não encontrado." };
                    }

                    // Mapeia os dados do DTO para o modelo existente (apenas as propriedades que você deseja atualizar)
                    _mapper.Map(postoTrabalhoModel, postoTrabalhoExistente);

                    // O EF Core rastreará que o objeto foi modificado
                    _context.PostoTrabalhos.Update(postoTrabalhoExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    // Retorna o DTO atualizado (opcionalmente, você pode mapear de volta se quiser devolver os dados atualizados)
                    return _mapper.Map<PostoTrabalhoDTO>(postoTrabalhoExistente);
                }
            }
            catch (Exception e)
            {
                // Considerar usar um logger para registrar a exceção
                throw new Exception($"Erro ao alterar o objeto: {e.Message}");
            }
        }

        public async Task<List<PostoTrabalhoDTO>> BuscarTodos()
        {
            try
            {
                var postos = await _context.PostoTrabalhos.ToListAsync();
                var postoTrabalhoDTO = _mapper.Map<List<PostoTrabalhoDTO>>(postos);
                return postoTrabalhoDTO;
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }

        public async Task<PostoTrabalhoDTO> Deletar(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new PostoTrabalhoDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var funcionarioExistente = await _context.PostoTrabalhos.FindAsync(id);
                    if (funcionarioExistente == null)
                    {
                        return new PostoTrabalhoDTO { valido = false, mensagem = "Posto não encontrado." };
                    }


                    // O EF Core rastreará que o objeto foi modificado
                    _context.PostoTrabalhos.Remove(funcionarioExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    //retornar aviso de deletado
                    return new PostoTrabalhoDTO { valido = true, mensagem = "Posto deletado com sucesso." };
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }

    }
}
