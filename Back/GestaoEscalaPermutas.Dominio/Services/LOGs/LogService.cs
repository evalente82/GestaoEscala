using GestaoEscalaPermutas.Dominio.Interfaces.LOGs;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using System.Text.Json;

namespace GestaoEscalaPermutas.Dominio.Services.LOGs
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;

        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task RegistrarAsync<T>(string acao, string entidade, string entidadeId, T? valorAntigo, T? valorNovo, string usuarioResponsavel)
        {
            var logEntry = new LogGestaoEscala
            {
                UsuarioResponsavel = usuarioResponsavel,
                TipoAcao = acao,
                EntidadeAfetada = entidade,
                IdEntidadeAfetada = entidadeId,
                ValoresAntigos = valorAntigo != null ? JsonSerializer.Serialize(valorAntigo) : null,
                ValoresNovos = valorNovo != null ? JsonSerializer.Serialize(valorNovo) : null,
                DataOcorrencia = DateTimeOffset.UtcNow
            };

            await _logRepository.AdicionarAsync(logEntry);
        }
    }
}
