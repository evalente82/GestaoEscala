using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Departamento;
using GestaoEscalaPermutas.Dominio.Interfaces.Departamento;
using GestaoEscalaPermutas.Repository.Interfaces;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.Departamento
{
    public class DepartamentoService : IDepartamentoService
    {
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly IMapper _mapper;

        public DepartamentoService(IDepartamentoRepository departamentoRepository, IMapper mapper)
        {
            _departamentoRepository = departamentoRepository;
            _mapper = mapper;
        }

        public async Task<DepartamentoDTO> Incluir(DepartamentoDTO departamentoDTO)
        {
            try
            {
                if (departamentoDTO is null)
                {
                    return new DepartamentoDTO { valido = false, mensagem = "Objeto não preenchido." };
                }

                var departamento = _mapper.Map<DepInfra.Departamento>(departamentoDTO);
                var novoDepartamento = await _departamentoRepository.AdicionarAsync(departamento);

                return _mapper.Map<DepartamentoDTO>(novoDepartamento);
            }
            catch (Exception e)
            {
                return new DepartamentoDTO { valido = false, mensagem = $"Erro ao salvar o objeto: {e.Message}" };
            }
        }

        public async Task<DepartamentoDTO> Alterar(Guid id, DepartamentoDTO departamentoDTO)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new DepartamentoDTO { valido = false, mensagem = "Id fora do Range." };
                }

                var departamentoExistente = await _departamentoRepository.ObterPorIdAsync(id);
                if (departamentoExistente == null)
                {
                    return new DepartamentoDTO { valido = false, mensagem = "Departamento não encontrado." };
                }

                _mapper.Map(departamentoDTO, departamentoExistente);
                await _departamentoRepository.AtualizarAsync(departamentoExistente);

                return _mapper.Map<DepartamentoDTO>(departamentoExistente);
            }
            catch (Exception e)
            {
                return new DepartamentoDTO { valido = false, mensagem = $"Erro ao alterar o objeto: {e.Message}" };
            }
        }

        public async Task<List<DepartamentoDTO>> BuscarTodos()
        {
            try
            {
                var departamentos = await _departamentoRepository.ObterTodosAsync();
                return _mapper.Map<List<DepartamentoDTO>>(departamentos);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar departamentos: {e.Message}");
            }
        }

        public async Task<DepartamentoDTO> Deletar(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new DepartamentoDTO { valido = false, mensagem = "Id fora do Range." };
                }

                var departamentoExistente = await _departamentoRepository.ObterPorIdAsync(id);
                if (departamentoExistente == null)
                {
                    return new DepartamentoDTO { valido = false, mensagem = "Departamento não encontrado." };
                }

                await _departamentoRepository.RemoverAsync(id);

                return new DepartamentoDTO { valido = true, mensagem = "Departamento deletado com sucesso." };
            }
            catch (Exception e)
            {
                return new DepartamentoDTO { valido = false, mensagem = $"Erro ao deletar o objeto: {e.Message}" };
            }
        }
    }
}
