using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfilFuncionalidades;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfisFuncionalidades;
using GestaoEscalaPermutas.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.PerfisFuncionalidades
{
    public class PerfisFuncionalidadesService : IPerfisFuncionalidadesService
    {
        private readonly DefesaCivilMaricaContext _context;
        private readonly IMapper _mapper;

        public async Task<IEnumerable<PerfisFuncionalidadesDTO>> BuscarTodas()
        {
            var perfisFuncionalidades = await _context.PerfisFuncionalidades.ToListAsync();
            return _mapper.Map<IEnumerable<PerfisFuncionalidadesDTO>>(perfisFuncionalidades);
        }
        public PerfisFuncionalidadesService(DefesaCivilMaricaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> AtribuirFuncionalidadeAoPerfil(Guid idPerfil, Guid idFuncionalidade)
        {
            var relacionamentoExistente = await _context.PerfisFuncionalidades
                .AnyAsync(pf => pf.IdPerfil == idPerfil && pf.IdFuncionalidade == idFuncionalidade);

            if (relacionamentoExistente)
                return false;

            var novoRelacionamento = new DepInfra.PerfisFuncionalidades
            {
                IdPerfil = idPerfil,
                IdFuncionalidade = idFuncionalidade
                
            };

            _context.PerfisFuncionalidades.Add(novoRelacionamento);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoverFuncionalidadeDoPerfil(Guid idPerfil, Guid idFuncionalidade)
        {
            var relacionamento = await _context.PerfisFuncionalidades
                .FirstOrDefaultAsync(pf => pf.IdPerfil == idPerfil && pf.IdFuncionalidade == idFuncionalidade);

            if (relacionamento == null)
                return false;

            _context.PerfisFuncionalidades.Remove(relacionamento);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<FuncionalidadeDTO>> BuscarFuncionalidadesPorPerfil(Guid idPerfil)
        {
            var funcionalidades = await _context.PerfisFuncionalidades
                .Include(pf => pf.Funcionalidade)
                .Where(pf => pf.IdPerfil == idPerfil)
                .Select(pf => pf.Funcionalidade)
                .ToListAsync();

            return _mapper.Map<IEnumerable<FuncionalidadeDTO>>(funcionalidades);
        }
    }
}
