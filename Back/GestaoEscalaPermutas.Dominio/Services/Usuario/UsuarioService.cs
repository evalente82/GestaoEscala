using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Usuario;
using GestaoEscalaPermutas.Dominio.Interfaces.Usuario;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly DefesaCivilMaricaContext _context;
        private readonly IMapper _mapper;

        public UsuarioService(DefesaCivilMaricaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UsuarioDTO> CriarUsuarioVinculado(UsuarioDTO usuarioDTO)
        {
            // Verificar se o funcionário já existe
            var funcionario = await _context.Funcionarios.FindAsync(usuarioDTO.IdFuncionario);

            if (funcionario == null)
            {
                throw new Exception("Funcionário não encontrado.");
            }

            // Criar o usuário vinculado
            var usuario = _mapper.Map<DepInfra.Usuarios>(usuarioDTO);
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuarioDTO.Senha); // Hash da senha
            usuario.DataCriacao = DateTime.UtcNow;
            usuario.Nome = funcionario.NmNome;
            usuario.Email = funcionario.NmEmail;
            usuario.IdFuncionario = funcionario.IdFuncionario;

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            return _mapper.Map<UsuarioDTO>(usuario);
        }

        public async Task<bool> VerificarUsuarioPorFuncionario(Guid idFuncionario)
        {
            return await _context.Usuario.AnyAsync(u => u.IdFuncionario == idFuncionario);
        }

        public async Task<UsuarioDTO> Criar(UsuarioDTO usuarioDTO)
        {
            // Verifica se o perfil existe
            var perfil = await _context.Perfil.FindAsync(usuarioDTO.IdPerfil);
            if (perfil == null)
            {
                throw new Exception("Perfil não encontrado.");
            }

            // Verifica se o funcionário existe (opcional)
            if (usuarioDTO.IdFuncionario.HasValue)
            {
                var funcionario = await _context.Funcionarios.FindAsync(usuarioDTO.IdFuncionario);
                if (funcionario == null)
                {
                    throw new Exception("Funcionário não encontrado.");
                }
            }

            var usuario = _mapper.Map<DepInfra.Usuarios>(usuarioDTO);
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuarioDTO.Senha); // Hash da senha
            usuario.DataCriacao = DateTime.UtcNow;

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            return _mapper.Map<UsuarioDTO>(usuario);
        }


        public async Task<UsuarioDTO> BuscarPorId(Guid id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            return _mapper.Map<UsuarioDTO>(usuario);
        }

        public async Task<IEnumerable<UsuarioDTO>> BuscarTodos()
        {
            var usuarios = await _context.Usuario.ToListAsync();
            return _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);
        }

        public async Task<UsuarioDTO> Atualizar(UsuarioDTO usuarioDTO)
        {
            var usuario = await _context.Usuario.FindAsync(usuarioDTO.IdUsuario);

            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            usuario.Nome = usuarioDTO.Nome;
            usuario.Email = usuarioDTO.Email;
            usuario.Ativo = usuarioDTO.Ativo;

            if (!string.IsNullOrEmpty(usuarioDTO.Senha))
                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuarioDTO.Senha);

            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();

            return _mapper.Map<UsuarioDTO>(usuario);
        }

        public async Task<bool> Deletar(Guid id)
        {
            var usuario = await _context.Usuario.FindAsync(id);

            if (usuario == null)
                return false;

            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
