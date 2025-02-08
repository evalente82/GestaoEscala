using GestaoEscalaPermutas.Repository.Interfaces;
using GestaoEscalaPermutas.Repository.Implementations;
using GestaoEscalaPermutas.Repository.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoEscalaPermutas.Repository.DependencyInjection
{
    public static class RepositoryModule
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IEscalaRepository, EscalaRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWorkService>();
            services.AddScoped<ICargoRepository, CargoRepository>();
            services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
            services.AddScoped<IEscalaProntaRepository, EscalaProntaRepository>();
            services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<ICargoPerfisRepository, CargoPerfisRepository>();
            services.AddScoped<IFuncionalidadeRepository, FuncionalidadeRepository>();
            services.AddScoped<IPerfilRepository, PerfilRepository>();
            services.AddScoped<IPerfisFuncionalidadesRepository, PerfisFuncionalidadesRepository>();
            services.AddScoped<IPermutasRepository, PermutasRepository>();
            services.AddScoped<IPostoTrabalhoRepository, PostoTrabalhoRepository>();
            services.AddScoped<ISetorRepository, SetorRepository>();
            services.AddScoped<ITipoEscalaRepository, TipoEscalaRepository>();

            return services; // 🔹 Agora retorna IServiceCollection
        }
    }
}
