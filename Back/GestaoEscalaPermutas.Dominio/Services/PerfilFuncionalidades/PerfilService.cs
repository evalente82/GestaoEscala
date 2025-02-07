using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfilFuncionalidades;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;


namespace GestaoEscalaPermutas.Dominio.Services.PerfilFuncionalidades
{
    public class PerfilService : IPerfilService
    {
        private readonly IPerfilRepository _perfilRepository;
        private readonly IMapper _mapper;

        public PerfilService(IPerfilRepository perfilRepository, IMapper mapper)
        {
            _perfilRepository = perfilRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PerfilDTO>> BuscarTodos()
        {
            var perfis = await _perfilRepository.BuscarTodosAsync();
            return _mapper.Map<IEnumerable<PerfilDTO>>(perfis);
        }

        public async Task<PerfilDTO> BuscarPorId(Guid idPerfil)
        {
            var perfil = await _perfilRepository.BuscarPorIdAsync(idPerfil);
            return _mapper.Map<PerfilDTO>(perfil);
        }

        public async Task<PerfilDTO> Criar(PerfilDTO perfilDTO)
        {
            var perfil = _mapper.Map<Infra.Data.EntitiesDefesaCivilMarica.Perfil>(perfilDTO);
            perfil = await _perfilRepository.CriarAsync(perfil);
            return _mapper.Map<PerfilDTO>(perfil);
        }

        public async Task<PerfilDTO> Atualizar(PerfilDTO perfilDTO)
        {
            var perfil = await _perfilRepository.BuscarPorIdAsync(perfilDTO.IdPerfil);

            if (perfil == null)
                throw new Exception("Perfil não encontrado.");

            perfil.Nome = perfilDTO.Nome;
            perfil.Descricao = perfilDTO.Descricao;

            perfil = await _perfilRepository.AtualizarAsync(perfil);
            return _mapper.Map<PerfilDTO>(perfil);
        }

        public async Task<bool> Deletar(Guid idPerfil)
        {
            return await _perfilRepository.DeletarAsync(idPerfil);
        }
    }
}