using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO;
using GestaoEscalaPermutas.Dominio.Interfaces;
using GestaoEscalaPermutas.Infra.Data.Context;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using Microsoft.EntityFrameworkCore;
using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfilFuncionalidades;

public class FuncionalidadeService : IFuncionalidadeService
{
    private readonly DefesaCivilMaricaContext _context;
    private readonly IMapper _mapper;

    public FuncionalidadeService(DefesaCivilMaricaContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<FuncionalidadeDTO> Criar(FuncionalidadeDTO funcionalidadeDTO)
    {
        var funcionalidade = _mapper.Map<DepInfra.Funcionalidade>(funcionalidadeDTO);

        await _context.Funcionalidades.AddAsync(funcionalidade);
        await _context.SaveChangesAsync();

        return _mapper.Map<FuncionalidadeDTO>(funcionalidade);
    }

    public async Task<FuncionalidadeDTO> Atualizar(FuncionalidadeDTO funcionalidadeDTO)
    {
        var funcionalidade = await _context.Funcionalidades.FindAsync(funcionalidadeDTO.IdFuncionalidade);
        if (funcionalidade == null)
            throw new KeyNotFoundException("Funcionalidade não encontrada.");

        funcionalidade.Nome = funcionalidadeDTO.Nome;
        funcionalidade.Descricao = funcionalidadeDTO.Descricao;

        _context.Funcionalidades.Update(funcionalidade);
        await _context.SaveChangesAsync();

        return _mapper.Map<FuncionalidadeDTO>(funcionalidade);
    }

    public async Task<bool> Deletar(Guid id)
    {
        var funcionalidade = await _context.Funcionalidades.FindAsync(id);
        if (funcionalidade == null)
            return false;

        _context.Funcionalidades.Remove(funcionalidade);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<FuncionalidadeDTO>> BuscarTodas()
    {
        var funcionalidades = await _context.Funcionalidades.OrderBy(x => x.Nome).ToListAsync();
        return _mapper.Map<IEnumerable<FuncionalidadeDTO>>(funcionalidades);
    }

    public async Task<FuncionalidadeDTO?> BuscarPorId(Guid id)
    {
        var funcionalidade = await _context.Funcionalidades.FindAsync(id);
        return _mapper.Map<FuncionalidadeDTO?>(funcionalidade);
    }
}
