
using GestaoEscalaPermutas.Dominio.DTO.Permutas;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Dominio.Interfaces.Mensageria;
using Microsoft.Extensions.DependencyInjection; // ADICIONE ESTE USING!
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FirebaseAdmin.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Services.Mensageria
{
    public class PermutasMessageConsumer : BackgroundService
    {
        private readonly IMessageBus _messageBus;
        private readonly ILogger<PermutasMessageConsumer> _logger;
        // 1. REMOVEMOS o IFuncionarioService daqui...
        // private readonly IFuncionarioService _funcionarioService;

        // E ADICIONAMOS a fábrica de escopos.
        private readonly IServiceScopeFactory _scopeFactory;

        // 2. O CONSTRUTOR AGORA RECEBE A FÁBRICA DE ESCOPOS
        public PermutasMessageConsumer(
            IMessageBus messageBus,
            ILogger<PermutasMessageConsumer> logger,
            IServiceScopeFactory scopeFactory) // IFuncionarioService foi trocado por IServiceScopeFactory
        {
            _messageBus = messageBus;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Consumir mensagens de permutas solicitadas
            _messageBus.Subscribe<PermutaMensagemDTO>("permutas.solicitadas", async msg =>
            {
                // 3. PARA CADA MENSAGEM, CRIAMOS UM ESCOPO NOVO E SEGURO
                using (var scope = _scopeFactory.CreateScope())
                {
                    // 4. DENTRO DO ESCOPO, PEGAMOS OS SERVIÇOS QUE PRECISAMOS
                    var funcionarioService = scope.ServiceProvider.GetRequiredService<IFuncionarioService>();

                    _logger.LogInformation($"Permuta solicitada: {msg.NmNomeSolicitante} solicitou {msg.NmNomeSolicitado} para {msg.DtDataSolicitadaTroca}");

                    // Enviar notificação ao funcionário solicitado
                    try
                    {
                        // 5. USAMOS A VARIÁVEL LOCAL 'funcionarioService'
                        string fcmTokenSolicitado = await funcionarioService.GetFcmTokenAsync(msg.IdFuncionarioSolicitado);
                        if (!string.IsNullOrEmpty(fcmTokenSolicitado))
                        {
                            await SendFcmNotification(
                                fcmTokenSolicitado,
                                "Nova Solicitação de Permuta",
                                $"{msg.NmNomeSolicitante} solicitou uma permuta para {msg.DtDataSolicitadaTroca}");
                        }
                        else
                        {
                            _logger.LogWarning($"FCM Token não encontrado para o funcionário solicitado: {msg.IdFuncionarioSolicitado}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Erro ao enviar notificação ao solicitado: {ex.Message}");
                    }

                    // Enviar notificação aos administradores
                    try
                    {
                        var administradores = await funcionarioService.GetAdministradoresAsync();
                        foreach (var admin in administradores)
                        {
                            string fcmTokenAdmin = await funcionarioService.GetFcmTokenAsync(admin.IdFuncionario);
                            if (!string.IsNullOrEmpty(fcmTokenAdmin))
                            {
                                await SendFcmNotification(
                                    fcmTokenAdmin,
                                    "Nova Permuta Solicitada",
                                    $"{msg.NmNomeSolicitante} solicitou uma permuta com {msg.NmNomeSolicitado} para {msg.DtDataSolicitadaTroca}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Erro ao enviar notificação aos administradores: {ex.Message}");
                    }
                } // O escopo é descartado aqui, limpando os serviços temporários.
            });

            // O mesmo padrão é aplicado para as outras filas
            _messageBus.Subscribe<PermutaMensagemDTO>("permutas.pendentes", async msg =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var funcionarioService = scope.ServiceProvider.GetRequiredService<IFuncionarioService>();
                    _logger.LogInformation($"Permuta pendente de aprovação: {msg.IdPermuta}");
                    try
                    {
                        var administradores = await funcionarioService.GetAdministradoresAsync();
                        foreach (var admin in administradores)
                        {
                            string fcmTokenAdmin = await funcionarioService.GetFcmTokenAsync(admin.IdFuncionario);
                            if (!string.IsNullOrEmpty(fcmTokenAdmin))
                            {
                                await SendFcmNotification(fcmTokenAdmin, "Permuta Pendente", $"Permuta {msg.IdPermuta} aguardando aprovação.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Erro ao notificar administradores sobre permuta pendente: {ex.Message}");
                    }
                }
            });

            _messageBus.Subscribe<PermutaMensagemDTO>("permutas.resultado", msg =>
            {
                _logger.LogInformation($"Permuta {msg.IdPermuta} foi {msg.NmStatus} para {msg.NmNomeSolicitante} e {msg.NmNomeSolicitado}");
            });

            return Task.CompletedTask;
        }

        private async Task SendFcmNotification(string fcmToken, string title, string body)
        {
            try
            {
                if (string.IsNullOrEmpty(fcmToken))
                {
                    _logger.LogWarning("FCM Token não fornecido. Notificação não enviada.");
                    return;
                }

                var message = new Message
                {
                    Token = fcmToken,
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body,
                    },
                };

                string result = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                _logger.LogInformation($"Notificação FCM enviada com sucesso: {result}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao enviar notificação FCM: {ex.Message}");
            }
        }
    }
}