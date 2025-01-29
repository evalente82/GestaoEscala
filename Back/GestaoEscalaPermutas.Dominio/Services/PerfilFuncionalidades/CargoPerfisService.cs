using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using AutoMapper;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using Microsoft.EntityFrameworkCore;

public class CargoPerfisService : ICargoPerfisService
{
    private readonly DefesaCivilMaricaContext _context;
    private readonly IMapper _mapper;

    public CargoPerfisService(DefesaCivilMaricaContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CargoPerfisDTO>> BuscarTodos()
    {
        var funcionariosPerfis = await _context.FuncionariosPerfis.ToListAsync();
        return _mapper.Map<IEnumerable<CargoPerfisDTO>>(funcionariosPerfis);
    }
    public async Task<bool> AtribuirPerfilAoFuncionario(Guid idFuncionario, Guid idPerfil)
    {
        if (await _context.FuncionariosPerfis.AnyAsync(fp => fp.IdCargo == idFuncionario && fp.IdPerfil == idPerfil))
        {
            return false; // Associação já existe
        }

        _context.FuncionariosPerfis.Add(new CargoPerfis
        {
            IdCargo = idFuncionario,
            IdPerfil = idPerfil
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoverPerfilDoFuncionario(Guid idFuncionario, Guid idPerfil)
    {
        var vinculo = await _context.FuncionariosPerfis.FirstOrDefaultAsync(fp => fp.IdCargo == idFuncionario && fp.IdPerfil == idPerfil);

        if (vinculo == null)
        {
            return false;
        }

        _context.FuncionariosPerfis.Remove(vinculo);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CargoPerfisDTO>> BuscarPerfisPorFuncionario(Guid idFuncionario)
    {
        var perfis = await _context.FuncionariosPerfis
            .Where(fp => fp.IdCargo == idFuncionario)
            .Include(fp => fp.Perfil)
            .Select(fp => new CargoPerfisDTO
            {
                IdFuncionario = fp.IdCargo,
                IdPerfil = fp.IdPerfil,
                NomePerfil = fp.Perfil.Nome
            })
            .ToListAsync();

        return perfis;
    }
}
