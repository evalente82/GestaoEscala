using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.Interfaces.Escala;
using GestaoEscalaPermutas.Dominio.Interfaces.Permutas;
using GestaoEscalaPermutas.Dominio.Services.Escala;
using GestaoEscalaPermutas.Server.Models.Escala;
using GestaoEscalaPermutas.Server.Models;
using Microsoft.AspNetCore.Mvc;
using GestaoEscalaPermutas.Dominio.DTO.Permutas;
using GestaoEscalaPermutas.Server.Models.Permuta;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Server.Models.Funcionarios;
using GestaoEscalaPermutas.Dominio.Interfaces.Mensageria;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using FirebaseAdmin.Messaging;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using System.Security.Claims;

namespace GestaoEscalaPermutas.Server.Controllers.Permutas
{
    [ApiController]
    [Route("permutas")]
    public class PermutasController : ControllerBase
    {
        private readonly IPermutasService _permutasService;
        private readonly IMapper _mapper;
        private readonly IMessageBus _messageBus;
        private readonly IFuncionarioService _funcionarioService;
        public PermutasController(
         IPermutasService permutasService,
         IFuncionarioService funcionarioService,
         IMapper mapper,
         IMessageBus messageBus)
        {
            _permutasService = permutasService;
            _funcionarioService = funcionarioService;
            _mapper = mapper;
            _messageBus = messageBus;
        }

        [HttpPost]
        [Route("Incluir/")]
        public async Task<ActionResult> IncluirPermuta([FromBody] PermutasDTO escala)
        {
            var permutasDTO = await _permutasService.Incluir(_mapper.Map<PermutasDTO>(escala));
            var permutaModel = _mapper.Map<PermutaModel>(permutasDTO);

            if (permutaModel.Valido)
            {
                var mensagem = _mapper.Map<PermutaMensagemDTO>(permutasDTO);
                mensagem.NmStatus = "Solicitada";
                await _messageBus.PublishAsync("permutas.solicitadas", mensagem);

                // Enviar notificação ao funcionário solicitado
                try
                {
                    string fcmTokenSolicitado = await _funcionarioService.GetFcmTokenAsync(permutasDTO.IdFuncionarioSolicitado);
                    await SendFcmNotification(
                        fcmTokenSolicitado,
                        "Nova Permuta",
                        $"Permuta solicitada por {permutasDTO.NmNomeSolicitante} para {permutasDTO.DtDataSolicitadaTroca}"
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao obter FCM Token ou enviar notificação: {ex.Message}");
                }

                return Ok(permutaModel);
            }
            return BadRequest(new RetornoModel { Valido = false, Mensagem = permutaModel.Mensagem });
        }

        [HttpPatch]
        [Route("Atualizar/{id:Guid}")]
        public async Task<ActionResult> AtualizarPermuta(Guid id, [FromBody] PermutasDTO permuta)
        {
            permuta.IdPermuta = id;
            var permutaDTO = await _permutasService.Alterar(id, _mapper.Map<PermutasDTO>(permuta));
            var pemutaModel = _mapper.Map<PermutaModel>(permutaDTO);
            return (pemutaModel.Valido) ? Ok(pemutaModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = pemutaModel.Mensagem });
        }

