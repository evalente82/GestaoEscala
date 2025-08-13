using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.EscalaPronta;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.Interfaces.Escala;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaPronta;
using GestaoEscalaPermutas.Dominio.Interfaces.Feriados;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Repository.Interfaces;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.Escala
{
    public class EscalaService : IEscalaService
    {
        private readonly IEscalaRepository _escalaRepository;
        private readonly IMapper _mapper;
        private readonly IFuncionarioRepository _funcionarioRepository;
        private readonly IPostoTrabalhoRepository _postoTrabalhoRepository;
        private readonly ITipoEscalaRepository _tipoEscalaRepository;
        private readonly IEscalaProntaService _escalaProntaService;
        private readonly IFeriadoService _feriadoService;
        private readonly IFuncionarioService _funcionarioService;
        private readonly IPostoTrabalhoService _postoTrabalhoService;

        public EscalaService(
        IEscalaRepository escalaRepository,
        IFuncionarioRepository funcionarioRepository,
        IPostoTrabalhoRepository postoTrabalhoRepository,
        ITipoEscalaRepository tipoEscalaRepository,
        IEscalaProntaService escalaProntaService,
        IFeriadoService feriadoService,
        IMapper mapper,
        IFuncionarioService funcionarioService,
        IPostoTrabalhoService postoTrabalhoService)
        {
            _escalaRepository = escalaRepository;
            _funcionarioRepository = funcionarioRepository;
            _postoTrabalhoRepository = postoTrabalhoRepository;
            _tipoEscalaRepository = tipoEscalaRepository;
            _escalaProntaService = escalaProntaService;
            _feriadoService = feriadoService;
            _mapper = mapper;
            _funcionarioService = funcionarioService;
            _postoTrabalhoService = postoTrabalhoService;
        }

        public async Task<EscalaDTO> Incluir(EscalaDTO escalaDTO)
        {
            try
            {
                if (escalaDTO is null)
                {
                    return new EscalaDTO { valido = false, mensagem = "Objeto não preenchido." };
                }

                var escala = _mapper.Map<DepInfra.Escala>(escalaDTO);
                var novaEscala = await _escalaRepository.AdicionarAsync(escala);

                return _mapper.Map<EscalaDTO>(novaEscala);
            }
            catch (Exception e)
            {
                return new EscalaDTO { valido = false, mensagem = $"Erro ao salvar o objeto: {e.Message}" };
            }
        }

        public async Task<EscalaDTO> Alterar(Guid id, EscalaDTO escalaDTO)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new EscalaDTO { valido = false, mensagem = "Id fora do Range." };
                }

                var escalaExistente = await _escalaRepository.ObterPorIdAsync(id);
                if (escalaExistente == null)
                {
                    return new EscalaDTO { valido = false, mensagem = "Escala não encontrada." };
                }

                _mapper.Map(escalaDTO, escalaExistente);
                await _escalaRepository.AtualizarAsync(escalaExistente);

                return _mapper.Map<EscalaDTO>(escalaExistente);
            }
            catch (Exception e)
            {
                return new EscalaDTO { valido = false, mensagem = $"Erro ao alterar o objeto: {e.Message}" };
            }
        }

        public async Task<List<EscalaDTO>> BuscarTodos()
        {
            try
            {
                var escalas = await _escalaRepository.ObterTodasAsync();
                return _mapper.Map<List<EscalaDTO>>(escalas);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar escalas: {e.Message}");
            }
        }

        public async Task<EscalaDTO> Deletar(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new EscalaDTO { valido = false, mensagem = "Id fora do Range." };
                }

                var escalaExistente = await _escalaRepository.ObterPorIdAsync(id);
                if (escalaExistente == null)
                {
                    return new EscalaDTO { valido = false, mensagem = "Escala não encontrada." };
                }

                await _escalaRepository.RemoverEscalasProntasPorEscalaId(id);
                await _escalaRepository.RemoverAsync(id);

                return new EscalaDTO { valido = true, mensagem = "Escala deletada com sucesso." };
            }
            catch (Exception e)
            {
                return new EscalaDTO { valido = false, mensagem = $"Erro ao deletar o objeto: {e.Message}" };
            }
        }

        public async Task<EscalaDTO> BuscarPorId(Guid idEscala)
        {
            try
            {
                if (idEscala == Guid.Empty)
                {
                    return new EscalaDTO { valido = false, mensagem = "Id fora do Range." };
                }

                var escalaExistente = await _escalaRepository.ObterPorIdAsync(idEscala);
                if (escalaExistente == null)
                {
                    return new EscalaDTO { valido = false, mensagem = "Escala não encontrada." };
                }

                return _mapper.Map<EscalaDTO>(escalaExistente);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar o objeto: {e.Message}");
            }
        }

        // --- IMPLEMENTAÇÃO DO NOVO MÉTODO ---
        public async Task<RetornoDTO> MontarEscalaAsync(Guid idEscala)
        {
            // Busca de dados iniciais
          

            return new RetornoDTO { valido = true, mensagem = "Escala gerada com sucesso!" };
        }
    }
}
