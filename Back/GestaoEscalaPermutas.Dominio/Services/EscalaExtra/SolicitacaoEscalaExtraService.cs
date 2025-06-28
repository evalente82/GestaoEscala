using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.EscalaExtra;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra;
using GestaoEscalaPermutas.Repository.Interfaces;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Server.Settings;
using Microsoft.Extensions.Options;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.RecaptchaEnterprise.V1;
using Grpc.Core; // Para RpcException
using Microsoft.Extensions.Logging;
using GestaoEscalaPermutas.Dominio.Interfaces.Email;
using GestaoEscalaPermutas.Dominio.ENUM;
using GestaoEscalaPermutas.Repository.Implementations;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using static Google.Cloud.RecaptchaEnterprise.V1.TransactionData.Types;
namespace GestaoEscalaPermutas.Dominio.Services.EscalaExtra
{
    public class SolicitacaoEscalaExtraService : ISolicitacaoEscalaExtraService
    {
        private readonly ISolicitacaoEscalaExtraRepository _SolicitacaoEscalaExtraRepository;
        private readonly IMapper _mapper;
        private readonly IEscalaExtraRepository _escalaExtraRepository;
        private readonly ISetorRepository _setorRepository;
        private readonly RecaptchaSettings _recaptchaSettings;
        private readonly ILogger<SolicitacaoEscalaExtraService> _logger;
        private readonly IEscalaProntaRepository _escalaProntaRepository;
        private readonly IFuncionarioRepository _funcionarioRepository;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEscalaExtraCargoRepository _escalaExtraCargoRepository;
        private readonly IEscalaRepository _escalaRepository;
        private readonly ITipoEscalaRepository _tipoEscalaRepository;

        public SolicitacaoEscalaExtraService(
            ISolicitacaoEscalaExtraRepository SolicitacaoEscalaExtraRepository,
            IMapper mapper,
            IEscalaExtraRepository escalaExtraRepository,
            ISetorRepository setorRepository,
            IOptions<RecaptchaSettings> recaptchaOptions,
            ILogger<SolicitacaoEscalaExtraService> logger,
            IEscalaProntaRepository escalaProntaRepository,
            IFuncionarioRepository funcionarioRepository,
            IEmailService emailService,
            IUnitOfWork unitOfWork,
            IEscalaExtraCargoRepository escalaExtraCargoRepository,
            IEscalaRepository escalaRepository,
            ITipoEscalaRepository tipoEscalaRepository
            )
        {
            _SolicitacaoEscalaExtraRepository = SolicitacaoEscalaExtraRepository;
            _mapper = mapper;
            _escalaExtraRepository = escalaExtraRepository;
            _setorRepository = setorRepository;
            _recaptchaSettings = recaptchaOptions.Value;
            _logger = logger;
            _escalaProntaRepository = escalaProntaRepository;
            _funcionarioRepository = funcionarioRepository;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _escalaExtraCargoRepository = escalaExtraCargoRepository;
            _setorRepository = setorRepository;
            _escalaRepository = escalaRepository;
            _tipoEscalaRepository = tipoEscalaRepository;
        }

