using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Repository.Interfaces
{
    public interface IFeriadoRepository
    {
        Task AdicionarAsync(Feriado feriado);
        Task<IEnumerable<Feriado>> ObterPorAnoAsync(int ano);
    }
}
