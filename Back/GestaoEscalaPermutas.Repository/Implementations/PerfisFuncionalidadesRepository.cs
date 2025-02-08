using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class PerfisFuncionalidadesRepository : IPerfisFuncionalidadesRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public PerfisFuncionalidadesRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PerfisFuncionalidades>> BuscarTodasAsync()
        {
            return await _context.PerfisFuncionalidades.ToListAsync();
        }

        public async Task<bool> AtribuirFuncionalidadeAoPerfilAsync(Guid idPerfil, Guid idFuncionalidade)
        {
            var relacionamentoExistente = await _context.PerfisFuncionalidades
                .AnyAsync(pf => pf.IdPerfil == idPerfil && pf.IdFuncionalidade == idFuncionalidade);

            if (relacionamentoExistente)
                return false;

            var novoRelacionamento = new PerfisFuncionalidades
            {
                IdPerfil = idPerfil,
                IdFuncionalidade = idFuncionalidade
            };

            _context.PerfisFuncionalidades.Add(novoRelacionamento);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoverFuncionalidadeDoPerfilAsync(Guid idPerfil, Guid idFuncionalidade)
        {
            var relacionamento = await _context.PerfisFuncionalidades
                .FirstOrDefaultAsync(pf => pf.IdPerfil == idPerfil && pf.IdFuncionalidade == idFuncionalidade);

            if (relacionamento == null)
                return false;

            _context.PerfisFuncionalidades.Remove(relacionamento);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Funcionalidade>> BuscarFuncionalidadesPorPerfilAsync(Guid idPerfil)
        {
            return await _context.PerfisFuncionalidades
                .Include(pf => pf.Funcionalidade)
                .Where(pf => pf.IdPerfil == idPerfil)
                .Select(pf => pf.Funcionalidade)
                .ToListAsync();
        }
    }
}
