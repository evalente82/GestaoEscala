using GestaoEscalaPermutas.Dominio.DTO.Permutas;
using GestaoEscalaPermutas.Dominio.Interfaces.Mensageria;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Services.Mensageria
{
    public class PermutasMessageConsumer : BackgroundService
    {
        private readonly IMessageBus _messageBus;
        private readonly ILogger<PermutasMessageConsumer> _logger;

        public PermutasMessageConsumer(IMessageBus messageBus, ILogger<PermutasMessageConsumer> logger)
        {
            _messageBus = messageBus;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Consumir mensagens de permutas solicitadas (pode ser usado para logs ou outras ações)
            _messageBus.Subscribe<PermutaMensagemDTO>("permutas.solicitadas", msg =>
            {
                _logger.LogInformation($"Permuta solicitada: {msg.NmNomeSolicitante} solicitou {msg.NmNomeSolicitado} para {msg.DtDataSolicitadaTroca}");
            });

            // Consumir mensagens de permutas pendentes (pode notificar administrador)
            _messageBus.Subscribe<PermutaMensagemDTO>("permutas.pendentes", msg =>
            {
                _logger.LogInformation($"Permuta pendente de aprovação: {msg.IdPermuta}");
            });

            // Consumir mensagens de resultado (pode ser usado para logs ou outras ações)
            _messageBus.Subscribe<PermutaMensagemDTO>("permutas.resultado", msg =>
            {
                _logger.LogInformation($"Permuta {msg.IdPermuta} foi {msg.NmStatus} para {msg.NmNomeSolicitante} e {msg.NmNomeSolicitado}");
            });

            return Task.CompletedTask;
        }
    }
}
