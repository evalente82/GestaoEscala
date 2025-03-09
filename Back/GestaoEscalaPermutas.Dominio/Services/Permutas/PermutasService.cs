using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.Permutas;
using GestaoEscalaPermutas.Dominio.Interfaces.Permutas;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.Permutas
{
    public class PermutasService : IPermutasService
    {
        private readonly IPermutasRepository _permutasRepository;
        private readonly IMapper _mapper;

        public PermutasService(IPermutasRepository permutasRepository, IMapper mapper)
        {
            _permutasRepository = permutasRepository;
            _mapper = mapper;
        }

        public async Task<PermutasDTO> Incluir(PermutasDTO permutasDTO)
        {
            try
            {
                if (permutasDTO is null)
                    return new PermutasDTO { valido = false, mensagem = "Objeto não preenchido." };

                var permuta = _mapper.Map<DepInfra.Permuta>(permutasDTO);
                // Normalizar como UTC (sem .Date, pois já vem correto do mobile)
                permuta.DtDataSolicitadaTroca = DateTime.SpecifyKind(permuta.DtDataSolicitadaTroca, DateTimeKind.Utc);
                permuta.DtSolicitacao = DateTime.SpecifyKind(permuta.DtSolicitacao, DateTimeKind.Utc);

                // Log para verificar o valor antes de gravar
                Console.WriteLine($"DtDataSolicitadaTroca antes de gravar: {permuta.DtDataSolicitadaTroca}");
                var permutaCriada = await _permutasRepository.IncluirAsync(permuta);
                return _mapper.Map<PermutasDTO>(permutaCriada);
            }
            catch (Exception e)
            {
                return new PermutasDTO { valido = false, mensagem = $"Erro ao incluir permuta: {e.Message}" };
            }
        }
        public async Task<PermutasDTO> Alterar(Guid id, PermutasDTO permutaDTO)
        {
            try
            {
                if (id == Guid.Empty)
                    return new PermutasDTO { valido = false, mensagem = "Id fora do Range." };

                var permutaExistente = await _permutasRepository.BuscarPorIdAsync(id);
                if (permutaExistente == null)
                    return new PermutasDTO { valido = false, mensagem = "Permuta não encontrada." };

                _mapper.Map(permutaDTO, permutaExistente);
                var permutaAtualizada = await _permutasRepository.AlterarAsync(permutaExistente);

                return _mapper.Map<PermutasDTO>(permutaAtualizada);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao alterar permuta: {e.Message}");
            }
        }

        public async Task<PermutasDTO> BuscarPorId(Guid idPermuta)
        {
            try
            {
                if (idPermuta == Guid.Empty)
                    return new PermutasDTO { valido = false, mensagem = "Id fora do Range." };

                var permuta = await _permutasRepository.BuscarPorIdAsync(idPermuta);
                return permuta == null
                    ? new PermutasDTO { valido = false, mensagem = "Permuta não encontrada." }
                    : _mapper.Map<PermutasDTO>(permuta);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar permuta: {e.Message}");
            }
        }

        public async Task<List<PermutasDTO>> BuscarTodos()
        {
            try
            {
                var permutas = await _permutasRepository.BuscarTodosAsync();
                return _mapper.Map<List<PermutasDTO>>(permutas);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar todas as permutas: {e.Message}");
            }
        }

        public async Task<PermutasDTO> Deletar(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return new PermutasDTO { valido = false, mensagem = "Id fora do Range." };

                var sucesso = await _permutasRepository.DeletarAsync(id);
                return sucesso
                    ? new PermutasDTO { valido = true, mensagem = "Permuta deletada com sucesso." }
                    : new PermutasDTO { valido = false, mensagem = "Permuta não encontrada." };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao deletar permuta: {e.Message}");
            }
        }

        public async Task<List<PermutasDTO>> BuscarFuncPorId(Guid idFuncionario)
        {
            try
            {
                if (idFuncionario == Guid.Empty)
                    return new List<PermutasDTO> { new PermutasDTO { valido = false, mensagem = "Id fora do Range." } };

                var permutas = await _permutasRepository.BuscarFuncPorIdAsync(idFuncionario);

                if (permutas == null || !permutas.Any())
                {
                    return new List<PermutasDTO> { new PermutasDTO { valido = false, mensagem = "Permutas não encontradas." } };
                }

                return _mapper.Map<List<PermutasDTO>>(permutas);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar permutas: {e.Message}");
            }
        }

        public async Task<List<PermutasDTO>> BuscarSolicitacoesPorId(Guid idFuncionario)
        {
            try
            {
                if (idFuncionario == Guid.Empty)
                    return new List<PermutasDTO> { new PermutasDTO { valido = false, mensagem = "Id fora do Range." } };

                var permutas = await _permutasRepository.BuscaSolicitacoesPorIdAsync(idFuncionario);

                if (permutas == null || !permutas.Any())
                {
                    return new List<PermutasDTO> { new PermutasDTO { valido = false, mensagem = "Permutas não encontradas." } };
                }

                return _mapper.Map<List<PermutasDTO>>(permutas);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar permutas: {e.Message}");
            }
        }

        public async Task<List<PermutasDTO>> BuscarSolicitacoesFuncPorId(Guid idFuncionario)
        {
            try
            {
                if (idFuncionario == Guid.Empty)
                    return new List<PermutasDTO> { new PermutasDTO { valido = false, mensagem = "Id fora do Range." } };

                var permutas = await _permutasRepository.BuscarSolicitacoesFuncPorIdAsync(idFuncionario);

                if (permutas == null || !permutas.Any())
                {
                    return new List<PermutasDTO> { new PermutasDTO { valido = false, mensagem = "Permutas não encontradas." } };
                }

                return _mapper.Map<List<PermutasDTO>>(permutas);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar permutas: {e.Message}");
            }
        }
    }
}