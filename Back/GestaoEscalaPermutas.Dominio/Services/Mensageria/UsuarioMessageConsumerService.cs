

using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.Interfaces.Mensageria;
using Microsoft.Extensions.Hosting;

namespace GestaoEscalaPermutas.Dominio.Services.Mensageria
{
    //public class UsuarioMessageConsumer : BackgroundService
    //{
    //    private readonly IMessageBus _messageBus;

    //    public UsuarioMessageConsumer(IMessageBus messageBus)
    //    {
    //        _messageBus = messageBus;
    //    }

    //    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        _messageBus.Subscribe<FuncionarioDTO>("usuarios.queue", ProcessarMensagem);
    //        return Task.CompletedTask;
    //    }

    //    private void ProcessarMensagem(FuncionarioDTO usuario)
    //    {
    //        // Processar mensagem (lógica do consumidor)
    //        Console.WriteLine($"Mensagem recebida: {usuario.NmNome}");
    //    }
    //}
}
