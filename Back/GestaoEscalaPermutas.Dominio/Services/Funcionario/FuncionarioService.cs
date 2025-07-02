using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.DTO.Usuario;
using GestaoEscalaPermutas.Dominio.Entities;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Implementations;
using GestaoEscalaPermutas.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.Funcionario
{
    namespace GestaoEscalaPermutas.Dominio.Services.Funcionario
    {
        public class FuncionarioService : IFuncionarioService
        {
            private readonly IFuncionarioRepository _funcionarioRepository;
            private readonly IMapper _mapper;
            private readonly IUsuarioRepository _usuarioRepository;

            public FuncionarioService(IFuncionarioRepository funcionarioRepository, IMapper mapper, IUsuarioRepository usuarioRepository)
            {
                _funcionarioRepository = funcionarioRepository;
                _mapper = mapper;
                _usuarioRepository = usuarioRepository;
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
                // --- 1. VALIDAÇÃO INICIAL ---
                if (id == Guid.Empty || id != funcionarioDTO.IdFuncionario)
                {
                    return new FuncionarioDTO { valido = false, mensagem = "ID inválido ou inconsistente." };
                }

                // --- 2. BUSCAR AMBAS AS ENTIDADES QUE SERÃO ALTERADAS ---
                var funcionarioExistente = await _funcionarioRepository.ObterPorIdAsync(id);
                if (funcionarioExistente == null)
                {
                    return new FuncionarioDTO { valido = false, mensagem = "Funcionário não encontrado." };
                }

                // É crucial usar 'await' para obter o objeto Usuario, e não a Task.
                var usuarioExistente = await _usuarioRepository.BuscarPorIdAsync(id);
                if (usuarioExistente == null)
                {
                    // Decide como tratar: pode ser um erro ou talvez o usuário ainda não exista.
                    // Por segurança, vamos tratar como um erro.
                    return new FuncionarioDTO { valido = false, mensagem = "Usuário correspondente não encontrado." };
                }


                // --- 3. ATUALIZAR OS OBJETOS EM MEMÓRIA ---

                // a) Usa o AutoMapper para atualizar o funcionário com os dados do DTO.
                _mapper.Map(funcionarioDTO, funcionarioExistente);

                // b) Atualiza manualmente as propriedades do usuário com base no funcionário já atualizado.
                //    Não precisamos criar um novo DTO para isso.
                usuarioExistente.Email = funcionarioExistente.NmEmail;
                usuarioExistente.Nome = funcionarioExistente.NmNome;
                // Se houver outras propriedades para sincronizar, adicione-as aqui.


                // --- 4. PREPARAR AS ALTERAÇÕES PARA SEREM SALVAS ---
                // Apenas informa ao repositório que as entidades foram modificadas.
                // Estes métodos não devem chamar SaveChanges() se você usa Unit of Work.
                await _funcionarioRepository.AlterarAsync(funcionarioExistente);
                await _usuarioRepository.AtualizarAsync(usuarioExistente);
                
                // --- 6. RETORNAR O RESULTADO ---
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

            public async Task<List<FuncionarioDTO>> GetAdministradoresAsync()
            {
                try
                {
                    // Assumindo que administradores têm um cargo específico, como "Administrador"
                    var administradores = await _funcionarioRepository.ObterAdministradoresAsync();
                    if (administradores == null || !administradores.Any())
                    {
                        return new List<FuncionarioDTO> { new FuncionarioDTO { valido = false, mensagem = "Nenhum administrador encontrado." } };
                    }
                    return _mapper.Map<List<FuncionarioDTO>>(administradores);
                }
                catch (Exception ex)
                {
                    return new List<FuncionarioDTO> { new FuncionarioDTO { valido = false, mensagem = $"Erro ao buscar administradores: {ex.Message}" } };
                }
            }

            public async Task<Guid> BuscarPorNomeFuncionario(string nome)
            {
                // Verifica se o nome é vazio ou nulo
                if (string.IsNullOrEmpty(nome))
                    throw new ArgumentException("Nome vazio ou nulo.");

                // Obtém todos os funcionários
                var funcionarios = await _funcionarioRepository.ObterTodosAsync();

                // Filtra o funcionário com o nome correspondente
                var funcionario = funcionarios
                    .Where(f => f.NmNome.Equals(nome))
                    .FirstOrDefault();

                // Se o funcionário não for encontrado, retorna Guid.Empty
                if (funcionario == null)
                    return Guid.Empty;

                // Retorna o Id do funcionário encontrado
                return funcionario.IdFuncionario;
            }
        }
    }
}