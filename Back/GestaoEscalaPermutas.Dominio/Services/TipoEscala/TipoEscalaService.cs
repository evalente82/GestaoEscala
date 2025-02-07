using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.DTO.TipoEscala;
using GestaoEscalaPermutas.Dominio.Interfaces.TipoEscala;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.TipoEscala
{
    namespace GestaoEscalaPermutas.Dominio.Services
    {
        public class TipoEscalaService : ITipoEscalaService
        {
            private readonly ITipoEscalaRepository _tipoEscalaRepository;
            private readonly IMapper _mapper;

            public TipoEscalaService(ITipoEscalaRepository tipoEscalaRepository, IMapper mapper)
            {
                _tipoEscalaRepository = tipoEscalaRepository;
                _mapper = mapper;
            }

            public async Task<TipoEscalaDTO> Incluir(TipoEscalaDTO tipoEscalaDTO)
            {
                try
                {
                    if (tipoEscalaDTO is null)
                        return new TipoEscalaDTO { valido = false, mensagem = "Objeto não preenchido." };

                    var tipoEscala = _mapper.Map<DepInfra.TipoEscala>(tipoEscalaDTO);
                    var tipoEscalaCriado = await _tipoEscalaRepository.IncluirAsync(tipoEscala);
                    return _mapper.Map<TipoEscalaDTO>(tipoEscalaCriado);
                }
                catch (Exception e)
                {
                    return new TipoEscalaDTO { valido = false, mensagem = $"Erro ao incluir Tipo Escala: {e.Message}" };
                }
            }

            public async Task<TipoEscalaDTO> Alterar(Guid id, TipoEscalaDTO tipoEscalaDTO)
            {
                try
                {
                    if (id == Guid.Empty)
                        return new TipoEscalaDTO { valido = false, mensagem = "Id fora do Range." };

                    var tipoEscalaExistente = await _tipoEscalaRepository.BuscarPorIdAsync(id);
                    if (tipoEscalaExistente == null)
                        return new TipoEscalaDTO { valido = false, mensagem = "Tipo Escala não encontrado." };

                    _mapper.Map(tipoEscalaDTO, tipoEscalaExistente);
                    var tipoEscalaAtualizado = await _tipoEscalaRepository.AlterarAsync(tipoEscalaExistente);

                    return _mapper.Map<TipoEscalaDTO>(tipoEscalaAtualizado);
                }
                catch (Exception e)
                {
                    throw new Exception($"Erro ao alterar Tipo Escala: {e.Message}");
                }
            }

            public async Task<List<TipoEscalaDTO>> BuscarTodos()
            {
                try
                {
                    var tipoEscalas = await _tipoEscalaRepository.BuscarTodosAsync();
                    return _mapper.Map<List<TipoEscalaDTO>>(tipoEscalas);
                }
                catch (Exception e)
                {
                    throw new Exception($"Erro ao buscar todos os Tipos de Escala: {e.Message}");
                }
            }

            public async Task<TipoEscalaDTO> Deletar(Guid id)
            {
                try
                {
                    if (id == Guid.Empty)
                        return new TipoEscalaDTO { valido = false, mensagem = "Id fora do Range." };

                    var sucesso = await _tipoEscalaRepository.DeletarAsync(id);
                    return sucesso
                        ? new TipoEscalaDTO { valido = true, mensagem = "Tipo Escala deletado com sucesso." }
                        : new TipoEscalaDTO { valido = false, mensagem = "Tipo Escala não encontrado." };
                }
                catch (Exception e)
                {
                    throw new Exception($"Erro ao deletar Tipo Escala: {e.Message}");
                }
            }

            public async Task<TipoEscalaDTO> BuscarPorId(Guid idEscala)
            {
                try
                {
                    if (idEscala == Guid.Empty)
                        return new TipoEscalaDTO { valido = false, mensagem = "Id fora do Range." };

                    var tipoEscala = await _tipoEscalaRepository.BuscarPorIdAsync(idEscala);
                    return tipoEscala != null
                        ? _mapper.Map<TipoEscalaDTO>(tipoEscala)
                        : new TipoEscalaDTO { valido = false, mensagem = "Tipo Escala não encontrado." };
                }
                catch (Exception e)
                {
                    throw new Exception($"Erro ao buscar Tipo Escala por ID: {e.Message}");
                }
            }
        }
    }
}
