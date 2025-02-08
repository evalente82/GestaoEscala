using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.PostoTrabalho
{
    public class PostoTrabalhoService : IPostoTrabalhoService
    {
        private readonly IPostoTrabalhoRepository _postoTrabalhoRepository;
        private readonly IMapper _mapper;

        public PostoTrabalhoService(IPostoTrabalhoRepository postoTrabalhoRepository, IMapper mapper)
        {
            _postoTrabalhoRepository = postoTrabalhoRepository;
            _mapper = mapper;
        }

        public async Task<PostoTrabalhoDTO> Incluir(PostoTrabalhoDTO postoTrabalhoDTO)
        {
            try
            {
                if (postoTrabalhoDTO is null)
                    return new PostoTrabalhoDTO { valido = false, mensagem = "Objeto não preenchido." };

                var postoTrabalho = _mapper.Map<DepInfra.PostoTrabalho>(postoTrabalhoDTO);
                var postoCriado = await _postoTrabalhoRepository.IncluirAsync(postoTrabalho);
                return _mapper.Map<PostoTrabalhoDTO>(postoCriado);
            }
            catch (Exception e)
            {
                return new PostoTrabalhoDTO { valido = false, mensagem = $"Erro ao incluir posto de trabalho: {e.Message}" };
            }
        }

        public async Task<PostoTrabalhoDTO> Alterar(Guid id, PostoTrabalhoDTO postoTrabalhoModel)
        {
            try
            {
                if (id == Guid.Empty)
                    return new PostoTrabalhoDTO { valido = false, mensagem = "Id fora do Range." };

                var postoTrabalhoExistente = await _postoTrabalhoRepository.BuscarPorIdAsync(id);
                if (postoTrabalhoExistente == null)
                    return new PostoTrabalhoDTO { valido = false, mensagem = "Posto não encontrado." };

                _mapper.Map(postoTrabalhoModel, postoTrabalhoExistente);
                var postoAtualizado = await _postoTrabalhoRepository.AlterarAsync(postoTrabalhoExistente);

                return _mapper.Map<PostoTrabalhoDTO>(postoAtualizado);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao alterar posto de trabalho: {e.Message}");
            }
        }

        public async Task<List<PostoTrabalhoDTO>> BuscarTodos()
        {
            try
            {
                var postos = await _postoTrabalhoRepository.BuscarTodosAsync();
                return _mapper.Map<List<PostoTrabalhoDTO>>(postos);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar todos os postos de trabalho: {e.Message}");
            }
        }

        public async Task<PostoTrabalhoDTO> Deletar(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return new PostoTrabalhoDTO { valido = false, mensagem = "Id fora do Range." };

                var sucesso = await _postoTrabalhoRepository.DeletarAsync(id);
                return sucesso
                    ? new PostoTrabalhoDTO { valido = true, mensagem = "Posto de trabalho deletado com sucesso." }
                    : new PostoTrabalhoDTO { valido = false, mensagem = "Posto não encontrado." };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao deletar posto de trabalho: {e.Message}");
            }
        }

        public async Task<PostoTrabalhoDTO[]> IncluirLista(PostoTrabalhoDTO[] postoDTOs)
        {
            try
            {
                if (postoDTOs is null)
                    return new PostoTrabalhoDTO[] { new PostoTrabalhoDTO { valido = false, mensagem = "Lista de postos vazia." } };

                var postos = _mapper.Map<DepInfra.PostoTrabalho[]>(postoDTOs);
                var postosCriados = await _postoTrabalhoRepository.IncluirListaAsync(postos);
                return _mapper.Map<PostoTrabalhoDTO[]>(postosCriados);
            }
            catch (Exception e)
            {
                return new PostoTrabalhoDTO[] { new PostoTrabalhoDTO { valido = false, mensagem = $"Erro ao incluir lista de postos de trabalho: {e.Message}" } };
            }
        }

        public async Task<List<PostoTrabalhoDTO>> BuscarTodosAtivos()
        {
            try
            {
                var postosAtivos = await _postoTrabalhoRepository.BuscarTodosAtivosAsync();
                return _mapper.Map<List<PostoTrabalhoDTO>>(postosAtivos);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar postos de trabalho ativos: {e.Message}");
            }
        }
    }
}
