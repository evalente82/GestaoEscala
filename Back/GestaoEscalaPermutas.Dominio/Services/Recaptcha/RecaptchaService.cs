using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Services.Recaptcha
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using GestaoEscalaPermutas.Server.Settings;
    using Google.Api.Gax.ResourceNames;
    using Google.Cloud.RecaptchaEnterprise.V1;
    using Grpc.Core; // Para RpcException
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options; // Para injetar RecaptchaSettings

    namespace SeuNamespace.Services
    {
        // Supondo que SolicitacaoEscalaExtraDTO é a classe que representa o DTO recebido do frontend
        // Ela deve ter uma propriedade para o RecaptchaToken

        public class RecaptchaService
        {
            private readonly RecaptchaSettings _recaptchaSettings;
            private readonly ILogger<RecaptchaService> _logger;

            // O HttpClient não é mais necessário aqui, pois usaremos o cliente Google.Cloud.RecaptchaEnterprise.V1
            // private readonly HttpClient _httpClient; // Remova esta linha se não for mais usada

            public RecaptchaService(IOptions<RecaptchaSettings> recaptchaOptions, ILogger<RecaptchaService> logger)
            {
                _recaptchaSettings = recaptchaOptions.Value;
                _logger = logger;
                // _httpClient = httpClient; // Remova esta injeção se não for mais usada
            }

            public async Task<SolicitacaoEscalaExtraDTO> ValidarRecaptcha(SolicitacaoEscalaExtraDTO solicitacoesEscalaExtraDTOs)
            {
                _logger.LogInformation("Iniciando validação reCAPTCHA para token.");

                // 1. Verificação inicial do token
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

                    // 2. Verifique se o token é válido.
                    if (!response.TokenProperties.Valid)
                    {
                        string errorMessage = $"Validação reCAPTCHA falhou: Token inválido. Razões: {response.TokenProperties.InvalidReason}";
                        _logger.LogError(errorMessage);
                        return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = errorMessage };
                    }

                    // 3. Verifique se a ação esperada foi executada.
                    if (response.TokenProperties.Action != _recaptchaSettings.ExpectedAction)
                    {
                        string errorMessage = $"Validação reCAPTCHA falhou: Ação do token não corresponde. Esperado '{_recaptchaSettings.ExpectedAction}', Recebido '{response.TokenProperties.Action}'.";
                        _logger.LogError(errorMessage);
                        return new SolicitacaoEscalaExtraDTO { valido = false, mensagem = errorMessage };
                    }

                    // 4. Consulte a pontuação de risco.
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
                    return new SolicitacaoEscalaExtraDTO { valido = true, mensagem = "Validação reCAPTCHA bem-sucedida." };
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
            }
        }

        // Certifique-se de que SolicitacaoEscalaExtraDTO tenha uma propriedade para o status de validação
        public class SolicitacaoEscalaExtraDTO
        {
            public bool valido { get; set; }
            public string mensagem { get; set; }
            public string RecaptchaToken { get; set; }
            // ... outras propriedades
        }
    }
}
