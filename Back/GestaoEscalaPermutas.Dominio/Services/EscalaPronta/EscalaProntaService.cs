using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.EscalaPronta;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaPronta;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
namespace GestaoEscalaPermutas.Dominio.Services.EscalaPronta
{
    public class EscalaProntaService : IEscalaProntaService
    {
        private readonly DefesaCivilMaricaContext _context;
        private readonly IMapper _mapper;
        public EscalaProntaService(DefesaCivilMaricaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<EscalaProntaDTO> Incluir(EscalaProntaDTO escalaProntaDTO)
        {
            try
            {
                if (escalaProntaDTO is null)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Objeto não preenchido." };
                }
                else
                {
                    var escalaPronta = _mapper.Map<DepInfra.EscalaPronta>(escalaProntaDTO);

                    _context.EscalaPronta.Add(escalaPronta);
                    await _context.SaveChangesAsync();
                    return _mapper.Map<EscalaProntaDTO>(escalaPronta);
                }
            }
            catch (Exception e)
            {
                return new EscalaProntaDTO { valido = false, mensagem = $"Erro ao receber o Objeto: {e.Message}" };
            }
        }
        public async Task<EscalaProntaDTO> Alterar(Guid id, EscalaProntaDTO escalaProntaDTO)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var escalaProntaExistente = await _context.EscalaPronta.FindAsync(id);
                    if (escalaProntaExistente == null)
                    {
                        return new EscalaProntaDTO { valido = false, mensagem = "escala não encontrado." };
                    }

                    // Mapeia os dados do DTO para o modelo existente (apenas as propriedades que você deseja atualizar)
                    _mapper.Map(escalaProntaDTO, escalaProntaExistente);

