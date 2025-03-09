using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface ILoginRepository
    {
        /// <summary>
        /// Obtém um usuário pelo e-mail, incluindo perfil e permissões.
        /// </summary>
        Task<Usuarios> ObterUsuarioPorEmailAsync(string email);

        /// <summary>
        /// Obtém um funcionário pelo e-mail, incluindo cargo, perfil e permissões.
        /// </summary>
        Task<Funcionario> ObterFuncionarioPorEmailAsync(string email);

        /// <summary>
        /// Verifica se um usuário existe pelo e-mail.
        /// </summary>
        Task<bool> UsuarioExisteAsync(string email);

        /// <summary>
        /// Cria um novo usuário no banco de dados.
        /// </summary>
        Task<Usuarios> CriarUsuarioAsync(Usuarios usuario);

        /// <summary>
        /// Atualiza um usuário existente no banco de dados.
        /// </summary>
        Task AtualizarUsuarioAsync(Usuarios usuario);

        /// <summary>
        /// Obtém um perfil pelo ID.
        /// </summary>
        Task<Perfil> ObterPerfilPorIdAsync(Guid idPerfil);

        /// <summary>
        /// Obtém um cargo pelo ID.
        /// </summary>
        Task<Cargo> ObterCargoPorIdAsync(Guid idCargo);

        /// <summary>
        /// Obtém a relação Cargo-Perfil pelo ID do Cargo.
        /// </summary>
        Task<CargoPerfis> ObterCargoPerfilPorCargoAsync(Guid idCargo);

        /// <summary>
        /// Obtém um usuário pelo token de recuperação de senha.
        /// </summary>
        Task<Usuarios> ObterUsuarioPorTokenAsync(string token);

        Task<Usuarios> ObterUsuarioComPerfilEPermissoesAsync(string email);
        Task<Funcionario> ObterFuncionarioComCargoEPermissoesAsync(string email);

        Task<Usuarios> ObterUsuarioPorRefreshTokenAsync(string refreshToken);

        Task<Usuarios> ObterUsuarioPorIdAsync(Guid idUsuario); 
    }
}
