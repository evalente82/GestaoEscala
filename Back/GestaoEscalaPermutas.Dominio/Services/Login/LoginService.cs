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

        public async Task<LoginResponseDTO> Autenticar(LoginDTO loginRequest)
        {
            try
            {
                // Busca o usuário pelo nome
                var usuario = await _context.Usuario
                    .FirstOrDefaultAsync(u => u.IdUsuario == loginRequest.Id);

                if (usuario == null)
                {
                    return new LoginResponseDTO
                    {
                        Valido = false,
                        Mensagem = "Usuário ou senha inválidos."
                    };
                }

                // Verifica a senha
                if (!BCrypt.Net.BCrypt.Verify(loginRequest.Senha, usuario.SenhaHash))
                {
                    return new LoginResponseDTO
                    {
                        Valido = false,
                        Mensagem = "Usuário ou senha inválidos."
                    };
                }

                // Gera o token JWT
                var token = GerarTokenJWT(usuario);

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

        // Método para gerar o token JWT
        private string GerarTokenJWT(DepInfra.Usuarios usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("sua_chave_secreta_aqui"); // Use uma chave forte

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.Perfil) // Inclui o perfil como Role
        }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<LoginDTO> Incluir(LoginDTO loginDTO)
        {
            try
            {
                if (loginDTO == null)
                {
                    return new LoginDTO { valido = false, mensagem = "Objeto não preenchido." };
                }

                // Gera o hash da senha
                loginDTO.SenhaHash = BCrypt.Net.BCrypt.HashPassword(loginDTO.Senha);

                // Mapear LoginDTO para Login
                var login = _mapper.Map<DepInfra.Login>(loginDTO);

                // Salvar no banco
                _context.Login.Add(login);
                await _context.SaveChangesAsync();

                return _mapper.Map<LoginDTO>(login);
            }
            catch (Exception e)
            {
                return new LoginDTO { valido = false, mensagem = $"Erro ao incluir: {e.Message}" };
            }
        }


    }
}