        [HttpGet]
        [Route("buscarTodos")]
        public async Task<ActionResult> BuscarFuncionarios()
        {
            var permutas = await _permutasService.BuscarTodos();

            foreach (var permuta in permutas)
            {
                if (!permuta.valido)
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = permuta.mensagem });
                }
            }

            return Ok(permutas);
        }

        [HttpDelete]
        [Route("Deletar/{id:Guid}")]
        public async Task<ActionResult> DeletarFuncionario(Guid id)
        {
            var permutasDTO = await _permutasService.Deletar(id);
            var pemutasModel = _mapper.Map<PermutaModel>(permutasDTO);
            return (pemutasModel.Valido) ? Ok(pemutasModel.Mensagem) : BadRequest(new RetornoModel { Valido = false, Mensagem = pemutasModel.Mensagem });
        }

        [HttpGet]
        [Route("buscarPorId/{id:Guid}")]
        public async Task<ActionResult<PermutasDTO>> BuscarPorIdEscalas(Guid id)
        {
            var pemutas = await _permutasService.BuscarPorId(id);
            if (!pemutas.valido)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = pemutas.mensagem });
            }
            return Ok(pemutas);
        }

        [HttpGet]
        [Route("PermutaFuncionarioPorId/{idFuncionario:Guid}")]
        public async Task<ActionResult<List<PermutasDTO>>> BuscarPermFuncPorId(Guid idFuncionario)
        {
            var permutas = await _permutasService.BuscarSolicitacoesPorId(idFuncionario);

            if (permutas == null) // Verifica se a lista está vazia
            {
                return NotFound(new RetornoModel { Valido = false, Mensagem = "Nenhuma permuta encontrada para este funcionário." });
            }

            return Ok(permutas);
        }

        [HttpPut]
        [Route("AprovarSolicitado/{idPermuta:Guid}")]
        public async Task<ActionResult> AprovarSolicitado(Guid idPermuta)
        {
            var permuta = await _permutasService.BuscarPorId(idPermuta);
            if (!permuta.valido)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = permuta.mensagem });
            }

            permuta.NmStatus = "AprovadaSolicitado";
            var permutaAtualizada = await _permutasService.Alterar(idPermuta, permuta);

            if (permutaAtualizada.valido)
            {
                var mensagem = _mapper.Map<PermutaMensagemDTO>(permutaAtualizada);
                mensagem.NmStatus = "AprovadaSolicitado";
                await _messageBus.PublishAsync("permutas.pendentes", mensagem);

                // Tentar enviar notificação, mas continuar mesmo se falhar
                try
                {
                    string fcmTokenSolicitante = await _funcionarioService.GetFcmTokenAsync(permutaAtualizada.IdFuncionarioSolicitante);
                    if (!string.IsNullOrEmpty(fcmTokenSolicitante))
                    {
                        await SendFcmNotification(
                            fcmTokenSolicitante,
                            "Permuta Atualizada",
                            "Sua permuta foi aprovada pelo solicitado."
                        );
                        Console.WriteLine($"Notificação enviada para {permutaAtualizada.IdFuncionarioSolicitante}");
                    }
                    else
                    {
                        Console.WriteLine($"Nenhum FCM Token encontrado para o funcionário {permutaAtualizada.IdFuncionarioSolicitante}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao obter FCM Token ou enviar notificação: {ex.Message}");
                }

                return Ok(_mapper.Map<PermutaModel>(permutaAtualizada));
            }
            return BadRequest(new RetornoModel { Valido = false, Mensagem = permutaAtualizada.mensagem });
        }

        [HttpPut]
        [Route("RecusarSolicitado/{idPermuta:Guid}")]
        public async Task<ActionResult> RecusarSolicitado(Guid idPermuta)
        {
            var permuta = await _permutasService.BuscarPorId(idPermuta);
            if (!permuta.valido)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = permuta.mensagem });
            }

            permuta.NmStatus = "RecusadaSolicitado";
            //permuta.DtReprovacao = DateTime.Now;
            var permutaAtualizada = await _permutasService.Alterar(idPermuta, permuta);

            if (permutaAtualizada.valido)
            {
                var mensagem = _mapper.Map<PermutaMensagemDTO>(permutaAtualizada);
                mensagem.NmStatus = "Recusada";
                await _messageBus.PublishAsync("permutas.resultado", mensagem);

                try
                {
                    string fcmTokenSolicitante = await _funcionarioService.GetFcmTokenAsync(permutaAtualizada.IdFuncionarioSolicitante);
                    await SendFcmNotification(
                        fcmTokenSolicitante,
                        "Permuta Recusada",
                        "Sua permuta foi recusada pelo solicitado."
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao obter FCM Token ou enviar notificação: {ex.Message}");
                }

                return Ok(_mapper.Map<PermutaModel>(permutaAtualizada));
            }
            return BadRequest(new RetornoModel { Valido = false, Mensagem = permutaAtualizada.mensagem });
        }

        [HttpPut]
        [Route("Aprovar/{idPermuta:Guid}")]
        public async Task<ActionResult> AprovarPermuta(Guid idPermuta)
        {
            var permuta = await _permutasService.BuscarPorId(idPermuta);
            if (!permuta.valido)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = permuta.mensagem });
            }

            //permuta.IdFuncionarioAprovador = User.Identity.Name;
            permuta.NmNomeAprovador = User.Identity.Name;
            permuta.DtAprovacao = DateTime.UtcNow;
            permuta.NmStatus = "Aprovada";
            Claim idFunc = User.Claims.FirstOrDefault(x => x.Type.Contains("IdFuncionario"));

            permuta.IdFuncionarioAprovador = idFunc != null ? Guid.Parse(idFunc.Value) : null;
            var permutaAtualizada = await _permutasService.Alterar(idPermuta, permuta);

            if (permutaAtualizada.valido)
            {
                var mensagem = _mapper.Map<PermutaMensagemDTO>(permutaAtualizada);
                mensagem.NmStatus = "Aprovada";
                await _messageBus.PublishAsync("permutas.resultado", mensagem);

                try
                {
                    string fcmTokenSolicitante = await _funcionarioService.GetFcmTokenAsync(permutaAtualizada.IdFuncionarioSolicitante);
                    string fcmTokenSolicitado = await _funcionarioService.GetFcmTokenAsync(permutaAtualizada.IdFuncionarioSolicitado);
                    await SendFcmNotification(fcmTokenSolicitante, "Permuta Aprovada", "A permuta foi aprovada pela chefia.");
                    await SendFcmNotification(fcmTokenSolicitado, "Permuta Aprovada", "A permuta foi aprovada pela chefia.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao obter FCM Tokens ou enviar notificações: {ex.Message}");
                }

                return Ok(_mapper.Map<PermutaModel>(permutaAtualizada));
            }
            return BadRequest(new RetornoModel { Valido = false, Mensagem = permutaAtualizada.mensagem });
        }

        [HttpPut]
        [Route("Recusar/{idPermuta:Guid}")]
        public async Task<ActionResult> RecusarPermuta(Guid idPermuta)
        {
            var permuta = await _permutasService.BuscarPorId(idPermuta);
            if (!permuta.valido)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = permuta.mensagem });
            }

            permuta.NmNomeAprovador = User.Identity.Name;
            permuta.DtReprovacao = DateTime.UtcNow;
            permuta.NmStatus = "Recusada";
            var permutaAtualizada = await _permutasService.Alterar(idPermuta, permuta);

            if (permutaAtualizada.valido)
            {
                var mensagem = _mapper.Map<PermutaMensagemDTO>(permutaAtualizada);
                mensagem.NmStatus = "Recusada";
                await _messageBus.PublishAsync("permutas.resultado", mensagem);

                try
                {
                    string fcmTokenSolicitante = await _funcionarioService.GetFcmTokenAsync(permutaAtualizada.IdFuncionarioSolicitante);
                    string fcmTokenSolicitado = await _funcionarioService.GetFcmTokenAsync(permutaAtualizada.IdFuncionarioSolicitado);
                    await SendFcmNotification(fcmTokenSolicitante, "Permuta Recusada", "A permuta foi recusada pela chefia.");
                    await SendFcmNotification(fcmTokenSolicitado, "Permuta Recusada", "A permuta foi recusada pela chefia.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao obter FCM Tokens ou enviar notificações: {ex.Message}");
                }

                return Ok(_mapper.Map<PermutaModel>(permutaAtualizada));
            }
            return BadRequest(new RetornoModel { Valido = false, Mensagem = permutaAtualizada.mensagem });
        }

        private async Task SendFcmNotification(string fcmToken, string title, string body)
        {
            try
            {
                if (string.IsNullOrEmpty(fcmToken))
                {
                    Console.WriteLine("FCM Token não fornecido. Notificação não enviada.");
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
                Console.WriteLine($"Notificação FCM enviada com sucesso: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar notificação FCM: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("SolicitacoesPorId/{idFuncionario:Guid}")]
        public async Task<ActionResult<List<PermutasDTO>>> BuscarSolicitacoesFuncPorId(Guid idFuncionario)
        {
            //SolicitacoesPorId
            var permutas = await _permutasService.BuscarSolicitacoesFuncPorId(idFuncionario);

            if (permutas == null) // Verifica se a lista está vazia
            {
                return NotFound(new RetornoModel { Valido = false, Mensagem = "Nenhuma permuta encontrada para este funcionário." });
            }

            return Ok(permutas);
        }


        [HttpGet]
        [Route("ContarPendentes/{idFuncionario:Guid}")]
        public async Task<ActionResult<int>> ContarPermutasPendentes(Guid idFuncionario)
        {
            try
            {
                var permutasSolicitadas = await _permutasService.BuscarSolicitacoesPorId(idFuncionario);
                var permutasSolicitantes = await _permutasService.BuscarSolicitacoesFuncPorId(idFuncionario);

                // Contar permutas pendentes (status "Solicitada" ou "AprovadaSolicitado")
                int count = permutasSolicitantes
                    .Count(p => p.valido && (p.NmStatus == null));

                return Ok(count);
            }
            catch (Exception ex)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = $"Erro ao contar permutas pendentes: {ex.Message}" });
            }
        }
    }
}
