using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.EscalaExtra;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra;
using GestaoEscalaPermutas.Repository.Interfaces;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.EscalaExtra
{
    public class CriacaoEscalaExtraService : IEscalaExtraService
    {
        private readonly IEscalaExtraRepository _EscalaExtraRepository;
        private readonly IMapper _mapper;
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

        public async Task<EscalaExtraDTO[]> IncluirLista(EscalaExtraDTO[] escalaExtraDTOs)
        {
            if (escalaExtraDTOs is null || escalaExtraDTOs.Length == 0)
                return new EscalaExtraDTO[] { new() { valido = false, mensagem = "Lista de Escala Extra vazia." } };

            var escalaExtra = _mapper.Map<DepInfra.EscalaExtra[]>(escalaExtraDTOs);
            var novaEscalaExtra = await _EscalaExtraRepository.AdicionarListaAsync(escalaExtra);
            return _mapper.Map<EscalaExtraDTO[]>(novaEscalaExtra);
        }
    }
}
