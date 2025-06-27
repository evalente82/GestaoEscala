using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Repository.Implementations
{
    public class EscalaExtraCargoRepository : IEscalaExtraCargoRepository
    {
        // 1. Injete o seu DbContext
        // O repositório precisa de uma instância do seu DbContext para interagir com o banco.
        private readonly DefesaCivilMaricaContext _context;

        public EscalaExtraCargoRepository(DefesaCivilMaricaContext context) // <-- E AQUI TAMBÉM
        {
            _context = context;
        }

        /// <summary>
        /// Adiciona uma lista de registos da tabela de junção CriacaoEscalaExtraCargo
        /// de forma assíncrona e eficiente.
        /// </summary>
        /// <param name="listaDeCargos">A lista de objetos de junção a serem adicionados.</param>
        public async Task AdicionarListaExtraCargosAsync(List<CriacaoEscalaExtraCargo> listaDeCargos)
        {
            // 2. Validação (Boa Prática)
            // Se a lista estiver vazia ou for nula, não há nada a fazer.
            if (listaDeCargos == null || !listaDeCargos.Any())
            {
                return;
            }

            // 3. Use AddRangeAsync para performance
            // Este é o método do EF Core otimizado para adicionar múltiplos registos.
            // É muito mais rápido do que um loop a chamar "AddAsync" para cada item.
            // Assumimos que o seu DbSet se chama "CriacaoEscalaExtraCargos". Se tiver outro nome, ajuste aqui.
            await _context.CriacaoEscalaExtraCargo.AddRangeAsync(listaDeCargos);

            // 4. NÃO chame SaveChangesAsync() aqui!
            // O repositório apenas prepara as alterações. A responsabilidade de salvar (chamar SaveChangesAsync)
            // deve ser da camada de serviço (ou Unit of Work). Isso garante que a criação da EscalaExtra
            // e a adição dos seus cargos aconteçam na mesma transação. Se a adição dos cargos falhar,
            // a criação da EscalaExtra também é revertida.
        }

        public Task AlterarAsync(CriacaoEscalaExtraCargo escalaExtra)
        {
            _context.CriacaoEscalaExtraCargo.Update(escalaExtra);
            return Task.CompletedTask;
        }

        public void DeletarAsync(CriacaoEscalaExtraCargo escalaExtra)
        {
            _context.CriacaoEscalaExtraCargo.Remove(escalaExtra);
        }

        public async Task<CriacaoEscalaExtraCargo?> ObterPorIdAsync(Guid id)
        {
            return await _context.CriacaoEscalaExtraCargo.FindAsync(id);
        }

        public Task<List<CriacaoEscalaExtraCargo>> ObterTodosAsync()
        {
            return _context.CriacaoEscalaExtraCargo.ToListAsync();
        }
        public async Task<IEnumerable<Guid>> ObterCargosPorEscalaExtraIdAsync(Guid idCriacaoEscalaExtra)
        {
            return await _context.CriacaoEscalaExtraCargo
                             .Where(ec => ec.IdCriacaoEscalaExtra == idCriacaoEscalaExtra)
                             .Select(ec => ec.IdCargo)
                             .ToListAsync();
        }

    }
}
