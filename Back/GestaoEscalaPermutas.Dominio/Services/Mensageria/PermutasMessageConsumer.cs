using GestaoEscalaPermutas.Dominio.DTO.Permutas;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Dominio.Interfaces.Mensageria;
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
        private readonly IFuncionarioService _funcionarioService;

        public PermutasMessageConsumer(
            IMessageBus messageBus,
            ILogger<PermutasMessageConsumer> logger,
            IFuncionarioService funcionarioService)
        {
            _messageBus = messageBus;
            _logger = logger;
            _funcionarioService = funcionarioService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Verificar se a mensageria está disponível
            if (_messageBus == null)
            {
                _logger.LogInformation("Mensageria não disponível. Pulando consumo de mensagens para Permutas.");
                await Task.Delay(Timeout.Infinite, stoppingToken); // Mantém o serviço ativo até ser cancelado
                return;
            }

            try
            {
                // Consumir mensagens de permutas solicitadas
                _messageBus.Subscribe<PermutaMensagemDTO>("permutas.solicitadas", async msg =>
                {
                    _logger.LogInformation($"Permuta solicitada: {msg.NmNomeSolicitante} solicitou {msg.NmNomeSolicitado} para {msg.DtDataSolicitadaTroca}");

                    // Enviar notificação ao funcionário solicitado
                    try
                    {
                        string fcmTokenSolicitado = await _funcionarioService.GetFcmTokenAsync(msg.IdFuncionarioSolicitado);
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
                        var administradores = await _funcionarioService.GetAdministradoresAsync();
                        foreach (var admin in administradores)
                        {
                            string fcmTokenAdmin = await _funcionarioService.GetFcmTokenAsync(admin.IdFuncionario);
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
                });

                // Consumir mensagens de permutas pendentes
                _messageBus.Subscribe<PermutaMensagemDTO>("permutas.pendentes", async msg =>
                {
                    _logger.LogInformation($"Permuta pendente de aprovação: {msg.IdPermuta}");

                    // Notificar administradores sobre permuta pendente
                    try
                    {
                        var administradores = await _funcionarioService.GetAdministradoresAsync();
                        foreach (var admin in administradores)
                        {
                            string fcmTokenAdmin = await _funcionarioService.GetFcmTokenAsync(admin.IdFuncionario);
                            if (!string.IsNullOrEmpty(fcmTokenAdmin))
                            {
                                await SendFcmNotification(
                                    fcmTokenAdmin,
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
                    _logger.LogInformation($"Permuta {msg.IdPermuta} foi {msg.NmStatus} para {msg.NmNomeSolicitante} e {msg.NmNomeSolicitado}");
                });

                _logger.LogInformation("Consumidores de permutas configurados com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao configurar consumidores de permutas: {ex.Message}");
            }

            // Mantém o serviço ativo até ser cancelado
            await Task.Delay(Timeout.Infinite, stoppingToken);
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