        public async Task<List<SolicitacaoEscalaExtraDTO>> BuscarPorIdFuncionario(Guid idFuncionario)
        {
            try
            {
                // Verifica se o Id fornecido é válido
                if (idFuncionario == Guid.Empty)
                {
                    // Retorna um DTO de erro se o ID for inválido
                    return new List<SolicitacaoEscalaExtraDTO>
                    {
                        new SolicitacaoEscalaExtraDTO
                        {
                            valido = false,
                            mensagem = "Id fora do Range."
                        }
                    };
                }

                // Busca o objeto de EscalaExtra no repositório
                var solicitacaoEscalaExtra = await _SolicitacaoEscalaExtraRepository.ObterListaPorIdFuncionario(idFuncionario);


                // Mapeia as entidades de EscalaExtra para a lista de DTOs e retorna
                var listSolicitacoes = _mapper.Map<List<SolicitacaoEscalaExtraDTO>>(solicitacaoEscalaExtra);

                foreach (var item in listSolicitacoes)
                {
                    var escalaExtra = await _escalaExtraRepository.BuscarListaPorIdAsync(item.IdCriacaoEscalaExtra);
                    var setor = await _setorRepository.BuscarPorIdAsync(escalaExtra.IdSetor);

                    item.NmEscalaExtra = escalaExtra.NmEscalaExtra;
                    item.NmSetor = setor.NmNome;
                    item.DtEscalaExtra = escalaExtra.DtEscalaExtra;
                }
                return listSolicitacoes;
            }
            catch (Exception e)
            {
                // Lança a exceção com a mensagem de erro
                throw new Exception($"Erro ao buscar Solicitações de escala extra: {e.Message}", e);
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
                     

            // --- Validação do reCAPTCHA (Lógica para reCAPTCHA Enterprise) ---
            if (string.IsNullOrWhiteSpace(solicitacoesEscalaExtraDTOs.RecaptchaToken))
            {
                _logger.LogWarning("Token reCAPTCHA ausente. Solicitação bloqueada.");
                return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "Token reCAPTCHA ausente. A solicitação foi bloqueada." };
            }

            try
            {
                // Crie o cliente reCAPTCHA Enterprise.
                // O cliente automaticamente gerencia a autenticação via GOOGLE_APPLICATION_CREDENTIALS.
                RecaptchaEnterpriseServiceClient client = RecaptchaEnterpriseServiceClient.Create();
                _logger.LogInformation("Cliente RecaptchaEnterpriseServiceClient criado.");

                ProjectName projectName = new ProjectName(_recaptchaSettings.GoogleCloudProjectId);

                // Crie a solicitação de avaliação (assessment).
                CreateAssessmentRequest createAssessmentRequest = new CreateAssessmentRequest()
                {
                    Assessment = new Assessment()
                    {
                        Event = new Event()
                        {
                            SiteKey = _recaptchaSettings.SiteKey,
                            Token = solicitacoesEscalaExtraDTOs.RecaptchaToken,
                            ExpectedAction = _recaptchaSettings.ExpectedAction // A ação definida no frontend
                        },
                    },
                    ParentAsProjectName = projectName
                };

                _logger.LogInformation("Enviando requisição de CreateAssessment para o Google Cloud...");
                Assessment response = await client.CreateAssessmentAsync(createAssessmentRequest); // Use CreateAssessmentAsync

                _logger.LogInformation("Resposta do CreateAssessment recebida.");

                // 1. Verifique se o token é válido.
                if (!response.TokenProperties.Valid)
                {
                    string errorMessage = $"Validação reCAPTCHA falhou: Token inválido. Razões: {response.TokenProperties.InvalidReason}";
                    _logger.LogError(errorMessage);
                    return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = errorMessage };
                }

                // 2. Verifique se a ação esperada foi executada.
                if (response.TokenProperties.Action != _recaptchaSettings.ExpectedAction)
                {
                    string errorMessage = $"Validação reCAPTCHA falhou: Ação do token não corresponde. Esperado '{_recaptchaSettings.ExpectedAction}', Recebido '{response.TokenProperties.Action}'.";
                    _logger.LogError(errorMessage);
                    return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = errorMessage };
                }

                // 3. Consulte a pontuação de risco.
                _logger.LogInformation("Pontuação reCAPTCHA: {Score}", response.RiskAnalysis.Score);
                foreach (RiskAnalysis.Types.ClassificationReason reason in response.RiskAnalysis.Reasons)
                {
                    _logger.LogInformation("Motivo de classificação de risco: {Reason}", reason.ToString());
                }

