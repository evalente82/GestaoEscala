using AutoMapper;
using BCrypt.Net;
using GestaoEscalaPermutas.Dominio.DTO.Login;
using GestaoEscalaPermutas.Dominio.Interfaces.Email;
using GestaoEscalaPermutas.Dominio.Interfaces.Login;
using GestaoEscalaPermutas.Infra.Data.Context;
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
        private readonly DefesaCivilMaricaContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;        

        public LoginService(DefesaCivilMaricaContext context, IMapper mapper, IEmailService emailService)
        {
            _context = context;
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
                var usuario = await _context.Usuario
                    .Include(u => u.Perfil)
                        .ThenInclude(p => p.PerfisFuncionalidades)
                            .ThenInclude(pf => pf.Funcionalidade)
                    .FirstOrDefaultAsync(u => u.Email == loginRequest.Usuario); // Certifique-se de que está buscando pelo e-mail correto.

                if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Senha, usuario.SenhaHash))
                {
                    return new LoginResponseDTO { Valido = false, Mensagem = "Usuário ou senha inválidos." };
                }

                // Busca o funcionário associado ao usuário
                var funcionario = await _context.Funcionarios
                    .Include(f => f.Cargo)
                        .ThenInclude(c => c.CargoPerfis)
                            .ThenInclude(cp => cp.Perfil)
                                .ThenInclude(p => p.PerfisFuncionalidades)
                                    .ThenInclude(pf => pf.Funcionalidade)
                    .FirstOrDefaultAsync(f => f.NmEmail == usuario.Email);

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

                // Gera o token JWT
                var token = GerarTokenJWT(usuario, permissoes);

                return new LoginResponseDTO
                {
                    Valido = true,
                    Mensagem = "Autenticado com sucesso.",
                    Token = token,
                    NomeUsuario = usuario.Nome
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
        private string GerarTokenJWT(DepInfra.Usuarios usuario, List<string> permissoes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("ChaveSeguraSuperLongaParaTokenJWT123!");

            //var key = Encoding.ASCII.GetBytes("sua_chave_secreta_aqui"); // Substituir por configuração segura

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, usuario.Perfil?.Nome ?? "SemPerfil")
            };

            claims.AddRange(permissoes.Select(p => new Claim("Permissao", p)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
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
                var usuarioExistente = await _context.Usuario.AnyAsync(u => u.Email == loginDTO.Usuario);
                if (usuarioExistente)
                {
                    return new LoginResponseDTO { Valido = false, Mensagem = "Usuário já cadastrado." };
                }

                var funcionario = await _context.Funcionarios
                    .FirstOrDefaultAsync(f => f.NmEmail == loginDTO.Usuario);

                if (funcionario == null)
                {
                    return new LoginResponseDTO { Valido = false, Mensagem = "Funcionário não encontrado para este e-mail." };
                }

                loginDTO.SenhaHash = !string.IsNullOrEmpty(loginDTO.Senha)
                    ? BCrypt.Net.BCrypt.HashPassword(loginDTO.Senha, workFactor: 12)
                    : throw new Exception("Senha não pode ser nula.");

                var cargo = await _context.Cargos.FirstOrDefaultAsync(c => c.IdCargo == funcionario.IdCargo);

                var cargoPerfil = await _context.CargoPerfis.FirstOrDefaultAsync(p => p.IdCargo == cargo.IdCargo);
                var perfil = await _context.Perfil.FirstOrDefaultAsync(p => p.IdPerfil == cargoPerfil.IdPerfil);

                //loginDTO.Perfil = perfil?.IdPerfil != null ? perfil.IdPerfil.ToString() : "Sem Perfil";// Garante que Perfil nunca seja nulo

                var usuario = new DepInfra.Usuarios
                {
                    IdUsuario = Guid.NewGuid(),
                    IdFuncionario = funcionario.IdFuncionario,
                    IdPerfil = perfil.IdPerfil,
                    Nome = funcionario.NmNome,
                    Email = loginDTO.Usuario,
                    SenhaHash = loginDTO.SenhaHash,
                    Perfil = await _context.Perfil.FirstOrDefaultAsync(p => p.IdPerfil == cargoPerfil.IdPerfil)
                };

                _context.Usuario.Add(usuario);
                await _context.SaveChangesAsync();

                // 🔹 Gerar o Token JWT para o novo usuário
                var permissoes = new List<string> { perfil?.Nome ?? "SemPerfil" };
                var token = GerarTokenJWT(usuario, permissoes);

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
                var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Email == email);

                if (usuario == null)
                    return new LoginResponseDTO { Valido = false, Mensagem = "E-mail não encontrado." };

                // Gera um token único e define a expiração
                var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                usuario.TokenRecuperacaoSenha = token;
                usuario.TokenExpiracao = DateTime.UtcNow;

                _context.Usuario.Update(usuario);
                await _context.SaveChangesAsync();

                // Enviar e-mail com link para redefinição de senha
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

        public async Task<LoginResponseDTO> RedefinirSenha(RedefinirSenhaRequestDTO request)
        {
            try
            {
                var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.TokenRecuperacaoSenha == request.Token);

                var timezoneOffset = TimeSpan.FromHours(-3); // UTC-3
                var agoraUtc3 = DateTime.UtcNow.Add(timezoneOffset);

                if (usuario == null || usuario.TokenExpiracao < agoraUtc3)
                    return new LoginResponseDTO { Valido = false, Mensagem = "Token inválido ou expirado." };

                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha, workFactor: 12);
                usuario.TokenRecuperacaoSenha = null;
                usuario.TokenExpiracao = null;

                _context.Usuario.Update(usuario);
                await _context.SaveChangesAsync();

                return new LoginResponseDTO { Valido = true, Mensagem = "Senha redefinida com sucesso." };
            }
            catch (Exception e)
            {
                return new LoginResponseDTO { Valido = false, Mensagem = $"Erro ao redefinir senha: {e.Message}" };
            }
        }

    }
}
