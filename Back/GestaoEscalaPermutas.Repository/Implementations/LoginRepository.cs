using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class LoginRepository : ILoginRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public LoginRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém um usuário pelo e-mail, incluindo perfil e permissões.
        /// </summary>
        public async Task<Usuarios> ObterUsuarioPorEmailAsync(string email)
        {
            return await _context.Usuario
                .Include(u => u.Perfil)
                    .ThenInclude(p => p.PerfisFuncionalidades)
                        .ThenInclude(pf => pf.Funcionalidade)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Obtém um funcionário pelo e-mail, incluindo cargo, perfil e permissões.
        /// </summary>
        public async Task<Funcionario> ObterFuncionarioPorEmailAsync(string email)
        {
            return await _context.Funcionarios
                .Include(f => f.Cargo)
                    .ThenInclude(c => c.CargoPerfis)
                        .ThenInclude(cp => cp.Perfil)
                            .ThenInclude(p => p.PerfisFuncionalidades)
                                .ThenInclude(pf => pf.Funcionalidade)
                .FirstOrDefaultAsync(f => f.NmEmail == email);
        }

        /// <summary>
        /// Verifica se um usuário existe pelo e-mail.
        /// </summary>
        public async Task<bool> UsuarioExisteAsync(string email)
        {
            return await _context.Usuario.AnyAsync(u => u.Email == email);
        }

        /// <summary>
        /// Cria um novo usuário no banco de dados.
        /// </summary>
        public async Task<Usuarios> CriarUsuarioAsync(Usuarios usuario)
        {
            await _context.Usuario.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        /// <summary>
        /// Atualiza um usuário existente no banco de dados.
        /// </summary>
        public async Task AtualizarUsuarioAsync(Usuarios usuario)
        {
            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtém um perfil pelo ID.
        /// </summary>
        public async Task<Perfil> ObterPerfilPorIdAsync(Guid idPerfil)
        {
            return await _context.Perfil.FindAsync(idPerfil);
        }

        /// <summary>
        /// Obtém um cargo pelo ID.
        /// </summary>
        public async Task<Cargo> ObterCargoPorIdAsync(Guid idCargo)
        {
            return await _context.Cargos.FindAsync(idCargo);
        }

        /// <summary>
        /// Obtém a relação Cargo-Perfil pelo ID do Cargo.
        /// </summary>
        public async Task<CargoPerfis> ObterCargoPerfilPorCargoAsync(Guid idCargo)
        {
            return await _context.CargoPerfis.FirstOrDefaultAsync(p => p.IdCargo == idCargo);
        }

        /// <summary>
        /// Obtém um usuário com perfil e permissões, utilizado na autenticação.
        /// </summary>
        public async Task<Usuarios> ObterUsuarioComPerfilEPermissoesAsync(string email)
        {
            return await _context.Usuario
                .Include(u => u.Perfil)
                    .ThenInclude(p => p.PerfisFuncionalidades)
                        .ThenInclude(pf => pf.Funcionalidade)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Obtém um funcionário com cargo e permissões, utilizado na autenticação.
        /// </summary>
        public async Task<Funcionario> ObterFuncionarioComCargoEPermissoesAsync(string email)
        {
            return await _context.Funcionarios
                .Include(f => f.Cargo)
                    .ThenInclude(c => c.CargoPerfis)
                        .ThenInclude(cp => cp.Perfil)
                            .ThenInclude(p => p.PerfisFuncionalidades)
                                .ThenInclude(pf => pf.Funcionalidade)
                .FirstOrDefaultAsync(f => f.NmEmail == email);
        }

        /// <summary>
        /// Obtém um usuário pelo token de recuperação de senha.
        /// </summary>
        public async Task<Usuarios> ObterUsuarioPorTokenAsync(string token)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.TokenRecuperacaoSenha == token);
        }

        public async Task<Usuarios> ObterUsuarioPorRefreshTokenAsync(string refreshToken)
        {
            return await _context.Usuario
                .Include(u => u.Perfil)
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<Usuarios> ObterUsuarioPorIdAsync(Guid idUsuario)
        {
            return await _context.Usuario
                .Include(u => u.Perfil)
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
        }
    }
}