                    // O EF Core rastreará que o objeto foi modificado
                    _context.EscalaPronta.Update(escalaProntaExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    // Retorna o DTO atualizado (opcionalmente, você pode mapear de volta se quiser devolver os dados atualizados)
                    return _mapper.Map<EscalaProntaDTO>(escalaProntaExistente);
                }
            }
            catch (Exception e)
            {
                // Considerar usar um logger para registrar a exceção
                throw new Exception($"Erro ao alterar o objeto: {e.Message}");
            }
        }
        public Task<List<EscalaProntaDTO>> BuscarTodos()
        {
            throw new NotImplementedException();
        }
        public async Task<List<EscalaProntaDTO>> BuscarPorId(Guid idEscalaPronta)
        {
            try
            {
                if (idEscalaPronta == Guid.Empty)
                {
                    return new List<EscalaProntaDTO>
            {
                new EscalaProntaDTO
                {
                    valido = false,
                    mensagem = "Id fora do Range."
                }
            };
                }

                var escalaProntaExistente = await _context.EscalaPronta
                    .Where(x => x.IdEscala == idEscalaPronta)
                    .OrderBy(x => x.DtDataServico)  // 🔹 Ordena primeiro pela Data
                    .ThenBy(x => x.IdPostoTrabalho) // 🔹 Depois pelo Posto de Trabalho
                    .ThenBy(x => x.IdFuncionario)   // 🔹 Por último, pelo Funcionário (mantendo a ordem de inserção)
                    .ToListAsync();

                if (!escalaProntaExistente.Any())
                {
                    return new List<EscalaProntaDTO>
            {
                new EscalaProntaDTO
                {
                    valido = false,
                    mensagem = "Escala Não encontrada."
                }
            };
                }

                return _mapper.Map<List<EscalaProntaDTO>>(escalaProntaExistente);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }

        public async Task<EscalaProntaDTO> Deletar(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var escalaProntaExistente = await _context.EscalaPronta.FindAsync(id);
                    if (escalaProntaExistente == null)
                    {
                        return new EscalaProntaDTO { valido = false, mensagem = "Escala não encontrado." };
                    }


                    // O EF Core rastreará que o objeto foi modificado
                    _context.EscalaPronta.Remove(escalaProntaExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    //retornar avido de deletado
                    return new EscalaProntaDTO { valido = true, mensagem = "Escala deletado com sucesso." };
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }

        public async Task<EscalaProntaDTO[]> IncluirLista(EscalaProntaDTO[] escalaProntaDTOs)
        {
            try
            {

                if (escalaProntaDTOs is null)
                {
                    return new EscalaProntaDTO[] {
                        new EscalaProntaDTO {
                            valido = false, mensagem = "Lista de escala vazia."
                        }
                    };
                }
                else
                {
                    var escalaPronta = _mapper.Map<DepInfra.EscalaPronta[]>(escalaProntaDTOs);

                    _context.EscalaPronta.AddRange(escalaPronta);
                    await _context.SaveChangesAsync();

                    return _mapper.Map<EscalaProntaDTO[]>(escalaPronta);
                }
            }
            catch (Exception e)
            {
                return new EscalaProntaDTO[] { new EscalaProntaDTO { valido = false, mensagem = $"Erro ao incluir a lista de Escala: {e.Message}" } };
            }
        }

        public async Task<EscalaProntaDTO[]> AlterarEscalaPronta(Guid idEscala, EscalaProntaDTO[] escalaProntaDTOs)
        {
            try
            {
                // Validação básica
                if (escalaProntaDTOs == null || escalaProntaDTOs.Length == 0)
                {
                    return new EscalaProntaDTO[]
                    {
                        new EscalaProntaDTO
                        {
                            valido = false,
                            mensagem = "Lista de Escala vazia."
                        }
                    };
                }

                // Inicia a transação no banco de dados
                using var transaction = await _context.Database.BeginTransactionAsync();

                // Remove os registros antigos de EscalaPronta com o idEscala fornecido
                var registrosExistentes = _context.EscalaPronta.Where(e => e.IdEscala == idEscala);
                _context.EscalaPronta.RemoveRange(registrosExistentes);

                // Mapeia os novos dados de EscalaProntaDTO para as entidades do banco
                var novasEscalas = _mapper.Map<DepInfra.EscalaPronta[]>(escalaProntaDTOs);

                // Insere os novos registros
                await _context.EscalaPronta.AddRangeAsync(novasEscalas);

                // Salva as alterações no banco
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Retorna os registros salvos como DTO
                return _mapper.Map<EscalaProntaDTO[]>(novasEscalas);
            }
            catch (Exception e)
            {
                return new EscalaProntaDTO[]
                {
                    new EscalaProntaDTO
                    {
                        valido = false,
                        mensagem = $"Erro ao salvar a escala alterada: {e.Message}"
                    }
                };
            }
        }

        public async Task<RetornoDTO> RecriarEscalaProximoMes(Guid idEscala)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(); // 🔹 Inicia a transação

            try
            {
                var escalaAtual = await _context.Escalas.FindAsync(idEscala);
                if (escalaAtual == null)
                {
                    return new RetornoDTO { valido = false, mensagem = "Escala não encontrada!" };
                }

                // ✅ Obtendo informações sobre o tipo de escala
                var tipoEscalaAtual = await _context.TipoEscalas.FindAsync(escalaAtual.IdTipoEscala);
                if (tipoEscalaAtual == null)
                {
                    throw new Exception("Tipo de escala não encontrado!");
                }                

                // ✅ Obtendo todas as escalas prontas associadas à escala antiga
                var escalaProntaAntiga = await _context.EscalaPronta
                    .Where(ep => ep.IdEscala == idEscala)
                    .OrderBy(ep => ep.DtDataServico)
                    .ToListAsync();

                // 🔹 Criando lista de postos de trabalho presentes na escala antiga
                var listaPostos = escalaProntaAntiga
                    .Select(e => e.IdPostoTrabalho)
                    .Distinct()
                    .ToList();

                if (!escalaProntaAntiga.Any())
                {
                    throw new Exception("Nenhuma escala pronta encontrada para replicação.");
                }

                // 🔹 Obtendo a lista de funcionários por dia e posto
                var dataPostosFuncionarios = escalaProntaAntiga
                    .GroupBy(e => new { e.DtDataServico, e.IdPostoTrabalho }) // Agrupa por Data e Posto
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderBy(e => e.DtDataServico) // Mantém a ordem correta dos funcionários no dia
                              .Select(e => e.IdFuncionario)
                              .ToList()
                    );

                // 🔹 Criando um índice para rastrear a rotação dos funcionários por posto
                var listIndiceFunc = new Dictionary<Guid, Dictionary<int, List<Guid>>>();

                foreach (var posto in listaPostos)
                {
                    var gruposUnicos = new Dictionary<string, int>();
                    var indiceAtual = 1;
                    var ordemFuncionarios = new Dictionary<int, List<Guid>>();

                    foreach (var (dataPosto, listaFuncionarios) in dataPostosFuncionarios.Where(d => d.Key.IdPostoTrabalho == posto))
                    {
                        var chaveGrupo = string.Join("-", listaFuncionarios.OrderBy(f => f)); // Cria uma chave única para o grupo

                        if (!gruposUnicos.ContainsKey(chaveGrupo))
                        {
                            gruposUnicos[chaveGrupo] = indiceAtual;
                            indiceAtual++;
                        }

                        var indiceGrupo = gruposUnicos[chaveGrupo];

                        if (!ordemFuncionarios.ContainsKey(indiceGrupo))
                        {
                            ordemFuncionarios[indiceGrupo] = new List<Guid>(listaFuncionarios); // Copia a lista para manter a ordem
                        }
                    }

                    listIndiceFunc[posto] = ordemFuncionarios;
                }

                // 🔹 Obtendo os últimos funcionários escalados em cada posto
                var ultimosFuncionarios = listaPostos.ToDictionary(
                    posto => posto,
                    posto =>
                    {
                        var ultimoDia = escalaProntaAntiga
                            .Where(e => e.IdPostoTrabalho == posto)
                            .OrderByDescending(e => e.DtDataServico)
                            .FirstOrDefault()?.DtDataServico;

                        if (ultimoDia == null)
                            return (indice: 1, funcionarios: new List<Guid>()); // Se não houver dados, começa com índice 1

                        var funcionariosUltimoDia = escalaProntaAntiga
                            .Where(e => e.IdPostoTrabalho == posto && e.DtDataServico == ultimoDia)
                            .Select(e => e.IdFuncionario)
                            .OrderBy(f => f)
                            .ToList();

                        // Encontrar qual índice corresponde a esse grupo de funcionários
                        var indiceGrupo = listIndiceFunc[posto]
                            .FirstOrDefault(kv => kv.Value.OrderBy(f => f).SequenceEqual(funcionariosUltimoDia))
                            .Key;

                        // Se não encontrou um índice correspondente, começa do primeiro
                        if (indiceGrupo == 0)
                            indiceGrupo = 1;

                        return (indice: indiceGrupo, funcionarios: listIndiceFunc[posto][indiceGrupo]);
                    }
                );

                // ✅ Criando nova escala para o próximo mês
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
                // Lista de meses em português (ajustável para outros idiomas se necessário)
                string[] meses = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
                    .Where(m => !string.IsNullOrEmpty(m))
                    .Select(m => m.ToUpper())
                    .ToArray();

                // Expressão regular para encontrar os meses na string (caso venham separados por "-")
                string padraoMeses = string.Join("|", meses);
                //novaEscala.NmNomeEscala = "2 x 1 Praia - ABRIL - MAIO";

                // Encontrar todos os meses na string
                var matches = Regex.Matches(novaEscala.NmNomeEscala, padraoMeses);

                if (matches.Count > 1) // Se houver mais de um mês, remover os anteriores
                {
                    // Pegamos o último mês encontrado
                    string ultimoMes = matches[^1].Value;

                    // Removemos todos os meses anteriores
                    novaEscala.NmNomeEscala = Regex.Replace(novaEscala.NmNomeEscala, $@"\s*-\s*({padraoMeses})", "").Trim();

                    // Adicionamos apenas o último mês ao final da string
                    novaEscala.NmNomeEscala += " - " + ultimoMes;

                }

                _context.Escalas.Add(novaEscala);
                await _context.SaveChangesAsync();

                int ano = novaEscala.NrMesReferencia == 1 ? DateTime.UtcNow.Year + 1 : DateTime.UtcNow.Year;
                int mes = novaEscala.NrMesReferencia;
                int totalDias = DateTime.DaysInMonth(ano, mes);

                var novaEscalaPronta = new List<EscalaProntaDTO>();

                foreach (var idPosto in listaPostos)
                {
                    if (!ultimosFuncionarios.ContainsKey(idPosto)) continue;

                    var (indiceAtual, listaFuncionarios) = ultimosFuncionarios[idPosto];

                    for (int dia = 1; dia <= totalDias; dia++)
                    {
                        var novaDataServico = new DateTime(ano, mes, dia);

                        // 🔹 GARANTE QUE OS FUNCIONÁRIOS SE MANTENHAM NA MESMA ORDEM
                        var listaOrdenada = listaFuncionarios.OrderBy(f => f).ToList();

                        foreach (var funcionario in listaOrdenada)
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
                        // Avança para o próximo índice mantendo a ordem original do grupo
                        indiceAtual = indiceAtual % listIndiceFunc[idPosto].Count + 1;
                        listaFuncionarios = new List<Guid>(listIndiceFunc[idPosto][indiceAtual]);
                    }
                }

                var escalaPronta = _mapper.Map<List<DepInfra.EscalaPronta>>(novaEscalaPronta);

                foreach(var escala in escalaPronta)
{
                    _context.EscalaPronta.Add(escala);
                    await _context.SaveChangesAsync(); // 🔹 Força a inserção imediata, preservando a ordem
                }

                await transaction.CommitAsync(); // 🔹 Confirma a transação se tudo der certo

                return new RetornoDTO { valido = true, mensagem = "Escala recriada com sucesso!" };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // 🔹 Reverte todas as mudanças no banco caso ocorra um erro

                return new RetornoDTO { valido = false, mensagem = $"Erro ao recriar escala: {ex.Message}" };
            }
        }

        public async Task<EscalaProntaDTO> DeletarOcorrenciaFuncionario(Guid idFuncionario, Guid idEscala)
        {
            try
            {
                if (idFuncionario == Guid.Empty || idEscala == Guid.Empty)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Id inválido!" };
                }

                // 🔹 Obtendo todas as ocorrências do funcionário na escala específica
                var ocorrencias = await _context.EscalaPronta
                    .Where(ep => ep.IdFuncionario == idFuncionario && ep.IdEscala == idEscala)
                    .ToListAsync();

                if (!ocorrencias.Any())
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Nenhuma ocorrência encontrada para o funcionário." };
                }

                // 🔹 Atualizando o campo IdFuncionario para Guid.Empty
                foreach (var ocorrencia in ocorrencias)
                {
                    ocorrencia.IdFuncionario = Guid.Empty;
                }

                await _context.SaveChangesAsync();

                return new EscalaProntaDTO { valido = true, 
                    mensagem = "Ocorrências do funcionário atualizadas com sucesso." };
            }
            catch (Exception e)
            {
                return new EscalaProntaDTO { valido = false, mensagem = $"Erro ao atualizar ocorrência: {e.Message}" };
            }
        }

        public async Task<EscalaProntaDTO> IncluirFuncionarioEscala(EscalaProntaDTO incluiFuncEscalaProntaDTO)
        {
            try
            {
                if (incluiFuncEscalaProntaDTO is null)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Dados não preenchidos." };
                }

                

                var escala = await _context.Escalas.Where(x => x.IdEscala == incluiFuncEscalaProntaDTO.IdEscala).FirstOrDefaultAsync();
                var escalaPronta = await _context.EscalaPronta.Where(x => x.IdEscala == incluiFuncEscalaProntaDTO.IdEscala).ToListAsync();
                // 🔹 Obtém a escala para verificar o limite de funcionários por posto
                if (escalaPronta == null || escala == null)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Escala não encontrada." };
                }

                var funcionarioValido = await _context.Funcionarios.Where(x => x.IdFuncionario == incluiFuncEscalaProntaDTO.IdFuncionario).FirstOrDefaultAsync();
                if (funcionarioValido.IdCargo != escala.IdCargo)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Funcionário com cargo diferente do permitido para a escala." };
                }

                // 🔹 Verifica se o posto de trabalho existe
                var postoExiste = escalaPronta.Any(p => p.IdPostoTrabalho == incluiFuncEscalaProntaDTO.IdPostoTrabalho);
                if (!postoExiste)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Posto de trabalho não encontrado." };
                }                

                // 🔹 Buscar um funcionário escalado no mesmo dia em qualquer outro posto
                var referencia = escalaPronta
                    .Where(e => e.IdEscala == incluiFuncEscalaProntaDTO.IdEscala && e.DtDataServico.Date == incluiFuncEscalaProntaDTO.DtDataServico.Date )
                    .FirstOrDefault();

                if (referencia == null)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Nenhum funcionário encontrado em outro posto para seguir o padrão." };
                }

                var funcExistente = escalaPronta
                    .Where(x => x.IdEscala == incluiFuncEscalaProntaDTO.IdEscala && x.IdFuncionario == incluiFuncEscalaProntaDTO.IdFuncionario)
                    .FirstOrDefault();

                if (funcExistente != null)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Funcionário já existe na Escala." };
                }

                // 🔹 Identificar os dias em que esse funcionário trabalha
                var diasTrabalho = escalaPronta
                    .Where(e => e.IdEscala == incluiFuncEscalaProntaDTO.IdEscala 
                            && e.IdFuncionario == referencia.IdFuncionario)
                    .Select(e => e.DtDataServico.Date)
                    .OrderBy(d => d)
                    .ToList();

                if (!diasTrabalho.Any())
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Não foi possível identificar um padrão de escala." };
                }

                List<DepInfra.EscalaPronta> novasEscalas = new();
                foreach (var data in diasTrabalho)
                {
                    // 🔹 Obtém os funcionários escalados para o posto neste dia
                    var funcionariosNoPosto = escalaPronta
                        .Where(e => e.IdPostoTrabalho == incluiFuncEscalaProntaDTO.IdPostoTrabalho && e.DtDataServico.Date == data)
                        .ToList();

                    // 🔹 Conta quantos funcionários já estão no posto, excluindo os IDs zerados (Guid.Empty)
                    int qtdFuncionariosNoPosto = funcionariosNoPosto.Count(e => e.IdFuncionario != Guid.Empty);

                    // 🔹 Verifica se o posto já atingiu o limite permitido
                    if (qtdFuncionariosNoPosto >= escala.NrPessoaPorPosto)
                    {
                        return new EscalaProntaDTO { valido = false, mensagem = "Quantidade máxima de funcioários atingida por dia neste posto." };
                    }

                    // 🔹 Se houver um funcionário com ID zerado, substituir ele pelo novo funcionário
                    var vagaDisponivel = funcionariosNoPosto.FirstOrDefault(e => e.IdFuncionario == Guid.Empty);
                    if (vagaDisponivel != null)
                    {
                        vagaDisponivel.IdFuncionario = incluiFuncEscalaProntaDTO.IdFuncionario;
                        novasEscalas.Add(vagaDisponivel);
                    }
                }

                if (novasEscalas.Any())
                {
                    foreach (var escal in novasEscalas)
                    {
                        _context.EscalaPronta.Update(escal); // ✅ Atualiza cada registro individualmente
                    }

                    await _context.SaveChangesAsync(); // 🔹 Salva todas as alterações no banco
                }

                return new EscalaProntaDTO { valido = true, mensagem = "Funcionário incluído na escala seguindo o padrão existente em outro posto!" };
            }
            catch (Exception e)
            {
                return new EscalaProntaDTO { valido = false, mensagem = $"Erro ao incluir funcionário: {e.Message}" };
            }
        }

    }
}
