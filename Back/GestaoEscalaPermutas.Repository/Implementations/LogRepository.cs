using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class LogRepository : ILogRepository
    {
        private readonly DefesaCivilMaricaContext _context;

        public LogRepository(DefesaCivilMaricaContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(LogGestaoEscala log)
        {
            await _context.LogGestaoEscala.AddAsync(log);
        }
    }
}
