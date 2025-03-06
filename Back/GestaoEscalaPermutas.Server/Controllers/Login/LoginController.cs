using AutoMapper;

using GestaoEscalaPermutas.Server.Models;
using Microsoft.AspNetCore.Mvc;
using GestaoEscalaPermutas.Dominio.Interfaces.Login;
using GestaoEscalaPermutas.Dominio.DTO.Login;
using GestaoEscalaPermutas.Server.Models.Login;
using Microsoft.AspNetCore.Authorization;
using GestaoEscalaPermutas.Server.Helper;

namespace GestaoEscalaPermutas.Server.Controllers.Login
{

    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IMapper _mapper;

        public LoginController(ILoginService loginService, IMapper mapper)
        {
            _loginService = loginService;
            _mapper = mapper;
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
        [HttpPost("updateFcmToken")]
        public async Task<IActionResult> UpdateFcmToken([FromBody] UpdateFcmTokenRequestDTO request)
        {
            var resultado = await _loginService.UpdateFcmToken(request);
            return resultado.Valido ? Ok(new { Mensagem = resultado.Mensagem }) : BadRequest(new { Mensagem = resultado.Mensagem });
        }
    }
}
