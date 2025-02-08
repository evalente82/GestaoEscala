using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Setor;
using GestaoEscalaPermutas.Dominio.Interfaces.Setor;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.Setor
{
    public class SetorService : ISetorService
    {
        private readonly ISetorRepository _setorRepository;
        private readonly IMapper _mapper;

        public SetorService(ISetorRepository setorRepository, IMapper mapper)
        {
            _setorRepository = setorRepository;
            _mapper = mapper;
        }

        public async Task<SetorDTO> Incluir(SetorDTO setorDTO)
        {
            try
            {
                if (setorDTO is null)
                    return new SetorDTO { valido = false, mensagem = "Objeto não preenchido." };

                var setor = _mapper.Map<DepInfra.Setor>(setorDTO);
                var setorCriado = await _setorRepository.IncluirAsync(setor);
                return _mapper.Map<SetorDTO>(setorCriado);
            }
            catch (Exception e)
            {
                return new SetorDTO { valido = false, mensagem = $"Erro ao incluir setor: {e.Message}" };
            }
        }

        public async Task<SetorDTO> Alterar(Guid id, SetorDTO setorDTO)
        {
            try
            {
                if (id == Guid.Empty)
                    return new SetorDTO { valido = false, mensagem = "Id fora do Range." };

                var setorExistente = await _setorRepository.BuscarPorIdAsync(id);
                if (setorExistente == null)
                    return new SetorDTO { valido = false, mensagem = "Setor não encontrado." };

                _mapper.Map(setorDTO, setorExistente);
                var setorAtualizado = await _setorRepository.AlterarAsync(setorExistente);

                return _mapper.Map<SetorDTO>(setorAtualizado);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao alterar setor: {e.Message}");
            }
        }

        public async Task<SetorDTO> BuscarPorId(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return new SetorDTO { valido = false, mensagem = "Id fora do Range." };

                var setor = await _setorRepository.BuscarPorIdAsync(id);
                return setor != null
                    ? _mapper.Map<SetorDTO>(setor)
                    : new SetorDTO { valido = false, mensagem = "Setor não encontrado." };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar setor por ID: {e.Message}");
            }
        }

        public async Task<List<SetorDTO>> BuscarTodos()
        {
            try
            {
                var setores = await _setorRepository.BuscarTodosAsync();
                return _mapper.Map<List<SetorDTO>>(setores);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar todos os setores: {e.Message}");
            }
        }

        public async Task<SetorDTO> Deletar(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return new SetorDTO { valido = false, mensagem = "Id fora do Range." };

                var sucesso = await _setorRepository.DeletarAsync(id);
                return sucesso
                    ? new SetorDTO { valido = true, mensagem = "Setor deletado com sucesso." }
                    : new SetorDTO { valido = false, mensagem = "Setor não encontrado." };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao deletar setor: {e.Message}");
            }
        }
    }
}