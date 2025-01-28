using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfilFuncionalidades;
using GestaoEscalaPermutas.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.PerfilFuncionalidades
{
    public class PerfilService : IPerfilService
    {
        private readonly DefesaCivilMaricaContext _context;
        private readonly IMapper _mapper;

        public PerfilService(DefesaCivilMaricaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PerfilDTO>> BuscarTodos()
        {
            var perfis = await _context.Perfil.ToListAsync();
            return _mapper.Map<IEnumerable<PerfilDTO>>(perfis);
        }

        public async Task<PerfilDTO> BuscarPorId(Guid idPerfil)
        {
            var perfil = await _context.Perfil.FindAsync(idPerfil);
            return _mapper.Map<PerfilDTO>(perfil);
        }

        public async Task<PerfilDTO> Criar(PerfilDTO perfilDTO)
        {
            var perfil = _mapper.Map<Infra.Data.EntitiesDefesaCivilMarica.Perfil>(perfilDTO);
            perfil.IdPerfil = Guid.NewGuid();
            _context.Perfil.Add(perfil);
            await _context.SaveChangesAsync();
            return _mapper.Map<PerfilDTO>(perfil);
        }

        public async Task<PerfilDTO> Atualizar(PerfilDTO perfilDTO)
        {
            var perfil = await _context.Perfil.FindAsync(perfilDTO.IdPerfil);

            if (perfil == null)
                throw new Exception("Perfil não encontrado.");

            perfil.Nome = perfilDTO.Nome;
            perfil.Descricao = perfilDTO.Descricao;

            _context.Perfil.Update(perfil);
            await _context.SaveChangesAsync();
            return _mapper.Map<PerfilDTO>(perfil);
        }

        public async Task<bool> Deletar(Guid idPerfil)
        {
            var perfil = await _context.Perfil.FindAsync(idPerfil);

            if (perfil == null)
                return false;

            _context.Perfil.Remove(perfil);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
