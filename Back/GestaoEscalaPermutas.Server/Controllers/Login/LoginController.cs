using AutoMapper;

using GestaoEscalaPermutas.Server.Models;
using Microsoft.AspNetCore.Mvc;
using GestaoEscalaPermutas.Dominio.Interfaces.Login;
using GestaoEscalaPermutas.Dominio.DTO.Login;
using GestaoEscalaPermutas.Server.Models.Login;

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

        [HttpPost]
        [Route("autenticar")]
        public async Task<ActionResult> Autenticar([FromBody] LoginRequestDTO loginRequest)
        {
            var loginResponse = await _loginService.Autenticar(loginRequest);

            return loginResponse.Valido
                ? Ok(loginResponse)
                : BadRequest(loginResponse);
        }
    }
}
