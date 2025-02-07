using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Usuario;
using GestaoEscalaPermutas.Dominio.Interfaces.Usuario;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public UsuarioService(IUsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        public async Task<UsuarioDTO> CriarUsuarioVinculado(UsuarioDTO usuarioDTO)
        {
            var usuario = _mapper.Map<DepInfra.Usuarios>(usuarioDTO);
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuarioDTO.Senha);
            usuario.DataCriacao = DateTime.UtcNow;

            var usuarioCriado = await _usuarioRepository.CriarAsync(usuario);
            return _mapper.Map<UsuarioDTO>(usuarioCriado);
        }

        public async Task<bool> VerificarUsuarioPorFuncionario(Guid idFuncionario)
        {
            return await _usuarioRepository.VerificarUsuarioPorFuncionarioAsync(idFuncionario);
        }

        public async Task<UsuarioDTO> Criar(UsuarioDTO usuarioDTO)
        {
            var usuario = _mapper.Map<DepInfra.Usuarios>(usuarioDTO);
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuarioDTO.Senha);
            usuario.DataCriacao = DateTime.UtcNow;

            var usuarioCriado = await _usuarioRepository.CriarAsync(usuario);
            return _mapper.Map<UsuarioDTO>(usuarioCriado);
        }

        public async Task<UsuarioDTO> BuscarPorId(Guid id)
        {
            var usuario = await _usuarioRepository.BuscarPorIdAsync(id);
            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            return _mapper.Map<UsuarioDTO>(usuario);
        }

        public async Task<IEnumerable<UsuarioDTO>> BuscarTodos()
        {
            var usuarios = await _usuarioRepository.BuscarTodosAsync();
            return _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);
        }

        public async Task<UsuarioDTO> Atualizar(UsuarioDTO usuarioDTO)
        {
            var usuario = await _usuarioRepository.BuscarPorIdAsync(usuarioDTO.IdUsuario);
            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            usuario.Nome = usuarioDTO.Nome;
            usuario.Email = usuarioDTO.Email;
            usuario.Ativo = usuarioDTO.Ativo;

            if (!string.IsNullOrEmpty(usuarioDTO.Senha))
                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuarioDTO.Senha);

            var usuarioAtualizado = await _usuarioRepository.AtualizarAsync(usuario);
            return _mapper.Map<UsuarioDTO>(usuarioAtualizado);
        }

        public async Task<bool> Deletar(Guid id)
        {
            return await _usuarioRepository.DeletarAsync(id);
        }
    }
}