using AutoMapper;
using BCrypt.Net;
using GestaoEscalaPermutas.Dominio.DTO.Login;
using GestaoEscalaPermutas.Dominio.Interfaces.Email;
using GestaoEscalaPermutas.Dominio.Interfaces.Login;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;


namespace GestaoEscalaPermutas.Dominio.Services.Login
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public LoginService(ILoginRepository loginRepository, IMapper mapper, IEmailService emailService)
        {
            _loginRepository = loginRepository;
            _mapper = mapper;
            _emailService = emailService;
        }

        /// <summary>
        /// Autentica um usuário e gera um token com as permissões.
        /// </summary>
        public async Task<LoginResponseDTO> Autenticar(LoginRequestDTO loginRequest)
        {
            try
            {
                var usuario = await _loginRepository.ObterUsuarioComPerfilEPermissoesAsync(loginRequest.Usuario);

                if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Senha, usuario.SenhaHash))
                {
                    return new LoginResponseDTO { Valido = false, Mensagem = "Usuário ou senha inválidos." };
                }

                var funcionario = await _loginRepository.ObterFuncionarioComCargoEPermissoesAsync(usuario.Email);

                if (funcionario == null)
                {
                    return new LoginResponseDTO { Valido = false, Mensagem = "Funcionário não encontrado." };
                }

                var perfil = funcionario.Cargo?.CargoPerfis.FirstOrDefault()?.Perfil.Nome ?? "Sem Perfil";

                var permissoes = funcionario.Cargo?.CargoPerfis
                    .SelectMany(cp => cp.Perfil.PerfisFuncionalidades)
                    .Select(pf => pf.Funcionalidade.Nome)
                    .Distinct()
                    .ToList() ?? new List<string>();

                var token = GerarTokenJWT(usuario, funcionario, permissoes);

                return new LoginResponseDTO
                {
                    Valido = true,
                    Mensagem = "Autenticado com sucesso.",
                    Token = token,
                    NomeUsuario = usuario.Nome ?? string.Empty,
                    Matricula = funcionario.NrMatricula,
                    IdFuncionario = funcionario.IdFuncionario,
                    Permissoes = permissoes
                };
            }
            catch (Exception e)
            {
                return new LoginResponseDTO { Valido = false, Mensagem = $"Erro ao autenticar: {e.Message}" };
            }
        }

        /// <summary>
        /// Gera um token JWT com permissões.
        /// </summary>
        private string GerarTokenJWT(DepInfra.Usuarios usuario, DepInfra.Funcionario funcionario, List<string> permissoes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var chaveSecreta = "g9h0N7quw2S8mJAF8LKxUF0Os3leG+NDJoypOcWohOEa"; // 🔑 Deve ser idêntica à do `Program.cs`
            var key = Encoding.UTF8.GetBytes(chaveSecreta); // 🔹 UTF-8 corretamente aplicado

            var utcNow = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.SerialNumber, funcionario.NrMatricula.ToString()),
                new Claim("IdFuncionario", usuario.IdFuncionario.ToString() ?? "SemIdFuncionario"),
                new Claim(ClaimTypes.Role, usuario.Perfil?.Nome ?? "SemPerfil"),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(utcNow).ToUnixTimeSeconds().ToString()), // Not Before
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(utcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64) // Issued At
            };

            claims.AddRange(permissoes.Select(p => new Claim("Permissao", p)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = utcNow.AddHours(8), // 🔹 Expiração correta
                Issuer = "gestao-escala-backend",
                Audience = "gestao-escala-frontend",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        /// <summary>
        /// Cria um novo usuário no sistema.
        /// </summary>
        public async Task<LoginResponseDTO> Incluir(LoginDTO loginDTO)
        {
            try
            {
                if (await _loginRepository.UsuarioExisteAsync(loginDTO.Usuario))
                {
                    return new LoginResponseDTO { Valido = false, Mensagem = "Usuário já cadastrado." };
                }

                var funcionario = await _loginRepository.ObterFuncionarioPorEmailAsync(loginDTO.Usuario);
                var idPerfis = funcionario.Cargo.CargoPerfis
                        .Where(x => x.IdPerfil != null)
                        .Select(x => x.IdPerfil)
                        .ToList();
                if (funcionario == null)
                {
                    return new LoginResponseDTO { Valido = false, Mensagem = "Funcionário não encontrado para este e-mail." };
                }

                loginDTO.SenhaHash = !string.IsNullOrEmpty(loginDTO.Senha)
                    ? BCrypt.Net.BCrypt.HashPassword(loginDTO.Senha, workFactor: 12)
                    : throw new Exception("Senha não pode ser nula.");

                var usuario = _mapper.Map<DepInfra.Usuarios>(loginDTO);
                usuario.Nome = funcionario.NmNome;
                usuario.IdUsuario = Guid.NewGuid();
                usuario.IdFuncionario = funcionario.IdFuncionario;
                usuario.Email = funcionario.NmEmail;
                usuario.Ativo = funcionario.IsAtivo;
                usuario.IdPerfil = idPerfis.FirstOrDefault();


                await _loginRepository.CriarUsuarioAsync(usuario);

                var permissoes = new List<string> { usuario.Perfil?.Nome ?? "SemPerfil" };
                var token = GerarTokenJWT(usuario, funcionario, permissoes);

                return new LoginResponseDTO
                {
                    Valido = true,
                    Mensagem = "Usuário cadastrado com sucesso.",
                    Token = token,
                    NomeUsuario = usuario.Nome
                };
            }
            catch (Exception e)
            {
                return new LoginResponseDTO { Valido = false, Mensagem = $"Erro ao incluir usuário: {e.Message}" };
            }
        }

        /// <summary>
        /// Gera um Token para redefinir a senha.
        /// </summary>
        public async Task<LoginResponseDTO> GerarTokenRedefinicaoSenha(string email)
        {
            try
            {
                var usuario = await _loginRepository.ObterUsuarioPorEmailAsync(email);

                if (usuario == null)
                    return new LoginResponseDTO { Valido = false, Mensagem = "E-mail não encontrado." };

                var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                usuario.TokenRecuperacaoSenha = token;
                usuario.TokenExpiracao = DateTime.UtcNow.AddHours(1);

                await _loginRepository.AtualizarUsuarioAsync(usuario);

                var linkRedefinicao = $"http://localhost:5173/RedefinirSenha?token={token}";

                await _emailService.EnviarEmail(usuario.Email, "Recuperação de Senha",
                    $"Clique no link para redefinir sua senha: <a href='{linkRedefinicao}'>Redefinir Senha</a>");

                return new LoginResponseDTO { Valido = true, Mensagem = "E-mail enviado com sucesso." };
            }
            catch (Exception e)
            {
                return new LoginResponseDTO { Valido = false, Mensagem = $"Erro ao gerar link de redefinição: {e.Message}" };
            }
        }

        /// <summary>
        /// Redefine a senha do usuário.
        /// </summary>
        public async Task<LoginResponseDTO> RedefinirSenha(RedefinirSenhaRequestDTO request)
        {
            try
            {
                var usuario = await _loginRepository.ObterUsuarioPorTokenAsync(request.Token);

                var timezoneOffset = TimeSpan.FromHours(-3);
                var agoraUtc3 = DateTime.UtcNow.Add(timezoneOffset);

                if (usuario == null || usuario.TokenExpiracao < agoraUtc3)
                    return new LoginResponseDTO { Valido = false, Mensagem = "Token inválido ou expirado." };

                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha, workFactor: 12);
                usuario.TokenRecuperacaoSenha = null;
                usuario.TokenExpiracao = null;

                await _loginRepository.AtualizarUsuarioAsync(usuario);

                return new LoginResponseDTO { Valido = true, Mensagem = "Senha redefinida com sucesso." };
            }
            catch (Exception e)
            {
                return new LoginResponseDTO { Valido = false, Mensagem = $"Erro ao redefinir senha: {e.Message}" };
            }
        }
    }
}