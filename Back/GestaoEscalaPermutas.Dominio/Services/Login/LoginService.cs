using AutoMapper;
using BCrypt.Net;
using GestaoEscalaPermutas.Dominio.DTO.Login;
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

        public LoginService(DefesaCivilMaricaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Autentica um usuário e gera um token com as permissões.
        /// </summary>
        public async Task<LoginResponseDTO> Autenticar(LoginRequestDTO loginRequest)
        {
            try
            {
                // Busca o usuário no banco de dados
                var usuario = await _context.Usuario
                    .Include(u => u.Perfil) // Inclui o perfil do usuário
                    .ThenInclude(p => p.PerfisFuncionalidades) // Inclui as permissões do perfil
                    .ThenInclude(pf => pf.Funcionalidade) // Inclui os nomes das funcionalidades
                    .FirstOrDefaultAsync(u => u.Email == loginRequest.Usuario);// aqui vai buscar o email ? forçar 

                if (usuario == null)
                {
                    return new LoginResponseDTO { Valido = false, Mensagem = "Usuário ou senha inválidos." };
                }

                if (!BCrypt.Net.BCrypt.Verify(loginRequest.Senha, usuario.SenhaHash))
                {
                    return new LoginResponseDTO { Valido = false, Mensagem = "Usuário ou senha inválidos." };
                }

                // Obtém as permissões do usuário
                var permissoes = usuario.Perfil?.PerfisFuncionalidades?
                    .Select(pf => pf.Funcionalidade.Nome)
                    .ToList() ?? new List<string>();

                // Gera o token JWT com as permissões
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
                return new LoginResponseDTO
                {
                    Valido = false,
                    Mensagem = $"Erro ao autenticar: {e.Message}"
                };
            }
        }

        /// <summary>
        /// Gera um token JWT com permissões.
        /// </summary>
        private string GerarTokenJWT(DepInfra.Usuarios usuario, List<string> permissoes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("sua_chave_secreta_aqui"); // Use uma chave forte

            //var secret = _config["JwtSettings:Secret"]; // analisar para usar mais segurança
            //var key = Encoding.ASCII.GetBytes(secret);

            var claims = new List<Claim>
    {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome), // Corrigido de usuario.Usuario para usuario.Nome
                new Claim(ClaimTypes.Role, usuario.Perfil?.Nome ?? "SemPerfil") // Corrigido de usuario.Perfil.Nome para usuario.Perfil (que é string)
    };

            // Adiciona permissões como claims personalizadas
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

                // Gera o hash da senha antes de salvar
                loginDTO.SenhaHash = BCrypt.Net.BCrypt.HashPassword(loginDTO.Senha, workFactor: 12);// maior segurança

                // Converte DTO para entidade
                var usuario = _mapper.Map<DepInfra.Usuarios>(loginDTO) ?? new DepInfra.Usuarios();


                // Salva no banco de dados
                _context.Usuario.Add(usuario);
                await _context.SaveChangesAsync();

                return new LoginResponseDTO
                {
                    Valido = true,
                    Mensagem = "Usuário cadastrado com sucesso."
                };
            }
            catch (Exception e)
            {
                return new LoginResponseDTO { Valido = false, Mensagem = $"Erro ao incluir usuário: {e.Message}" };
            }
        }
    }
}
