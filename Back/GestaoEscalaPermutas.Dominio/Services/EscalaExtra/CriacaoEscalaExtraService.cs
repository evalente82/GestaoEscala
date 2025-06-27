using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.EscalaExtra;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Implementations;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.EscalaExtra
{
    public class CriacaoEscalaExtraService : IEscalaExtraService
    {
        private readonly IEscalaExtraRepository _EscalaExtraRepository;
        private readonly IEscalaExtraCargoRepository _EscalaExtraCargoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly DefesaCivilMaricaContext _context;

        public CriacaoEscalaExtraService(
            IEscalaExtraRepository escalaExtraRepository, 
            IMapper mapper, 
            IEscalaExtraCargoRepository EscalaExtraCargoRepository,
            IUnitOfWork unitOfWork,
            DefesaCivilMaricaContext context)
        {
            _EscalaExtraRepository = escalaExtraRepository;
            _mapper = mapper;
            _EscalaExtraCargoRepository = EscalaExtraCargoRepository;
            _unitOfWork = unitOfWork;
            _context = context;
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
                // 1. Chama o novo método do repositório que já traz os cargos juntos.
                var escalasComCargos = await _unitOfWork.CriacaoEscalaExtra.ObterTodosComCargosAsync();


                if (escalasComCargos == null || !escalasComCargos.Any())
                {
                    return new List<EscalaExtraDTO>();
                }

                // 2. Mapeia os resultados.
                // O AutoMapper vai mapear as propriedades principais (NmEscalaExtra, DtEscalaExtra, etc.).
                // A lista de cargos precisaremos preencher manualmente, pois os nomes das propriedades são diferentes.
                var resultadoFinalDTOs = new List<EscalaExtraDTO>();

                foreach (var escala in escalasComCargos)
                {
                    // Mapeia as propriedades simples da escala para o DTO
                    var dto = _mapper.Map<EscalaExtraDTO>(escala);

                    // Preenche a lista de IDs de cargo no DTO
                    // usando a propriedade de navegação que o .Include() populou.
                    dto.IdCargo = escala.CriacaoEscalaExtraCargos
                                        .Select(juncao => juncao.IdCargo) // Para cada item na junção, pegue o IdCargo
                                        .ToList(); // E transforme numa lista de Guids

                    // Adiciona o DTO completo à lista final
                    resultadoFinalDTOs.Add(dto);
                }

                return resultadoFinalDTOs;
            }
            catch (Exception e)
            {
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

            // PASSO 1: Prepara a adição da entidade principal (Escala)
            // O método do repositório não precisa de um 'await' se for síncrono,
            // ou se retornar Task, pode usar 'await'.
            await _unitOfWork.CriacaoEscalaExtra.AdicionarListaAsync(escalaExtra); // Supondo que o repositório use AddAsync

            // PASSO 2: Prepara a lista de entidades de junção
            var listEscalaExtraCargo = escalaExtraDTOs.IdCargo
                .Select(umIdDeCargo => new CriacaoEscalaExtraCargo
                {
                    IdCriacaoEscalaExtra = escalaExtra.IdCriacaoEscalaExtra,
                    IdCargo = umIdDeCargo
                }).ToList();

            if (listEscalaExtraCargo.Any())
            {
                // PASSO 3: Prepara a adição da lista de junção
                await _unitOfWork.EscalaExtraCargo.AdicionarListaExtraCargosAsync(listEscalaExtraCargo);
            }

            // PASSO 4: Salva TODAS as alterações numa única transação
            await _unitOfWork.CompleteAsync();

            // ======================= A CORREÇÃO ESTÁ AQUI =======================
            // Mapeia de volta a entidade 'escalaExtra' original, que o EF Core já atualizou.
            // NÃO mapeie o resultado do método do repositório.
            return _mapper.Map<EscalaExtraDTO>(escalaExtra);
        }

        // Método Deletar (versão simplificada com Cascade Delete)
        public async Task<EscalaExtraDTO> Deletar(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new EscalaExtraDTO { valido = false, mensagem = "O ID fornecido é inválido." };
            }

            try
            {
                var escalaParaDeletar = await _unitOfWork.CriacaoEscalaExtra.ObterPorIdAsync(id);

                if (escalaParaDeletar == null)
                {
                    return new EscalaExtraDTO { valido = false, mensagem = "Escala extra não encontrada." };
                }

                // Você SÓ precisa de mandar apagar a entidade principal.
                _unitOfWork.CriacaoEscalaExtra.DeletarAsync(escalaParaDeletar);

                // Ao salvar, o banco de dados irá apagar a escala E todos os seus cargos associados automaticamente.
                await _unitOfWork.CompleteAsync();

                return new EscalaExtraDTO { valido = true, mensagem = "Escala extra e seus cargos foram deletados com sucesso." };
            }
            catch (Exception e)
            {
                return new EscalaExtraDTO { valido = false, mensagem = $"Erro ao deletar a escala extra: {e.Message}" };
            }
        }


        public async Task<EscalaExtraDTO> Alterar(Guid id, EscalaExtraDTO escalaExtraModel)
        {
            try
            {
                // --- VALIDAÇÃO INICIAL ---
                if (id == Guid.Empty || id != escalaExtraModel.IdCriacaoEscalaExtra)
                {
                    return new EscalaExtraDTO { valido = false, mensagem = "ID inválido ou inconsistente." };
                }

                // --- PASSO 1: BUSCAR A ENTIDADE EXISTENTE COM SEUS RELACIONAMENTOS ---
                // Garanta que este método do repositório usa .Include(e => e.CriacaoEscalaExtraCargos)
                var escalaextraExistente = await _unitOfWork.CriacaoEscalaExtra.BuscarComCargosPorIdAsync(id);

                if (escalaextraExistente == null)
                {
                    return new EscalaExtraDTO { valido = false, mensagem = "Escala extra não encontrada." };
                }

                // --- PASSO 2: ATUALIZAR DATAS E HORAS (Sua lógica atual) ---
                // Verificando se a hora e data são válidas
                if (!string.IsNullOrEmpty(escalaExtraModel.horaDoServico))
                {
                    DateTime dtExtraComHora = escalaExtraModel.DtEscalaExtra.Date + TimeSpan.Parse(escalaExtraModel.horaDoServico);
                    // Cuidado com AddHours(3) se a data já vier com fuso. Considere usar TimeZoneInfo se necessário.
                    escalaExtraModel.DtEscalaExtra = dtExtraComHora.AddHours(3);
                }

                if (!string.IsNullOrEmpty(escalaExtraModel.HoraAbertura))
                {
                    DateTime dtAberturaComHora = escalaExtraModel.DtAbertura.Date + TimeSpan.Parse(escalaExtraModel.HoraAbertura);
                    escalaExtraModel.DtAbertura = dtAberturaComHora.AddHours(3);
                }

                if (!string.IsNullOrEmpty(escalaExtraModel.HoraFechamento))
                {
                    DateTime dtFechamentoComHora = escalaExtraModel.DtFechamento.Date + TimeSpan.Parse(escalaExtraModel.HoraFechamento);
                    escalaExtraModel.DtFechamento = dtFechamentoComHora.AddHours(3);
                }

                // --- PASSO 3: ATUALIZAR OS DADOS DA ENTIDADE PRINCIPAL ---
                // O AutoMapper copia os valores de escalaExtraModel para escalaextraExistente
                _mapper.Map(escalaExtraModel, escalaextraExistente);

                // --- PASSO 4: ATUALIZAR A TABELA DE JUNÇÃO (CriacaoEscalaExtraCargo) ---

                // 4.1. Limpa os cargos existentes. O EF vai rastrear isso como "para deletar".
                escalaextraExistente.CriacaoEscalaExtraCargos.Clear();

                // 4.2. Adiciona os novos cargos que vieram no DTO. O EF vai rastrear como "para adicionar".
                if (escalaExtraModel.IdCargo != null && escalaExtraModel.IdCargo.Any())
                {
                    foreach (var cargoId in escalaExtraModel.IdCargo)
                    {
                        escalaextraExistente.CriacaoEscalaExtraCargos.Add(new CriacaoEscalaExtraCargo
                        {
                            IdCargo = cargoId,
                            IdCriacaoEscalaExtra = escalaextraExistente.IdCriacaoEscalaExtra
                        });
                    }
                }

                // --- PASSO 5: PERSISTIR TODAS AS ALTERAÇÕES ---
                // Não é necessário chamar um método "AlterarAsync" específico se o EF já está rastreando a entidade.
                // O UnitOfWork/SaveChanges cuidará de tudo.
                await _unitOfWork.CompleteAsync();

                // --- PASSO 6: RETORNAR O RESULTADO ATUALIZADO ---
                // Mapeia a entidade que foi persistida (agora completa e atualizada) de volta para um DTO.
                return _mapper.Map<EscalaExtraDTO>(escalaextraExistente);
            }
            catch (Exception e)
            {
                // Idealmente, logar o erro aqui (e.g., com Serilog, NLog)
                throw new Exception($"Erro ao alterar Escala Extra: {e.Message}", e);
            }
        }

    }
}
