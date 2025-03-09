using AutoMapper;

using GestaoEscalaPermutas.Server.Models;
using Microsoft.AspNetCore.Mvc;
using GestaoEscalaPermutas.Dominio.Interfaces.Login;
using GestaoEscalaPermutas.Dominio.DTO.Login;
using GestaoEscalaPermutas.Server.Models.Login;
using Microsoft.AspNetCore.Authorization;
using GestaoEscalaPermutas.Server.Helper;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;

namespace GestaoEscalaPermutas.Server.Controllers.Login
{

    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IMapper _mapper;
        private readonly IFuncionarioService _funcionarioService;

        public LoginController(ILoginService loginService, IMapper mapper, IFuncionarioService funcionarioService)
        {
            _loginService = loginService;
            _mapper = mapper;
            _funcionarioService = funcionarioService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Incluir/")]
        public async Task<ActionResult> IncluirLogin([FromBody] LoginDTO funcionario)
        {
            if (string.IsNullOrEmpty(funcionario.Usuario) || string.IsNullOrEmpty(funcionario.Senha))
            {
                return BadRequest(new { Mensagem = "Usuário e senha são obrigatórios." });
            }

            var loginDTO = await _loginService.Incluir(_mapper.Map<LoginDTO>(funcionario));

            var loginModel = _mapper.Map<LoginModel>(loginDTO);

            return (loginModel.Valido) ? Ok(loginModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = loginModel.Mensagem });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("autenticar")]
        public async Task<ActionResult> Autenticar([FromBody] LoginRequestDTO loginRequest)
        {
            var loginResponse = await _loginService.Autenticar(loginRequest);

            return loginResponse.Valido
                ? Ok(loginResponse) // Retorna token e refreshToken
                : BadRequest(loginResponse);
        }

        [AllowAnonymous]
        [HttpPost("esqueci-senha")]
        public async Task<IActionResult> EsqueciSenha([FromBody] EsqueciSenhaRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return BadRequest(new { mensagem = "E-mail é obrigatório." });

            var resultado = await _loginService.GerarTokenRedefinicaoSenha(request.Email);

            return resultado.Valido ? Ok(new { mensagem = resultado.Mensagem }) : BadRequest(new { mensagem = resultado.Mensagem });
        }

        [AllowAnonymous]
        [HttpPost("redefinir-senha")]
        public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaRequestDTO request)
        {
            var resultado = await _loginService.RedefinirSenha(request);

            return resultado.Valido ? Ok(new { mensagem = resultado.Mensagem }) : BadRequest(new { mensagem = resultado.Mensagem });
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequestDTO request)
        {
            var response = await _loginService.RefreshToken(request.RefreshToken);
            return response.Valido ? Ok(response) : BadRequest(response);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("updateFcmToken")]
        public async Task<IActionResult> UpdateFcmToken([FromBody] UpdateFcmTokenRequestDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.IdFuncionario) || string.IsNullOrEmpty(request.FcmToken))
                    return BadRequest(new { Mensagem = "IdFuncionario ou FcmToken inválidos" });

                Guid idFuncionario = Guid.Parse(request.IdFuncionario); // Converter string para Guid
                await _funcionarioService.SaveFcmTokenAsync(idFuncionario, request.FcmToken);
                return Ok(new { Mensagem = "FCM Token atualizado com sucesso" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar FCM Token: {ex.Message}");
                return BadRequest(new { Mensagem = $"Erro ao atualizar FCM Token: {ex.Message}" });
            }
        }
    }
}
