using AutoMapper;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfilFuncionalidades;
using GestaoEscalaPermutas.Repository.Interfaces;

namespace GestaoEscalaPermutas.Dominio.Services.Funcionalidade
{
    public class FuncionalidadeService : IFuncionalidadeService
    {
        private readonly IFuncionalidadeRepository _funcionalidadeRepository;
        private readonly IMapper _mapper;

        public FuncionalidadeService(IFuncionalidadeRepository funcionalidadeRepository, IMapper mapper)
        {
            _funcionalidadeRepository = funcionalidadeRepository;
            _mapper = mapper;
        }

        public async Task<FuncionalidadeDTO> Criar(FuncionalidadeDTO funcionalidadeDTO)
        {
            var funcionalidade = _mapper.Map<DepInfra.Funcionalidade>(funcionalidadeDTO);
            funcionalidade = await _funcionalidadeRepository.CriarAsync(funcionalidade);
            return _mapper.Map<FuncionalidadeDTO>(funcionalidade);
        }

        public async Task<FuncionalidadeDTO> Atualizar(FuncionalidadeDTO funcionalidadeDTO)
        {
            var funcionalidade = await _funcionalidadeRepository.BuscarPorIdAsync(funcionalidadeDTO.IdFuncionalidade);
            if (funcionalidade == null)
                throw new KeyNotFoundException("Funcionalidade não encontrada.");

            funcionalidade.Nome = funcionalidadeDTO.Nome;
            funcionalidade.Descricao = funcionalidadeDTO.Descricao;

            funcionalidade = await _funcionalidadeRepository.AtualizarAsync(funcionalidade);
            return _mapper.Map<FuncionalidadeDTO>(funcionalidade);
        }

        public async Task<bool> Deletar(Guid id)
        {
            return await _funcionalidadeRepository.DeletarAsync(id);
        }

        public async Task<IEnumerable<FuncionalidadeDTO>> BuscarTodas()
        {
            var funcionalidades = await _funcionalidadeRepository.BuscarTodasAsync();
            return _mapper.Map<IEnumerable<FuncionalidadeDTO>>(funcionalidades);
        }

        public async Task<FuncionalidadeDTO?> BuscarPorId(Guid id)
        {
            var funcionalidade = await _funcionalidadeRepository.BuscarPorIdAsync(id);
            return _mapper.Map<FuncionalidadeDTO?>(funcionalidade);
        }
    }
}