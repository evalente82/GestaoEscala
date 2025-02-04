using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.EscalaPronta;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaPronta;
using GestaoEscalaPermutas.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
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
                else
                {
                    var escalaProntaExistente = await _context.EscalaPronta
                        .Where(x => x.IdEscala == idEscalaPronta)
                        .OrderBy(x => x.DtDataServico.Date)
                        .ToListAsync();
                    if (escalaProntaExistente == null)
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

                // 🔹 Definindo o ciclo de trabalho e folga
                int cicloTrabalho = Math.Max(1, tipoEscalaAtual.NrHorasTrabalhada / 24);
                int cicloFolga = Math.Max(1, tipoEscalaAtual.NrHorasFolga / 24);

                // ✅ Obtendo todas as escalas prontas associadas à escala antiga
                var escalaProntaAntiga = await _context.EscalaPronta
                    .Where(ep => ep.IdEscala == idEscala)
                    .OrderBy(ep => ep.DtDataServico)
                    .ToListAsync();

                if (!escalaProntaAntiga.Any())
                {
                    throw new Exception("Nenhuma escala pronta encontrada para replicação.");
                }

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

                _context.Escalas.Add(novaEscala);
                await _context.SaveChangesAsync();

                // 🔹 Criando listas para armazenar os dados organizados por posto
                var listaPostos = escalaProntaAntiga
                    .Select(e => e.IdPostoTrabalho)
                    .Distinct()
                    .ToList();

                var listaFuncionariosPorPosto = listaPostos.ToDictionary(
                    posto => posto,
                    posto => escalaProntaAntiga
                        .Where(e => e.IdPostoTrabalho == posto)
                        .OrderBy(e => e.DtDataServico)
                        .Select(e => e.IdFuncionario)
                        .Distinct()
                        .ToList()
                );

                // 🔹 Criando um dicionário para rastrear a posição inicial dos funcionários
                var ordemInicialPorPosto = listaFuncionariosPorPosto
                    .ToDictionary(posto => posto.Key, posto => new List<Guid>(posto.Value));

                // 🔹 Listando todos os dias do próximo mês
                int ano = novaEscala.NrMesReferencia == 1 ? DateTime.UtcNow.Year + 1 : DateTime.UtcNow.Year;
                int mes = novaEscala.NrMesReferencia;
                int totalDias = DateTime.DaysInMonth(ano, mes);
                var diasDoNovoMes = Enumerable.Range(1, totalDias).ToList();

                // ✅ Criando novas escalas prontas para o novo mês, mantendo a ordem correta
                var novaEscalaPronta = new List<EscalaProntaDTO>();

                foreach (var idPosto in listaPostos)
                {
                    var listaFuncionarios = listaFuncionariosPorPosto[idPosto];
                    if (!listaFuncionarios.Any()) continue;

                    int totalFuncionarios = listaFuncionarios.Count;
                    int indiceCiclo = 0;

                    // 🔹 Mantendo a ordem inicial
                    var ordemAtual = new List<Guid>(ordemInicialPorPosto[idPosto]);

                    for (int dia = 1; dia <= totalDias; dia++)
                    {
                        var novaDataServico = new DateTime(ano, mes, dia);

                        // 🔹 Seleciona os funcionários seguindo o ciclo correto e mantendo a ordem
                        var funcionariosDoTurno = ordemAtual.Take(escalaAtual.NrPessoaPorPosto).ToList();

                        foreach (var funcionario in funcionariosDoTurno)
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

                        // 🔹 Rotaciona a lista para o próximo dia, mantendo a ordem original
                        if (ordemAtual.Count >= escalaAtual.NrPessoaPorPosto)
                        {
                            var rotacionados = ordemAtual.Take(escalaAtual.NrPessoaPorPosto).ToList();
                            ordemAtual.RemoveRange(0, escalaAtual.NrPessoaPorPosto);
                            ordemAtual.AddRange(rotacionados);
                        }
                    }
                }

                // 🔹 Debug: Print das listas organizadas
                Console.WriteLine("\n==== Lista de Postos ====");
                foreach (var posto in listaPostos)
                {
                    Console.WriteLine($"Posto: {posto}");
                }

                Console.WriteLine("\n==== Lista de Funcionários por Posto ====");
                foreach (var (posto, funcionarios) in listaFuncionariosPorPosto)
                {
                    Console.WriteLine($"Posto {posto}: {string.Join(", ", funcionarios)}");
                }

                Console.WriteLine("\n==== Nova Escala Pronta ====");
                foreach (var escala in novaEscalaPronta)
                {
                    Console.WriteLine($"Posto: {escala.IdPostoTrabalho}, Func: {escala.IdFuncionario}, Dia: {escala.DtDataServico.Day}");
                }

                // 🔹 Inserindo os dados no banco
                var escalaPronta = _mapper.Map<List<DepInfra.EscalaPronta>>(novaEscalaPronta);
                _context.EscalaPronta.AddRange(escalaPronta);
                await _context.SaveChangesAsync();

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

                _context.EscalaPronta.RemoveRange(ocorrencias);
                await _context.SaveChangesAsync();

                return new EscalaProntaDTO { valido = true, mensagem = "Ocorrências do funcionário removidas com sucesso." };
            }
            catch (Exception e)
            {
                return new EscalaProntaDTO { valido = false, mensagem = $"Erro ao remover ocorrência: {e.Message}" };
            }
        }

        public async Task<EscalaProntaDTO> IncluirFuncionarioEscala(EscalaProntaDTO escalaProntaDTO)
        {
            try
            {
                if (escalaProntaDTO is null)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Dados não preenchidos." };
                }

                // 🔹 Verifica se o posto de trabalho existe
                var postoExiste = await _context.PostoTrabalhos.AnyAsync(p => p.IdPostoTrabalho == escalaProntaDTO.IdPostoTrabalho);
                if (!postoExiste)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Posto de trabalho não encontrado." };
                }

                // 🔹 Obtém a escala para verificar o limite de funcionários por posto
                var escala = await _context.Escalas.FindAsync(escalaProntaDTO.IdEscala);
                if (escala == null)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Escala não encontrada." };
                }

                // 🔹 Conta quantos funcionários já estão no posto nesse dia
                int funcionariosNoPosto = await _context.EscalaPronta
                    .CountAsync(e => e.IdPostoTrabalho == escalaProntaDTO.IdPostoTrabalho && e.DtDataServico.Date == escalaProntaDTO.DtDataServico.Date);

                if (funcionariosNoPosto >= escala.NrPessoaPorPosto)
                {
                    return new EscalaProntaDTO { valido = false, mensagem = "Limite de funcionários para este posto atingido!" };
                }

                var novaEscalaPronta = _mapper.Map<DepInfra.EscalaPronta>(escalaProntaDTO);
                _context.EscalaPronta.Add(novaEscalaPronta);
                await _context.SaveChangesAsync();

                return _mapper.Map<EscalaProntaDTO>(novaEscalaPronta);
            }
            catch (Exception e)
            {
                return new EscalaProntaDTO { valido = false, mensagem = $"Erro ao incluir funcionário: {e.Message}" };
            }
        }
    }
}
