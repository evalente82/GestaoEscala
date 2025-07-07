using GestaoEscalaPermutas.Dominio.DTO.Permutas;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Dominio.Interfaces.Mensageria;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.DependencyInjection; // <<< PASSO 1: Adicione este using

namespace GestaoEscalaPermutas.Dominio.Services.Mensageria
{
    public class PermutasMessageConsumer : BackgroundService
    {
        private readonly IMessageBus _messageBus;
        private readonly ILogger<PermutasMessageConsumer> _logger;
        // private readonly IFuncionarioService _funcionarioService; // <<< PASSO 2: Remova o serviço Scoped daqui

        private readonly IServiceScopeFactory _scopeFactory; // <<< PASSO 3: Adicione a IServiceScopeFactory

        public PermutasMessageConsumer(
            IMessageBus messageBus,
            ILogger<PermutasMessageConsumer> logger,
            // IFuncionarioService funcionarioService, // <<< PASSO 4: Remova o serviço Scoped do construtor
            IServiceScopeFactory scopeFactory)
        {
            _messageBus = messageBus;
            _logger = logger;
            _scopeFactory = scopeFactory; // <<< PASSO 5: Atribua a factory
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Consumir mensagens de permutas solicitadas
            _messageBus.Subscribe<PermutaMensagemDTO>("permutas.solicitadas", async msg =>
            {
                // PASSO 6: Crie um escopo para esta mensagem específica
                using var scope = _scopeFactory.CreateScope();
                // PASSO 7: Resolva o serviço Scoped DENTRO do escopo
                var funcionarioService = scope.ServiceProvider.GetRequiredService<IFuncionarioService>();

                _logger.LogInformation($"Permuta solicitada: {msg.NmNomeSolicitante} solicitou {msg.NmNomeSolicitado} para {msg.DtDataSolicitadaTroca}");

                // Enviar notificação ao funcionário solicitado
                try
                {
                    string fcmTokenSolicitado = await funcionarioService.GetFcmTokenAsync(msg.IdFuncionarioSolicitado);
                    if (!string.IsNullOrEmpty(fcmTokenSolicitado))
                    {
                        await SendFcmNotification(fcmTokenSolicitado,
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
                            await SendFcmNotification(fcmTokenAdmin,
                                "Nova Permuta Solicitada",
                                $"{msg.NmNomeSolicitante} solicitou uma permuta com {msg.NmNomeSolicitado} para {msg.DtDataSolicitadaTroca}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao enviar notificação aos administradores: {ex.Message}");
                }
            });

            // Consumir mensagens de permutas pendentes
            _messageBus.Subscribe<PermutaMensagemDTO>("permutas.pendentes", async msg =>
            {
                // Crie um novo escopo para esta outra mensagem
                using var scope = _scopeFactory.CreateScope();
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
                            await SendFcmNotification(fcmTokenAdmin,
                                "Permuta Pendente",
                                $"Permuta {msg.IdPermuta} aguardando aprovação.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao notificar administradores sobre permuta pendente: {ex.Message}");
                }
            });

            // Consumir mensagens de resultado
            _messageBus.Subscribe<PermutaMensagemDTO>("permutas.resultado", msg =>
            {
                // Para este callback, que é síncrono e só faz log, não é necessário
                // criar um escopo, pois não usa serviços Scoped.
                _logger.LogInformation($"Permuta {msg.IdPermuta} foi {msg.NmStatus} para {msg.NmNomeSolicitante} e {msg.NmNomeSolicitado}");
            });

            return Task.CompletedTask;
        }

        // Este método não precisa de alterações, pois não usa DI
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