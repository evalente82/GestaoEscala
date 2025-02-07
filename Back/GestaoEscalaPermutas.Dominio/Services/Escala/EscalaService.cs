using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Dominio.Interfaces.Escala;
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

        public EscalaService(IEscalaRepository escalaRepository, IMapper mapper)
        {
            _escalaRepository = escalaRepository;
            _mapper = mapper;
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
    }
}
