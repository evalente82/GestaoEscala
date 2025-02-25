using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.Entities;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.Funcionario
{
    namespace GestaoEscalaPermutas.Dominio.Services.Funcionario
    {
        public class FuncionarioService : IFuncionarioService
        {
            private readonly IFuncionarioRepository _funcionarioRepository;
            private readonly IMapper _mapper;

            public FuncionarioService(IFuncionarioRepository funcionarioRepository, IMapper mapper)
            {
                _funcionarioRepository = funcionarioRepository;
                _mapper = mapper;
            }

            public async Task<FuncionarioDTO> Incluir(FuncionarioDTO funcionarioDTO)
            {
                if (funcionarioDTO is null)
                    return new FuncionarioDTO { valido = false, mensagem = "Objeto não preenchido." };

                // ✅ Verifica se a matrícula ou e-mail já existem
                bool matriculaExiste = await _funcionarioRepository.MatriculaExisteAsync(funcionarioDTO.NrMatricula);
                bool emailExiste = await _funcionarioRepository.EmailExisteAsync(funcionarioDTO.NmEmail);

                if (matriculaExiste && emailExiste)
                    return new FuncionarioDTO { valido = false, mensagem = "Matrícula e E-mail já cadastrados." };
                if (matriculaExiste)
                    return new FuncionarioDTO { valido = false, mensagem = "Matrícula já cadastrada." };
                if (emailExiste)
                    return new FuncionarioDTO { valido = false, mensagem = "E-mail já cadastrado." };

                var funcionario = _mapper.Map<DepInfra.Funcionario>(funcionarioDTO);
                var novoFuncionario = await _funcionarioRepository.AdicionarAsync(funcionario);
                return _mapper.Map<FuncionarioDTO>(novoFuncionario);
            }

            public async Task<FuncionarioDTO> Alterar(Guid id, FuncionarioDTO funcionarioDTO)
            {
                if (id == Guid.Empty)
                    return new FuncionarioDTO { valido = false, mensagem = "Id fora do Range." };

                var funcionarioExistente = await _funcionarioRepository.ObterPorIdAsync(id);
                if (funcionarioExistente == null)
                    return new FuncionarioDTO { valido = false, mensagem = "Funcionário não encontrado." };

                _mapper.Map(funcionarioDTO, funcionarioExistente);
                await _funcionarioRepository.AlterarAsync(funcionarioExistente);
                return _mapper.Map<FuncionarioDTO>(funcionarioExistente);
            }

            public async Task<List<FuncionarioDTO>> BuscarTodos()
            {
                var funcionarios = await _funcionarioRepository.ObterTodosAsync();
                return _mapper.Map<List<FuncionarioDTO>>(funcionarios);
            }

            public async Task<List<FuncionarioDTO>> BuscarTodosAtivos()
            {
                var funcionariosAtivos = await _funcionarioRepository.ObterTodosAtivosAsync();
                return _mapper.Map<List<FuncionarioDTO>>(funcionariosAtivos);
            }

            public async Task<FuncionarioDTO> Deletar(Guid id)
            {
                if (id == Guid.Empty)
                    return new FuncionarioDTO { valido = false, mensagem = "Id fora do Range." };

                var funcionarioExistente = await _funcionarioRepository.ObterPorIdAsync(id);
                if (funcionarioExistente == null)
                    return new FuncionarioDTO { valido = false, mensagem = "Funcionário não encontrado." };

                await _funcionarioRepository.RemoverAsync(id);
                return new FuncionarioDTO { valido = true, mensagem = "Funcionário deletado com sucesso." };
            }

            public async Task<FuncionarioDTO[]> IncluirLista(FuncionarioDTO[] funcionarioDTOs)
            {
                if (funcionarioDTOs is null || funcionarioDTOs.Length == 0)
                    return new FuncionarioDTO[] { new() { valido = false, mensagem = "Lista de funcionários vazia." } };

                var funcionarios = _mapper.Map<DepInfra.Funcionario[]>(funcionarioDTOs);
                var novosFuncionarios = await _funcionarioRepository.AdicionarListaAsync(funcionarios);
                return _mapper.Map<FuncionarioDTO[]>(novosFuncionarios);
            }

            public async Task<string> GetFcmTokenAsync(Guid idFuncionario)
            {
                return await _funcionarioRepository.GetFcmTokenAsync(idFuncionario);
            }

            public async Task SaveFcmTokenAsync(Guid idFuncionario, string fcmToken)
            {
                if (string.IsNullOrEmpty(fcmToken))
                    throw new ArgumentException("O FCM Token não pode ser vazio.");

                await _funcionarioRepository.SaveFcmTokenAsync(idFuncionario, fcmToken);
            }
        }
    }
}