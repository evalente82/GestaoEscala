using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.EscalaExtra;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra;
using GestaoEscalaPermutas.Repository.Interfaces;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.EscalaExtra
{
    public class SolicitacaoEscalaExtraService : ISolicitacaoEscalaExtraService
    {
        private readonly ISolicitacaoEscalaExtraRepository _SolicitacaoEscalaExtraRepository;
        private readonly IMapper _mapper;

        public SolicitacaoEscalaExtraService(ISolicitacaoEscalaExtraRepository SolicitacaoEscalaExtraRepository, IMapper mapper)
        {
            _SolicitacaoEscalaExtraRepository = SolicitacaoEscalaExtraRepository;
            _mapper = mapper;
        }

        public async Task<SolicitacaoEscalaExtraDTO> BuscarPorId(Guid idEscalaExtra)
        {
            try
            {
                // Verifica se o Id fornecido é válido
                if (idEscalaExtra == Guid.Empty)
                {
                    // Retorna um DTO de erro se o ID for inválido
                    return new SolicitacaoEscalaExtraDTO
                    {
                        valido = false,
                        mensagem = "Id fora do Range."
                    };
                }

                // Busca o objeto de EscalaExtra no repositório
                var escalaExtra = await _SolicitacaoEscalaExtraRepository.ObterPorIdAsync(idEscalaExtra);

                // Verifica se o objeto foi encontrado
                if (escalaExtra == null)
                {
                    // Retorna um DTO de erro se não encontrar a permuta
                    return new SolicitacaoEscalaExtraDTO
                    {
                        valido = false,
                        mensagem = "Permuta não encontrada."
                    };
                }

                // Mapeia o objeto de EscalaExtra para DTO e retorna
                return _mapper.Map<SolicitacaoEscalaExtraDTO>(escalaExtra);
            }
            catch (Exception e)
            {
                // Lança a exceção com a mensagem de erro
                throw new Exception($"Erro ao buscar permuta: {e.Message}", e);
            }
        }

        public async Task<List<SolicitacaoEscalaExtraDTO>> BuscarTodos()
        {
            try
            {
                // Obtém todas as EscalasExtra do repositório
                var escalasExtras = await _SolicitacaoEscalaExtraRepository.ObterTodosAsync();

                // Se não houver registros, retorna uma lista vazia
                if (escalasExtras == null || !escalasExtras.Any())
                {
                    return new List<SolicitacaoEscalaExtraDTO>(); // Lista vazia
                }

                // Mapeia todas as EscalasExtra para a lista de DTOs
                return _mapper.Map<List<SolicitacaoEscalaExtraDTO>>(escalasExtras);
            }
            catch (Exception e)
            {
                // Lança uma exceção caso ocorra um erro
                throw new Exception($"Erro ao buscar todas as escalas extras: {e.Message}", e);
            }
        }

        public async Task<SolicitacaoEscalaExtraDTO> Incluir(SolicitacaoEscalaExtraDTO solicitacoesEscalaExtraDTOs)
        {
            if (solicitacoesEscalaExtraDTOs is null)
                return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "Lista de Extra vazia." };


            // Mapeia a lista de DTOs para as entidades (CriacaoEscalaExtra)
            var solicitacaoEscalaExtra = _mapper.Map<DepInfra.EscalaExtra>(solicitacoesEscalaExtraDTOs);

            // Adiciona a lista de escalas ao repositório
            var novaSolicitacaoEscalaExtra = await _SolicitacaoEscalaExtraRepository.AdicionarListaAsync(solicitacaoEscalaExtra);

            // Mapeia de volta para DTOs e retorna
            return _mapper.Map<SolicitacaoEscalaExtraDTO>(novaSolicitacaoEscalaExtra);
        }

        public async Task<SolicitacaoEscalaExtraDTO> Deletar(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "Id fora do Range." };

                var sucesso = await _SolicitacaoEscalaExtraRepository.DeletarAsync(id);
                return sucesso
                    ? new SolicitacaoEscalaExtraDTO { valido = true, mensagem = "Solicitação deletada com sucesso." }
                    : new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "Solicitação não encontrado." };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao deletar Solicitação: {e.Message}");
            }
        }

        public async Task<SolicitacaoEscalaExtraDTO> Alterar(Guid id, SolicitacaoEscalaExtraDTO escalaExtraModel)
        {
            try
            {
                if (id == Guid.Empty)
                    return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "Id fora do Range." };

                var escalaextraExistente = await _SolicitacaoEscalaExtraRepository.BuscarPorIdAsync(id);
                if (escalaextraExistente == null)
                    return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "Solicitação não encontrada." };

                _mapper.Map(escalaExtraModel, escalaextraExistente);
                var escalaExtraAtualizado = await _SolicitacaoEscalaExtraRepository.AlterarAsync(escalaextraExistente);

                return _mapper.Map<SolicitacaoEscalaExtraDTO>(escalaExtraAtualizado);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao alterar Escala Extra: {e.Message}");
            }
        }
    }
}
