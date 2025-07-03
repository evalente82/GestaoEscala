using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.EscalaPronta;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.Interfaces.Escala;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaPronta;
using GestaoEscalaPermutas.Dominio.Interfaces.Feriados;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Dominio.Services.PostoTrabalho;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
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
            var escalaDTO = await _escalaRepository.ObterPorIdAsync(idEscala);
            if (escalaDTO == null) return new RetornoDTO { valido = false, mensagem = "Escala não encontrada." };
            if (escalaDTO.IsGerada) return new RetornoDTO { valido = false, mensagem = $"A Escala {escalaDTO.NmNomeEscala} já foi gerada!" };

            var tipoEscala = await _tipoEscalaRepository.BuscarPorIdAsync(escalaDTO.IdTipoEscala);
            if (tipoEscala == null) return new RetornoDTO { valido = false, mensagem = "Tipo de Escala associado não encontrado." };

            // Buscando a lista de DTOs, como manda a arquitetura
            var funcionariosDTOs = await _funcionarioService.BuscarTodosAtivos();
            var listFuncionariosDTO = funcionariosDTOs.Where(x => x.IdCargo == escalaDTO.IdCargo).ToList();

            var postosDTOs = await _postoTrabalhoService.BuscarTodosAtivos();
            var listPostos = postosDTOs.Where(x => x.IdDepartamento == escalaDTO.IdDepartamento).ToList();

            var listEscalaPronta = new List<EscalaProntaDTO>();
            var anoReferencia = DateTime.Now.Year;
            var mesReferencia = escalaDTO.NrMesReferencia;

            // --- BIFURCAÇÃO DA LÓGICA ---
            if (tipoEscala.IsExpediente)
            {
                // LÓGICA PARA EXPEDIENTE USANDO A LISTA DE DTOs
                var feriados = await _feriadoService.ObterDatasFeriadosAsync(anoReferencia);
                var diasNoMes = DateTime.DaysInMonth(anoReferencia, mesReferencia);
                var funcionariosDisponiveis = new List<FuncionarioDTO>(listFuncionariosDTO); // Criando cópia da lista de DTOs

                foreach (var posto in listPostos)
                {
                    var equipeDoPosto = funcionariosDisponiveis.Take(escalaDTO.NrPessoaPorPosto).ToList();
                    if (!equipeDoPosto.Any()) break;

                    for (int dia = 1; dia <= diasNoMes; dia++)
                    {
                        var dataAtual = new DateTime(anoReferencia, mesReferencia, dia);
                        if (dataAtual.DayOfWeek == DayOfWeek.Saturday || dataAtual.DayOfWeek == DayOfWeek.Sunday || feriados.Contains(dataAtual.Date))
                        {
                            continue;
                        }

                        foreach (var funcionarioDaEquipe in equipeDoPosto)
                        {
                            listEscalaPronta.Add(new EscalaProntaDTO
                            {
                                IdEscala = escalaDTO.IdEscala,
                                IdFuncionario = funcionarioDaEquipe.IdFuncionario, // O DTO tem o ID que precisamos!
                                IdPostoTrabalho = posto.IdPostoTrabalho,
                                DtDataServico = dataAtual
                            });
                        }
                    }
                    funcionariosDisponiveis.RemoveAll(f => equipeDoPosto.Select(e => e.IdFuncionario).Contains(f.IdFuncionario));
                }
            }
            else
            {
                // SUA LÓGICA ORIGINAL DE PLANTÃO, AGORA DENTRO DO SERVIÇO, USANDO A LISTA DE DTOs
                var ht = tipoEscala.NrHorasTrabalhada;
                var hf = tipoEscala.NrHorasFolga;
                var pessoaPorPosto = escalaDTO.NrPessoaPorPosto;
                int qtdDias = DateTime.DaysInMonth(anoReferencia, mesReferencia);
                var alasPorPosto = (ht + hf) / 24;
                int ppp_X_TipoEscala = alasPorPosto * pessoaPorPosto;

                List<FuncionarioDTO> funcList = new List<FuncionarioDTO>(listFuncionariosDTO);

                foreach (var posto in listPostos)
                {
                    int countTpEscala = 0;
                    for (int dia = 1; dia <= qtdDias; dia++)
                    {
                        for (int i = 0; i < pessoaPorPosto; i++)
                        {
                            var escalaPronta = new EscalaProntaDTO();
                            string dataStr = $"{dia}-{mesReferencia}-{anoReferencia}";

                            if (countTpEscala >= ppp_X_TipoEscala) countTpEscala = 0;

                            if (funcList.Any() && countTpEscala < funcList.Count)
                            {
                                var funcionario = funcList[countTpEscala];
                                escalaPronta.IdFuncionario = funcionario.IdFuncionario;
                            }
                            countTpEscala++;

                            escalaPronta.IdEscala = escalaDTO.IdEscala;
                            escalaPronta.DtDataServico = Convert.ToDateTime(dataStr).Date;
                            escalaPronta.IdPostoTrabalho = posto.IdPostoTrabalho;
                            listEscalaPronta.Add(escalaPronta);
                        }
                    }
                    if (funcList.Count >= ppp_X_TipoEscala)
                    {
                        funcList.RemoveRange(0, ppp_X_TipoEscala);
                    }
                }
            }

            // --- SALVAMENTO UNIFICADO ---
            if (!listEscalaPronta.Any())
            {
                return new RetornoDTO { valido = false, mensagem = "Nenhum dia de serviço foi gerado." };
            }

            await _escalaProntaService.IncluirLista(_mapper.Map<EscalaProntaDTO[]>(listEscalaPronta));

            escalaDTO.IsGerada = true;
            await _escalaRepository.AtualizarAsync(escalaDTO);

            return new RetornoDTO { valido = true, mensagem = "Escala gerada com sucesso!" };
        }

    }
}