                // Para reCAPTCHA v3/Enterprise, você deve verificar a pontuação
                if (response.RiskAnalysis.Score < _recaptchaSettings.MinScore)
                {
                    string errorMessage = $"Validação reCAPTCHA v3: Pontuação muito baixa ({response.RiskAnalysis.Score}). Solicitação suspeita.";
                    _logger.LogWarning(errorMessage);
                    return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = errorMessage };
                }

                _logger.LogInformation("Validação reCAPTCHA Enterprise bem-sucedida.");
                // Se tudo passou, o token é considerado válido
            }
            catch (RpcException rpcEx) when (rpcEx.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogError(rpcEx, "Erro de argumento inválido ao chamar a API reCAPTCHA Enterprise. Isso pode indicar uma chave de projeto inválida, siteKey incorreta, ou problema com as credenciais.");
                return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = $"Erro de configuração reCAPTCHA: {rpcEx.Status.Detail}" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado na validação do reCAPTCHA Enterprise.");
                return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = $"Erro inesperado na validação do reCAPTCHA: {ex.Message}" };
            }
            // --- Fim da Validação do reCAPTCHA ---

            try
            {
                // --- 1. BUSCAR DADOS INICIAIS ---
                var solicitacaoEscalaExtra = _mapper.Map<DepInfra.EscalaExtra>(solicitacoesEscalaExtraDTOs);
                var funcionario = await _funcionarioRepository.ObterPorIdAsync(solicitacoesEscalaExtraDTOs.IdFuncionario);
                var extrasDisponiveis = await _escalaExtraRepository.BuscarListaPorIdAsync(solicitacaoEscalaExtra.IdCriacaoEscalaExtra);
                var escalasProntasDoFuncionario = await _escalaProntaRepository.BuscarPorIdFuncionario(solicitacoesEscalaExtraDTOs.IdFuncionario);

                if (funcionario == null || extrasDisponiveis == null)
                    return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "Funcionário ou Escala Extra não encontrados." };
                if (!funcionario.IsAtivo)
                    return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "Funcionário Inativo." };

                // --- 2. VALIDAÇÃO DE CARGO ---
                var cargosPermitidosIds = await _escalaExtraCargoRepository.ObterCargosPorEscalaExtraIdAsync(extrasDisponiveis.IdCriacaoEscalaExtra);
                if (cargosPermitidosIds.Any() && !cargosPermitidosIds.Contains(funcionario.IdCargo))
                {
                    return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "Seu cargo não é elegível para esta escala." };
                }

                // --- 3. VALIDAÇÃO DE DESCANSO MÍNIMO DE 11 HORAS ---
                DateTime inicioDoExtra = extrasDisponiveis.DtEscalaExtra;

                // Pré-carrega os detalhes das escalas para otimizar
                var idsDasEscalasRegulares = escalasProntasDoFuncionario.Select(ep => ep.IdEscala).Distinct().ToList();
                var escalasRegularesCompletas = await _escalaRepository.ObterEscalasComTipoPorIdsAsync(idsDasEscalasRegulares);
                var mapaDeEscalas = escalasRegularesCompletas.ToDictionary(e => e.IdEscala);

                foreach (var plantaoRegular in escalasProntasDoFuncionario)
                {
                    if (mapaDeEscalas.TryGetValue(plantaoRegular.IdEscala, out var detalhesDaEscalaRegular) && detalhesDaEscalaRegular.IdTipoEscala != null)
                    {
                        var tipoDaEscala = detalhesDaEscalaRegular.TipoEscala;

                        // CORREÇÃO DO ERRO 'ToDateTime': Combinando a data do plantão com a hora de início
                        DateTime inicioDoPlantao = plantaoRegular.DtDataServico.Date + tipoDaEscala.HoraInicio.ToTimeSpan();
                        DateTime fimDoPlantao = inicioDoPlantao.AddHours(tipoDaEscala.NrHorasTrabalhada);

                        if (inicioDoExtra > fimDoPlantao)
                        {
                            TimeSpan descanso = inicioDoExtra - fimDoPlantao;
                            if (descanso.TotalHours < 11)
                            {
                                return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = $"Descanso insuficiente. Mínimo de 11h necessário após o plantão que termina em {fimDoPlantao:dd/MM/yyyy HH:mm}h." };
                            }
                        }
                    }
                }

                // --- 4. VALIDAÇÃO DE INSCRIÇÃO DUPLICADA NO MESMO DIA ---
                var inscricoesNoMesmoDia = await _SolicitacaoEscalaExtraRepository.ObterInscricoesPorFuncionarioEData(funcionario.IdFuncionario, inicioDoExtra.Date);
                if (inscricoesNoMesmoDia.Any(i => i.StatusInscricao != StatusInscricaoEnum.Cancelado.ToString()))
                {
                    return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "Funcionário já possui inscrição em extra nesta data." };
                }

                // --- 5. LÓGICA DE VAGAS E FILA DE ESPERA ---
                StatusInscricaoEnum statusDaInscricao;
                if (extrasDisponiveis.QtdVagas > 0)
                {
                    statusDaInscricao = StatusInscricaoEnum.Confirmado;
                    extrasDisponiveis.QtdVagas--;
                }
                else if (extrasDisponiveis.QtdFilaEspera > 0)
                {
                    statusDaInscricao = StatusInscricaoEnum.FilaDeEspera;
                    extrasDisponiveis.QtdFilaEspera--;
                }
                else
                {
                    return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "Não há mais vagas disponíveis, nem na fila de espera." };
                }

                // --- 6. SALVANDO AS ALTERAÇÕES ---
                solicitacaoEscalaExtra.StatusInscricao = statusDaInscricao.ToString();
                await _SolicitacaoEscalaExtraRepository.AdicionarListaAsync(solicitacaoEscalaExtra);
                await _escalaExtraRepository.AlterarAsync(extrasDisponiveis);
                await _unitOfWork.CompleteAsync();

                //enviar e-mail
                try
                {
                    var dataServico = extrasDisponiveis.DtEscalaExtra.ToString("dd/MM/yyyy");
                    var horaServico = extrasDisponiveis.DtEscalaExtra.AddHours(-3).ToString("HH:mm");
                    var setor = await _setorRepository.BuscarPorIdAsync(extrasDisponiveis.IdSetor);
                    var setorServico = setor.NmNome;
                    var status = statusDaInscricao.ToString();

                    string corpoEmail = $@"
                    <html>
                    <head>
                        <style>
                            body {{ font-family: sans-serif; }}
                            h2 {{ color: #0056b3; }} /* Cor opcional para o título */
                            .details {{ margin-top: 15px; }}
                            .signature {{ margin-top: 20px; }}
                        </style>
                    </head>
                    <body>
                        <h2>Agendamento com sucesso.</h2>

                        <div class=""details"">
                            <strong>Status:</strong> {status}<br>
                            <strong>Data:</strong> {dataServico}<br>
                            <strong>Hora:</strong> {horaServico}<br>
                            <strong>Setor:</strong> {setorServico}<br>
                            <strong>Funcionário:</strong> {funcionario.NmNome}<br>
                            <strong>Matrícula:</strong> {funcionario.NrMatricula}
                        </div>

                        <div class=""signature"">
                            <p>Atenciosamente,<br>Defesa Civil de Maricá.</p>
                        </div>
                    </body>
                    </html>
                    ";

                    await _emailService.EnviarEmail(funcionario.NmEmail = "endrigo.valente@gmail.com", "Cadastro para Serviço Extra", corpoEmail);
                }
                catch (Exception ex)
                {

                    _logger.LogError(ex, "Erro inesperado no envio de e-mail.");
                    return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = $"Erro inesperado no envio de e-mail: {ex.Message}" };
                }


                // Mapeia de volta para DTOs e retorna
                return _mapper.Map<SolicitacaoEscalaExtraDTO>(solicitacaoEscalaExtra);
            }
            catch (Exception ex)
            {
                return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = $"Erro inesperado: {ex.Message}" };
            }
            
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

        public async Task<List<VisualizarSolicitacoesDTO>> ListarTodos()
        {
            try
            {
                var listVisualizarSolicitacoesDTO = new List<VisualizarSolicitacoesDTO>();

                // Obtém todas as EscalasExtra do repositório
                var escalasExtras = await _SolicitacaoEscalaExtraRepository.ObterTodosAsync();

                // Se não houver registros, retorna uma lista vazia
                if (escalasExtras == null || !escalasExtras.Any())
                {
                    return new List<VisualizarSolicitacoesDTO>(); // Lista vazia
                }

                //buscar nome funcionario NmFuncionario
                foreach (var item in escalasExtras)
                {
                    var visualizarSolicitacoesDTO = new VisualizarSolicitacoesDTO();
                    var funcionario = await _funcionarioRepository.ObterPorIdAsync(item.IdFuncionario);
                    //buscar nome da escala extra NmEscalaExtra
                    var escala = await _escalaExtraRepository.BuscarListaPorIdAsync(item.IdCriacaoEscalaExtra);

                    //buscar nome do setor NmSetor
                    var setor = await _setorRepository.BuscarPorIdAsync(escala.IdSetor);

                    //IdCriacaoEscalaExtra e IdEscalaExtra
                    visualizarSolicitacoesDTO.IdEscalaExtra = item.IdEscalaExtra;
                    visualizarSolicitacoesDTO.IdCriacaoEscalaExtra = item.IdCriacaoEscalaExtra;
                    visualizarSolicitacoesDTO.IdFuncionario = item.IdFuncionario;
                    visualizarSolicitacoesDTO.NmFuncionario = funcionario.NmNome;
                    visualizarSolicitacoesDTO.NmEscalaExtra = escala.NmEscalaExtra;
                    visualizarSolicitacoesDTO.NmSetor = setor.NmNome;
                    visualizarSolicitacoesDTO.DtEscalaExtra = escala.DtEscalaExtra;
                    visualizarSolicitacoesDTO.DtCriacao = item.DtCriacao;
                    visualizarSolicitacoesDTO.StatusInscricao = item.StatusInscricao;

                    listVisualizarSolicitacoesDTO.Add(visualizarSolicitacoesDTO);
                }

                return listVisualizarSolicitacoesDTO.OrderBy(x => x.DtCriacao).ToList();
            }
            catch (Exception e)
            {
                // Lança uma exceção caso ocorra um erro
                throw new Exception($"Erro ao buscar todas as escalas extras: {e.Message}", e);
            }
        }

        public async Task<SolicitacaoEscalaExtraDTO> BuscarPorIdEscalaExtra(Guid idEscalaExtra)
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

                var escalaExtraEntiti = await _SolicitacaoEscalaExtraRepository.BuscarPorIdEscalaExtra(idEscalaExtra);

                // Busca o objeto de EscalaExtra no repositório
                var solicitacaoEscalaExtra = await _SolicitacaoEscalaExtraRepository.ObterListaPorIdFuncionario(escalaExtraEntiti.IdFuncionario);
                // Mapeia as entidades de EscalaExtra para DTO e retorna
                var escalaExtra = _mapper.Map<SolicitacaoEscalaExtraDTO>(escalaExtraEntiti);

                // Mapeia as entidades de EscalaExtra para a lista de DTOs e retorna
                var solicitacaoDeExtra = _mapper.Map<List<SolicitacaoEscalaExtraDTO>>(solicitacaoEscalaExtra);

                foreach (var item in solicitacaoDeExtra)
                {
                    var escalaExtra2 = await _escalaExtraRepository.BuscarListaPorIdAsync(item.IdCriacaoEscalaExtra);
                    var setor = await _setorRepository.BuscarPorIdAsync(escalaExtra2.IdSetor);
                    escalaExtra.NmEscalaExtra = escalaExtra2.NmEscalaExtra;
                    escalaExtra.NmSetor = setor.NmNome;
                    escalaExtra.DtEscalaExtra = escalaExtra2.DtEscalaExtra;
                }                                
                return escalaExtra;
            }
            catch (Exception e)
            {
                // Lança a exceção com a mensagem de erro
                throw new Exception($"Erro ao buscar escala extra: {e.Message}");
            }
        }

        public async Task<SolicitacaoEscalaExtraDTO> AlterarStatusExtra(Guid idEscalaExtra, string statusInscricao)
        {
            try
            {
                // Verifica se o Id fornecido é válido
                if (idEscalaExtra == Guid.Empty || statusInscricao == string.Empty)
                {
                    // Retorna um DTO de erro se o ID for inválido
                    return new SolicitacaoEscalaExtraDTO
                    {
                        valido = false,
                        mensagem = "Id fora do Range ou sem Status."
                    };
                }

                var escalaExtraEntiti = await _SolicitacaoEscalaExtraRepository.BuscarPorIdEscalaExtra(idEscalaExtra);

                // 2. Tentar converter a string para o enum StatusInscricaoEnum
                //    O 'true' ignora diferenças de maiúsculas/minúsculas (ex: "ausente" funciona)
                if (!Enum.TryParse<StatusInscricaoEnum>(statusInscricao, true, out StatusInscricaoEnum novoStatus))
                {
                    // Se a conversão falhar, o status fornecido é inválido.
                    return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = $"O status '{statusInscricao}' é inválido." };
                }


                //modificar o StatusInscricao para 
                if (escalaExtraEntiti != null)
                {
                    escalaExtraEntiti.StatusInscricao = novoStatus.ToString();
                    await _SolicitacaoEscalaExtraRepository.AlterarAsync(escalaExtraEntiti);
                }

                // Busca o objeto de EscalaExtra no repositório
                var solicitacaoEscalaExtra = await _SolicitacaoEscalaExtraRepository.ObterListaPorIdFuncionario(escalaExtraEntiti.IdFuncionario);

                // Mapeia as entidades de EscalaExtra para DTO e retorna
                var escalaExtra = _mapper.Map<SolicitacaoEscalaExtraDTO>(escalaExtraEntiti);

                // Mapeia as entidades de EscalaExtra para a lista de DTOs e retorna
                var solicitacaoDeExtra = _mapper.Map<List<SolicitacaoEscalaExtraDTO>>(solicitacaoEscalaExtra);



                foreach (var item in solicitacaoDeExtra)
                {
                    var escalaExtra2 = await _escalaExtraRepository.BuscarListaPorIdAsync(item.IdCriacaoEscalaExtra);
                    var setor = await _setorRepository.BuscarPorIdAsync(escalaExtra2.IdSetor);
                    escalaExtra.NmEscalaExtra = escalaExtra2.NmEscalaExtra;
                    escalaExtra.NmSetor = setor.NmNome;
                    escalaExtra.DtEscalaExtra = escalaExtra2.DtEscalaExtra;
                }
                return escalaExtra;
            }
            catch (Exception e)
            {
                // Lança a exceção com a mensagem de erro
                throw new Exception($"Erro ao buscar escala extra: {e.Message}");
            }
        }

        public async Task<SolicitacaoEscalaExtraDTO> CancelarInscricaoEPromoverFilaAsync(Guid idInscricaoCancelando)
        {
            // 1. BUSCAR A INSCRIÇÃO QUE ESTÁ SENDO CANCELADA
            var inscricaoCancelando = await _SolicitacaoEscalaExtraRepository.BuscarPorIdEscalaExtra(idInscricaoCancelando);
            if (inscricaoCancelando == null)
            {
                return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "Inscrição a ser cancelada não encontrada." };
            }

            var statusOriginal = inscricaoCancelando.StatusInscricao;
            if (statusOriginal == StatusInscricaoEnum.Cancelado.ToString())
            {
                return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "Esta inscrição já foi cancelada." };
            }

            inscricaoCancelando.StatusInscricao = StatusInscricaoEnum.Cancelado.ToString();

            // 2. LÓGICA DE PROMOÇÃO (SÓ SE QUEM CANCELOU ESTAVA CONFIRMADO)
            if (statusOriginal == StatusInscricaoEnum.Confirmado.ToString())
            {
                var escalaPrincipal = await _escalaExtraRepository.ObterPorIdAsync(inscricaoCancelando.IdCriacaoEscalaExtra);
                if (escalaPrincipal == null)
                {
                    return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "Escala principal associada não encontrada." };
                }

                var proximoDaFila = await _SolicitacaoEscalaExtraRepository.ObterProximoDaFilaAsync(inscricaoCancelando.IdCriacaoEscalaExtra);

                if (proximoDaFila != null)
                {
                    // PROMOVE O USUÁRIO DA FILA
                    proximoDaFila.StatusInscricao = StatusInscricaoEnum.Confirmado.ToString();
                    proximoDaFila.DtConfirmacao = DateTime.UtcNow;

                    // --- ENVIO DE E-MAIL (AGORA SEGURO) ---
                    try
                    {
                        var funcionarioPromovido = await _funcionarioRepository.ObterPorIdAsync(proximoDaFila.IdFuncionario);
                        if (funcionarioPromovido != null)
                        {
                            var setor = await _setorRepository.BuscarPorIdAsync(escalaPrincipal.IdSetor);
                            var dataServico = escalaPrincipal.DtEscalaExtra.ToString("dd/MM/yyyy");
                            var horaServico = escalaPrincipal.DtEscalaExtra.AddHours(-3).ToString("HH:mm");

                            string corpoEmail = $@"
                        <html><body>
                            <h2>Vaga de Extra Confirmada!</h2>
                            <p>Olá, {funcionarioPromovido.NmNome}.</p>
                            <p>Uma vaga foi liberada na escala '{escalaPrincipal.NmEscalaExtra}' e você foi promovido da fila de espera. Sua inscrição agora está <strong>Confirmada</strong>.</p>
                            <div class='details'>
                                <strong>Data:</strong> {dataServico}<br>
                                <strong>Hora:</strong> {horaServico}<br>
                                <strong>Setor:</strong> {setor.NmNome}<br>
                            </div>
                            <div class='signature'><p>Atenciosamente,<br>Defesa Civil de Maricá.</p></div>
                        </body></html>";

                            // ==========================================================
                            // CORREÇÃO AQUI:
                            // Para testar, passe a string diretamente, sem atribuir.
                            // ==========================================================
                            string emailDestino = "endrigo.valente@gmail.com"; // Para teste
                                                                               // string emailDestino = funcionarioPromovido.NmEmail; // Para produção

                            // Log para depuração do corpo do e-mail
                            _logger.LogInformation("Corpo do e-mail a ser enviado: {CorpoEmail}", corpoEmail);

                            await _emailService.EnviarEmail(emailDestino, "Vaga de Serviço Extra Confirmada", corpoEmail);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Falha ao enviar e-mail de confirmação para o promovido.");
                    }
                }
                else // Se não há ninguém na fila
                {
                    // Apenas incrementa a vaga
                    //escalaPrincipal.QtdVagas++;
                }
            }

            // 3. SALVA TODAS AS ALTERAÇÕES RASTREADAS PELO EF CORE
            await _unitOfWork.CompleteAsync();

            // 4. RETORNA O DTO DA INSCRIÇÃO ORIGINALMENTE CANCELADA
            return _mapper.Map<SolicitacaoEscalaExtraDTO>(inscricaoCancelando);
        }


    }
}