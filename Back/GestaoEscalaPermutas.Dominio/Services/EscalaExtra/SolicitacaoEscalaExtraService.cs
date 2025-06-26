using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.EscalaExtra;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra;
using GestaoEscalaPermutas.Repository.Interfaces;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Server.Settings;
using Microsoft.Extensions.Options;
using System.Text.Json;

// --- NOVOS USINGS PARA reCAPTCHA ENTERPRISE ---
using Google.Api.Gax.ResourceNames;
using Google.Cloud.RecaptchaEnterprise.V1;
using Grpc.Core; // Para RpcException
using Microsoft.Extensions.Logging;
using GestaoEscalaPermutas.Dominio.Interfaces.Email; // Para logs

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
            IUnitOfWork unitOfWork
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

            _logger.LogInformation("Iniciando validação reCAPTCHA para token.");

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

            // Mapeia a lista de DTOs para as entidades (EscalaExtra)
            var solicitacaoEscalaExtra = _mapper.Map<DepInfra.EscalaExtra>(solicitacoesEscalaExtraDTOs);

            //verificar a Qtd de vagas disponiveis
            var extrasDisponiveis = await _escalaExtraRepository.BuscarListaPorIdAsync(solicitacaoEscalaExtra.IdCriacaoEscalaExtra);

            if (extrasDisponiveis.QtdVagas == 0)
            {
                return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "Sem Vagas disponíveis." };
            }

            var escalasProntas = await _escalaProntaRepository.BuscarPorIdFuncionario(solicitacoesEscalaExtraDTOs.IdFuncionario);
            var listEscalas = await _SolicitacaoEscalaExtraRepository.ObterTodosAsync();
            var funcionario = await _funcionarioRepository.ObterPorIdAsync(solicitacoesEscalaExtraDTOs.IdFuncionario);

            if (!funcionario.IsAtivo)
            {
                return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "Funcionário Inativo." };
            }

            //verificar se o funcionario esta de serviço no referido dia.
            foreach (var escala in escalasProntas)
            {
                if (extrasDisponiveis.DtEscalaExtra.Date == escala.DtDataServico.Date)
                {
                    return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "O Funcionário está de plantão nesta dia." };
                }
            }

            //verificar se o funcionario ja se cadastrou no dia e não pode cadastrar em outro setor no mesmo dia.
            foreach (var item in listEscalas)
            {
                var listaEscala = await _escalaExtraRepository.BuscarListaPorIdAsync(item.IdCriacaoEscalaExtra);

                if (funcionario.IdFuncionario == item.IdFuncionario)
                {
                    if (listaEscala.DtEscalaExtra.Date == extrasDisponiveis.DtEscalaExtra.Date)
                    {
                        return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = "O Funcionário já possui cadastro de escala extra nesta data." };
                    }
                }
                                
            }

            // Adiciona a lista de escalas ao repositório
            var novaSolicitacaoEscalaExtra = await _SolicitacaoEscalaExtraRepository.AdicionarListaAsync(solicitacaoEscalaExtra);

            extrasDisponiveis.QtdVagas -- ;
            var alteraQtdEscalaDisponiivel = _escalaExtraRepository.AlterarAsync(extrasDisponiveis);
            await _unitOfWork.CompleteAsync();

            //enviar e-mail
            try
            {
                var dataServico = extrasDisponiveis.DtEscalaExtra.ToString("dd/MM/yyyy");
                var horaServico = extrasDisponiveis.DtEscalaExtra.AddHours(-3).ToString("HH:mm");
                var setor = await _setorRepository.BuscarPorIdAsync(extrasDisponiveis.IdSetor);
                var setorServico = setor.NmNome;

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
                    listVisualizarSolicitacoesDTO.Add(visualizarSolicitacoesDTO);
                }




                // Mapeia todas as EscalasExtra para a lista de DTOs
                return listVisualizarSolicitacoesDTO;
            }
            catch (Exception e)
            {
                // Lança uma exceção caso ocorra um erro
                throw new Exception($"Erro ao buscar todas as escalas extras: {e.Message}", e);
            }
        }
    }
}