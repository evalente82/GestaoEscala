using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO;
using GestaoEscalaPermutas.Dominio.DTO.EscalaPronta;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaPronta;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Implementations;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.EscalaPronta
{
    public class EscalaProntaService : IEscalaProntaService
    {
        private readonly IEscalaProntaRepository _escalaProntaRepository;
        private readonly IFuncionarioRepository _funcionarioRepository;
        private readonly IEscalaRepository _escalaRepository;
        private readonly ITipoEscalaRepository _tipoEscalaRepository;
        private readonly IMapper _mapper;

        public EscalaProntaService(IEscalaProntaRepository escalaProntaRepository, IMapper mapper, IFuncionarioRepository funcionarioRepository, IEscalaRepository escalaRepository, ICargoRepository cargoRepository, ITipoEscalaRepository tipoEscalaRepository)
        {
            _escalaProntaRepository = escalaProntaRepository;
            _mapper = mapper;
            _funcionarioRepository = funcionarioRepository;
            _escalaRepository = escalaRepository;
            _tipoEscalaRepository = tipoEscalaRepository;
        }

        public async Task<EscalaProntaDTO> Incluir(EscalaProntaDTO escalaProntaDTO)
        {
            if (escalaProntaDTO is null)
                return new EscalaProntaDTO { valido = false, mensagem = "Objeto não preenchido." };

            var escalaPronta = _mapper.Map<DepInfra.EscalaPronta>(escalaProntaDTO);
            var novaEscalaPronta = await _escalaProntaRepository.AdicionarAsync(escalaPronta);
            return _mapper.Map<EscalaProntaDTO>(novaEscalaPronta);
        }

        public async Task<EscalaProntaDTO> Alterar(Guid id, EscalaProntaDTO escalaProntaDTO)
        {
            if (id == Guid.Empty)
                return new EscalaProntaDTO { valido = false, mensagem = "Id fora do Range." };

            var escalaProntaExistente = await _escalaProntaRepository.ObterPorIdAsync(id);
            if (escalaProntaExistente == null)
                return new EscalaProntaDTO { valido = false, mensagem = "Escala não encontrada." };

            _mapper.Map(escalaProntaDTO, escalaProntaExistente);
            await _escalaProntaRepository.AtualizarAsync(escalaProntaExistente);
            return _mapper.Map<EscalaProntaDTO>(escalaProntaExistente);
        }

        public async Task<List<EscalaProntaDTO>> BuscarTodos()
        {
            var escalasProntas = await _escalaProntaRepository.ObterTodosAsync();
            return _mapper.Map<List<EscalaProntaDTO>>(escalasProntas);
        }

        public async Task<List<EscalaProntaDTO>> BuscarPorId(Guid idEscalaPronta)
        {
            if (idEscalaPronta == Guid.Empty)
                return new List<EscalaProntaDTO> { new EscalaProntaDTO { valido = false, mensagem = "Id fora do Range." } };

            var escalasProntas = await _escalaProntaRepository.ObterPorEscalaIdAsync(idEscalaPronta);
            if (!escalasProntas.Any())
                return new List<EscalaProntaDTO> { new EscalaProntaDTO { valido = false, mensagem = "Escala Não encontrada." } };

            return _mapper.Map<List<EscalaProntaDTO>>(escalasProntas);
        }

        public async Task<EscalaProntaDTO> Deletar(Guid id)
        {
            if (id == Guid.Empty)
                return new EscalaProntaDTO { valido = false, mensagem = "Id fora do Range." };

            await _escalaProntaRepository.RemoverAsync(id);
            return new EscalaProntaDTO { valido = true, mensagem = "Escala deletada com sucesso." };
        }

        public async Task<EscalaProntaDTO[]> IncluirLista(EscalaProntaDTO[] escalaProntaDTOs)
        {
            if (escalaProntaDTOs is null || escalaProntaDTOs.Length == 0)
                return new EscalaProntaDTO[] { new() { valido = false, mensagem = "Lista de escala vazia." } };

            var escalasProntas = _mapper.Map<DepInfra.EscalaPronta[]>(escalaProntaDTOs);
            var novasEscalas = await _escalaProntaRepository.AdicionarListaAsync(escalasProntas);
            return _mapper.Map<EscalaProntaDTO[]>(novasEscalas);
        }

        public async Task<EscalaProntaDTO[]> AlterarEscalaPronta(Guid idEscala, EscalaProntaDTO[] escalaProntaDTOs)
        {
            if (escalaProntaDTOs == null || escalaProntaDTOs.Length == 0)
                return new EscalaProntaDTO[] { new() { valido = false, mensagem = "Lista de Escala vazia." } };

            await _escalaProntaRepository.RemoverListaPorEscalaAsync(idEscala);
            var novasEscalas = _mapper.Map<DepInfra.EscalaPronta[]>(escalaProntaDTOs);
            await _escalaProntaRepository.AdicionarListaAsync(novasEscalas);
            return _mapper.Map<EscalaProntaDTO[]>(novasEscalas);
        }

        public async Task<RetornoDTO> RecriarEscalaProximoMes(Guid idEscala)
        {
            using var transaction = await _escalaRepository.IniciarTransacaoAsync();

            try
            {
                var escalaAtual = await _escalaRepository.ObterPorIdAsync(idEscala);
                if (escalaAtual == null)
                    return new RetornoDTO { valido = false, mensagem = "Escala não encontrada!" };

                var tipoEscalaAtual = await _tipoEscalaRepository.BuscarPorIdAsync(escalaAtual.IdTipoEscala);
                if (tipoEscalaAtual == null)
                    throw new Exception("Tipo de escala não encontrado!");

                var escalaProntaAntiga = await _escalaProntaRepository.ObterPorEscalaIdAsync(idEscala);
                if (!escalaProntaAntiga.Any())
                    throw new Exception("Nenhuma escala pronta encontrada para replicação.");

                var listaPostos = escalaProntaAntiga
                    .Select(e => e.IdPostoTrabalho)
                    .Distinct()
                    .ToList();

                // Criar um dicionário com os grupos únicos de funcionários por posto
                var listIndiceFunc = new Dictionary<Guid, List<List<Guid>>>();

                foreach (var posto in listaPostos)
                {
                    var gruposUnicos = new Dictionary<string, List<Guid>>();
                    var diasOrdenados = escalaProntaAntiga
                        .Where(e => e.IdPostoTrabalho == posto)
                        .GroupBy(e => e.DtDataServico)
                        .OrderBy(g => g.Key);

                    foreach (var dia in diasOrdenados)
                    {
                        var funcionarios = dia.Select(e => e.IdFuncionario).OrderBy(f => f).ToList();
                        var chaveGrupo = string.Join("-", funcionarios);
                        if (!gruposUnicos.ContainsKey(chaveGrupo))
                            gruposUnicos[chaveGrupo] = funcionarios;
                    }

                    listIndiceFunc[posto] = gruposUnicos.Values.ToList(); // Lista ordenada de grupos únicos
                }

                // Identificar o último grupo usado por posto
                var ultimosFuncionarios = listaPostos.ToDictionary(
                    posto => posto,
                    posto =>
                    {
                        var ultimoDia = escalaProntaAntiga
                            .Where(e => e.IdPostoTrabalho == posto)
                            .OrderByDescending(e => e.DtDataServico)
                            .FirstOrDefault()?.DtDataServico;

                        if (ultimoDia == null)
                            return (indice: 0, funcionarios: listIndiceFunc[posto][0]);

                        var funcionariosUltimoDia = escalaProntaAntiga
                            .Where(e => e.IdPostoTrabalho == posto && e.DtDataServico == ultimoDia)
                            .Select(e => e.IdFuncionario)
                            .OrderBy(f => f)
                            .ToList();

                        var indiceGrupo = listIndiceFunc[posto]
                            .FindIndex(grupo => grupo.OrderBy(f => f).SequenceEqual(funcionariosUltimoDia));

                        return (indice: indiceGrupo, funcionarios: listIndiceFunc[posto][indiceGrupo]);
                    }
                );

                // Criar nova escala
                var novaEscala = new DepInfra.Escala
                {
                    IdEscala = Guid.NewGuid(),
                    IdDepartamento = escalaAtual.IdDepartamento,
                    IdTipoEscala = escalaAtual.IdTipoEscala,
                    IdCargo = escalaAtual.IdCargo,
                    NmNomeEscala = $"{escalaAtual.NmNomeEscala} - {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((escalaAtual.NrMesReferencia % 12) + 1).ToUpper()}",
                    DtCriacao = DateTime.UtcNow,
                    NrMesReferencia = (escalaAtual.NrMesReferencia % 12) + 1,
                    IsAtivo = true,
                    IsGerada = true,
                    NrPessoaPorPosto = escalaAtual.NrPessoaPorPosto,
                };

                novaEscala.NmNomeEscala = AtualizarNomeEscala(novaEscala.NmNomeEscala);
                await _escalaRepository.AdicionarAsync(novaEscala);

                int ano = novaEscala.NrMesReferencia == 1 ? DateTime.UtcNow.Year + 1 : DateTime.UtcNow.Year;
                int mes = novaEscala.NrMesReferencia;
                int totalDias = DateTime.DaysInMonth(ano, mes);

                var novaEscalaPronta = new List<EscalaProntaDTO>();

                foreach (var idPosto in listaPostos)
                {
                    if (!ultimosFuncionarios.ContainsKey(idPosto)) continue;

                    var (indiceUltimoGrupo, _) = ultimosFuncionarios[idPosto];
                    var grupos = listIndiceFunc[idPosto];
                    var totalGrupos = grupos.Count;
                    var indiceAtual = (indiceUltimoGrupo + 1) % totalGrupos; // Próximo grupo após o último

                    for (int dia = 1; dia <= totalDias; dia++)
                    {
                        var novaDataServico = new DateTime(ano, mes, dia);
                        var listaFuncionarios = grupos[indiceAtual];

                        foreach (var funcionario in listaFuncionarios)
                        {
                            novaEscalaPronta.Add(new EscalaProntaDTO
                            {
                                IdEscalaPronta = Guid.NewGuid(),
                                IdEscala = novaEscala.IdEscala,
                                IdPostoTrabalho = idPosto,
                                IdFuncionario = funcionario,
                                DtDataServico = novaDataServico,
                                DtCriacao = DateTime.UtcNow
                            });
                        }

                        indiceAtual = (indiceAtual + 1) % totalGrupos; // Avança para o próximo grupo
                    }
                }

                var escalaPronta = _mapper.Map<List<DepInfra.EscalaPronta>>(novaEscalaPronta);
                await _escalaProntaRepository.AdicionarEmLoteAsync(escalaPronta);

                await transaction.CommitAsync();
                return new RetornoDTO { valido = true, mensagem = "Escala recriada com sucesso!" };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new RetornoDTO { valido = false, mensagem = $"Erro ao recriar escala: {ex.Message}" };
            }
        }
        private string AtualizarNomeEscala(string nomeEscala)
        {
            string[] meses = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
                .Where(m => !string.IsNullOrEmpty(m))
                .Select(m => m.ToUpper())
                .ToArray();

            string padraoMeses = string.Join("|", meses);
            var matches = Regex.Matches(nomeEscala, padraoMeses);

            if (matches.Count > 1)
            {
                string ultimoMes = matches[^1].Value;
                nomeEscala = Regex.Replace(nomeEscala, $@"\s*-\s*({padraoMeses})", "").Trim();
                nomeEscala += " - " + ultimoMes;
            }

            return nomeEscala;
        }

        public async Task<EscalaProntaDTO> DeletarOcorrenciaFuncionario(Guid idFuncionario, Guid idEscala)
        {
            if (idFuncionario == Guid.Empty || idEscala == Guid.Empty)
                return new EscalaProntaDTO { valido = false, mensagem = "Id inválido!" };

            var ocorrencias = await _escalaProntaRepository.ObterPorEscalaIdAsync(idEscala);
            ocorrencias = ocorrencias.Where(ep => ep.IdFuncionario == idFuncionario).ToList();

            if (!ocorrencias.Any())
                return new EscalaProntaDTO { valido = false, mensagem = "Nenhuma ocorrência encontrada para o funcionário." };

            foreach (var ocorrencia in ocorrencias)
            {
                ocorrencia.IdFuncionario = Guid.Empty;
                await _escalaProntaRepository.AtualizarAsync(ocorrencia);
            }

            return new EscalaProntaDTO { valido = true, mensagem = "Ocorrências do funcionário atualizadas com sucesso." };
        }

        public async Task<EscalaProntaDTO> IncluirFuncionarioEcala(EscalaProntaDTO incluiFuncEscalaProntaDTO)
        {
            if (incluiFuncEscalaProntaDTO is null)
                return new EscalaProntaDTO { valido = false, mensagem = "Dados não preenchidos." };

            var escalas = await _escalaProntaRepository.ObterPorEscalaIdAsync(incluiFuncEscalaProntaDTO.IdEscala);
            if (escalas == null || !escalas.Any())
                return new EscalaProntaDTO { valido = false, mensagem = "Escala não encontrada." };

            var funcionarioJaExiste = escalas.Any(x => x.IdFuncionario == incluiFuncEscalaProntaDTO.IdFuncionario);
            if (funcionarioJaExiste)
                return new EscalaProntaDTO { valido = false, mensagem = "Funcionário já existe na Escala." };

            var funcionarioSemVaga = escalas
                .Where(e => e.IdPostoTrabalho == incluiFuncEscalaProntaDTO.IdPostoTrabalho)
                .FirstOrDefault(e => e.IdFuncionario == Guid.Empty);

            if (funcionarioSemVaga != null)
            {
                funcionarioSemVaga.IdFuncionario = incluiFuncEscalaProntaDTO.IdFuncionario;
                await _escalaProntaRepository.AtualizarAsync(funcionarioSemVaga);
                return new EscalaProntaDTO { valido = true, mensagem = "Funcionário incluído na escala com sucesso!" };
            }

            return new EscalaProntaDTO { valido = false, mensagem = "Nenhuma vaga disponível na escala." };
        }

        public async Task<List<EscalaProntaDTO>> BuscarPorIdFuncionario(Guid idFuncionario)
        {
            if (idFuncionario == Guid.Empty)
                return new List<EscalaProntaDTO> { new EscalaProntaDTO { valido = false, mensagem = "IdFuncionario inválido." } };

            var escalasProntas = await _escalaProntaRepository.BuscarPorIdFuncionario(idFuncionario);

            var listIdEscala = escalasProntas.Select(e => e.IdEscala).Distinct().ToList();
            var escalaTodas = await _escalaRepository.ObterTodasAsync();

            var listIdEscalaAtiva = escalaTodas
                .Where(e => e.IsAtivo)
                .Join(listIdEscala,
                      escala => escala.IdEscala,
                      id => id,
                      (escala, id) => id)
                .ToList();

            if (!escalasProntas.Any())
                return new List<EscalaProntaDTO> { new EscalaProntaDTO { valido = false, mensagem = "Nenhum dado encontrado para o funcionário." } };

            return escalasProntas
                .Where(ep => listIdEscalaAtiva.Contains(ep.IdEscala))
                .Select(ep => new EscalaProntaDTO
                {
                    DtDataServico = ep.DtDataServico,
                    IdEscala = ep.IdEscala,
                    NmNomeEscala = ep.Escala?.NmNomeEscala ?? "Sem Nome" // Evita erro caso Escala seja null
                })
                .ToList();
        }

        public async Task<EscalaProntaDTO> IncluirFuncionarioEscala(EscalaProntaDTO escalaProntaDTO)
        {
            if (escalaProntaDTO is null)
                return new EscalaProntaDTO { valido = false, mensagem = "Objeto não preenchido." };

            // 🔹 Verificar se o funcionário já existe na escala para a mesma data e posto de trabalho
            var existeFuncionarioNaEscala = await BuscarPorIdFuncionario(escalaProntaDTO.IdFuncionario);
            var buscarFuncionarioNaEscala = existeFuncionarioNaEscala?
                .Where(x => x.IdEscala == escalaProntaDTO.IdEscala)
                .FirstOrDefault();

            if (buscarFuncionarioNaEscala is not null)
            {
                return new EscalaProntaDTO { valido = false, mensagem = "Funcionário já está escalado." };
            }

            // 🔹 Buscar o funcionário e verificar se ele pertence ao cargo correto para esta escala
            var funcionarioExistente = await _funcionarioRepository.ObterPorIdAsync(escalaProntaDTO.IdFuncionario);
            var escalaPronta = await _escalaProntaRepository.ObterPorEscalaIdAsync(escalaProntaDTO.IdEscala);
            var escala = await _escalaRepository.ObterPorIdAsync(escalaProntaDTO.IdEscala);

            if (funcionarioExistente.IdCargo != escala.IdCargo)
            {
                return new EscalaProntaDTO { valido = false, mensagem = "Funcionário não pertence ao cargo necessário para esta escala." };
            }

            // 🔹 Verificar a quantidade de funcionários já escalados no posto para esta data
            var limitePorPosto = escala.NrPessoaPorPosto;

            // Buscar quantos funcionários já estão escalados para o mesmo posto e data
            var quantidadeFuncionariosNoPosto = escalaPronta
                .Where(x => x.IdPostoTrabalho == escalaProntaDTO.IdPostoTrabalho
                         && x.DtDataServico.Date == escalaProntaDTO.DtDataServico.Date
                         && x.IdFuncionario != Guid.Empty) // Exclui funcionário com ID zerado
                .Select(x => x.IdFuncionario)
                .Distinct()
                .Count();


            if (quantidadeFuncionariosNoPosto > 0 && quantidadeFuncionariosNoPosto >= limitePorPosto)
            {
                return new EscalaProntaDTO { valido = false, mensagem = "Limite de funcionários atingido para este posto nesta data." };
            }

            // 🔹 Buscar outro funcionário de outro posto no mesmo dia e listar os dias que ele trabalha
            var funcionarioDisponivel = escalaPronta
                .Where(x => x.DtDataServico.Date == escalaProntaDTO.DtDataServico.Date
                         && x.IdPostoTrabalho != escalaProntaDTO.IdPostoTrabalho
                         && x.IdFuncionario != Guid.Empty) // Exclui funcionário com ID zerado
                .FirstOrDefault();


            if (funcionarioDisponivel is null)
            {
                return new EscalaProntaDTO { valido = false, mensagem = "Nenhum funcionário encontrado em outro posto para seguir o padrão." };
            }

            // 🔹 Listar os dias que esse funcionário trabalha na escala
            var diasDeTrabalho = escalaPronta
                .Where(x => x.IdFuncionario == funcionarioDisponivel.IdFuncionario)
                .Select(x => x.DtDataServico)
                .ToList();

            // 🔹 Remover todos os registros dos dias que o funcionário já trabalha
            foreach (var dia in diasDeTrabalho)
            {
                var ocorrenciaParaRemover = await _escalaProntaRepository.ObterPorDataEPostoAsync(dia, escalaProntaDTO.IdPostoTrabalho);

                if (ocorrenciaParaRemover != null)
                {
                    await _escalaProntaRepository.RemoverAsync(ocorrenciaParaRemover.IdEscalaPronta);
                }
            }

            // 🔹 Inserir os novos registros para os dias desejados
            foreach (var dia in diasDeTrabalho)
            {
                var novaEscala = new DepInfra.EscalaPronta
                {
                    IdEscala = escalaProntaDTO.IdEscala,
                    IdPostoTrabalho = escalaProntaDTO.IdPostoTrabalho,
                    IdFuncionario = escalaProntaDTO.IdFuncionario,
                    DtDataServico = dia,
                };
                await _escalaProntaRepository.AdicionarAsync(novaEscala);
            }
            return new EscalaProntaDTO { valido = true, mensagem = "Funcionário incluído com sucesso!", IdFuncionario = escalaProntaDTO.IdFuncionario };
        }
    }
}