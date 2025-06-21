using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.EscalaExtra;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra;
using GestaoEscalaPermutas.Repository.Implementations;
using GestaoEscalaPermutas.Repository.Interfaces;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.EscalaExtra
{
    public class CriacaoEscalaExtraService : IEscalaExtraService
    {
        private readonly IEscalaExtraRepository _EscalaExtraRepository;
        private readonly IMapper _mapper;

        public CriacaoEscalaExtraService(IEscalaExtraRepository escalaExtraRepository, IMapper mapper)
        {
            _EscalaExtraRepository = escalaExtraRepository;
            _mapper = mapper;
        }

        public async Task<EscalaExtraDTO> BuscarPorId(Guid idEscalaExtra)
        {
            try
            {
                // Verifica se o Id fornecido é válido
                if (idEscalaExtra == Guid.Empty)
                {
                    // Retorna um DTO de erro se o ID for inválido
                    return new EscalaExtraDTO
                    {
                        valido = false,
                        mensagem = "Id fora do Range."
                    };
                }

                // Busca o objeto de EscalaExtra no repositório
                var escalaExtra = await _EscalaExtraRepository.ObterPorIdAsync(idEscalaExtra);

                // Verifica se o objeto foi encontrado
                if (escalaExtra == null)
                {
                    // Retorna um DTO de erro se não encontrar a permuta
                    return new EscalaExtraDTO
                    {
                        valido = false,
                        mensagem = "Permuta não encontrada."
                    };
                }

                // Mapeia o objeto de EscalaExtra para DTO e retorna
                return _mapper.Map<EscalaExtraDTO>(escalaExtra);
            }
            catch (Exception e)
            {
                // Lança a exceção com a mensagem de erro
                throw new Exception($"Erro ao buscar permuta: {e.Message}", e);
            }
        }

        public async Task<List<EscalaExtraDTO>> BuscarTodos()
        {
            try
            {
                // Obtém todas as EscalasExtra do repositório
                var escalasExtras = await _EscalaExtraRepository.ObterTodosAsync();

                // Se não houver registros, retorna uma lista vazia
                if (escalasExtras == null || !escalasExtras.Any())
                {
                    return new List<EscalaExtraDTO>(); // Lista vazia
                }

                // Mapeia todas as EscalasExtra para a lista de DTOs
                return _mapper.Map<List<EscalaExtraDTO>>(escalasExtras);
            }
            catch (Exception e)
            {
                // Lança uma exceção caso ocorra um erro
                throw new Exception($"Erro ao buscar todas as escalas extras: {e.Message}", e);
            }
        }

        public async Task<EscalaExtraDTO> IncluirLista(EscalaExtraDTO escalaExtraDTOs)
        {
            if (escalaExtraDTOs is null)
                return new EscalaExtraDTO{ valido = false, mensagem = "Lista de Escala Extra vazia."  };


            // Verificando se a hora e data são válidas
                if (!string.IsNullOrEmpty(escalaExtraDTOs.horaDoServico))
                {
                // Combina a data com a hora (assumindo que horaDoServico esteja no formato "HH:mm")
                    DateTime dtExtraComHora = escalaExtraDTOs.DtEscalaExtra.Date + TimeSpan.Parse(escalaExtraDTOs.horaDoServico);
                    escalaExtraDTOs.DtEscalaExtra = dtExtraComHora; // Atualiza a data do Extra com a hora combinada
                }

                if (!string.IsNullOrEmpty(escalaExtraDTOs.HoraAbertura))
                    {
                    // Combina a data com a hora (assumindo que HoraAbertura esteja no formato "HH:mm")
                    DateTime dtAberturaComHora = escalaExtraDTOs.DtAbertura.Date + TimeSpan.Parse(escalaExtraDTOs.HoraAbertura);
                    escalaExtraDTOs.DtAbertura = dtAberturaComHora; // Atualiza a data de abertura com a hora combinada
                }

                if (!string.IsNullOrEmpty(escalaExtraDTOs.HoraFechamento))
                {
                    // Combina a data de fechamento com a hora (assumindo que HoraFechamento esteja no formato "HH:mm")
                    DateTime dtFechamentoComHora = escalaExtraDTOs.DtFechamento.Date + TimeSpan.Parse(escalaExtraDTOs.HoraFechamento);
                    escalaExtraDTOs.DtFechamento = dtFechamentoComHora; // Atualiza a data de fechamento com a hora combinada
                }
                //Console.WriteLine("Fuso horário local: " + TimeZoneInfo.Local.DisplayName);


            // Mapeia a lista de DTOs para as entidades (CriacaoEscalaExtra)
            var escalaExtra = _mapper.Map<DepInfra.CriacaoEscalaExtra>(escalaExtraDTOs);

            // Adiciona a lista de escalas ao repositório
            var novaEscalaExtra = await _EscalaExtraRepository.AdicionarListaAsync(escalaExtra);

            // Mapeia de volta para DTOs e retorna
            return _mapper.Map<EscalaExtraDTO>(novaEscalaExtra);
        }

        public async Task<EscalaExtraDTO> Deletar(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return new EscalaExtraDTO { valido = false, mensagem = "Id fora do Range." };

                var sucesso = await _EscalaExtraRepository.DeletarAsync(id);
                return sucesso
                    ? new EscalaExtraDTO { valido = true, mensagem = "Posto de trabalho deletado com sucesso." }
                    : new EscalaExtraDTO { valido = false, mensagem = "Posto não encontrado." };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao deletar posto de trabalho: {e.Message}");
            }
        }

        public async Task<EscalaExtraDTO> Alterar(Guid id, EscalaExtraDTO escalaExtraModel)
        {
            try
            {
                if (id == Guid.Empty)
                    return new EscalaExtraDTO { valido = false, mensagem = "Id fora do Range." };

                var escalaextraExistente = await _EscalaExtraRepository.BuscarListaPorIdAsync(id);
                if (escalaextraExistente == null)
                    return new EscalaExtraDTO { valido = false, mensagem = "Escala não encontrada." };

                _mapper.Map(escalaExtraModel, escalaextraExistente);
                var escalaExtraAtualizado = await _EscalaExtraRepository.AlterarAsync(escalaextraExistente);

                return _mapper.Map<EscalaExtraDTO>(escalaExtraAtualizado);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao alterar Escala Extra: {e.Message}");
            }
        }
    }
}
