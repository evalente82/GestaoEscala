using Xunit;
using Moq;
using AutoMapper;
using GestaoEscalaPermutas.Dominio.Services.Funcionario.GestaoEscalaPermutas.Dominio.Services.Funcionario;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GestaoEscalaPermutas.Repository.Interfaces;

namespace GestaoEscalaPermutas.Tests.Services.Funcionario
{
    public class FuncionarioServiceTests
    {
        private readonly Mock<IFuncionarioRepository> _funcionarioRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly FuncionarioService _funcionarioService;

        public FuncionarioServiceTests()
        {
            _funcionarioRepositoryMock = new Mock<IFuncionarioRepository>();
            _mapperMock = new Mock<IMapper>();
            _funcionarioService = new FuncionarioService(_funcionarioRepositoryMock.Object, _mapperMock.Object);
        }

        // --- Testes para Incluir --- 

        [Fact]
        public async Task Incluir_QuandoFuncionarioValido_DeveRetornarFuncionarioDTOValido()
        {
            // Arrange
            var funcionarioDTO = new FuncionarioDTO { NrMatricula = 123, NmEmail = "teste@teste.com", NmNome = "Teste" };
            var funcionarioEntity = new Infra.Data.EntitiesDefesaCivilMarica.Funcionario {IdFuncionario = Guid.NewGuid(), NrMatricula = 123, NmEmail = "teste@teste.com", NmNome = "Teste" };

            _funcionarioRepositoryMock.Setup(r => r.MatriculaExisteAsync(funcionarioDTO.NrMatricula)).ReturnsAsync(false);
            _funcionarioRepositoryMock.Setup(r => r.EmailExisteAsync(funcionarioDTO.NmEmail)).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<Infra.Data.EntitiesDefesaCivilMarica.Funcionario>(funcionarioDTO)).Returns(funcionarioEntity);
            _funcionarioRepositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Funcionario>())).ReturnsAsync(funcionarioEntity);
            _mapperMock.Setup(m => m.Map<FuncionarioDTO>(funcionarioEntity)).Returns(funcionarioDTO);

            // Act
            var result = await _funcionarioService.Incluir(funcionarioDTO);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.valido);
            Assert.Equal(funcionarioDTO.NmNome, result.NmNome);
            _funcionarioRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Funcionario>()), Times.Once);
        }

        [Fact]
        public async Task Incluir_QuandoFuncionarioNulo_DeveRetornarInvalido()
        {
            // Act
            var result = await _funcionarioService.Incluir(null);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Objeto não preenchido.", result.mensagem);
            _funcionarioRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Funcionario>()), Times.Never);
        }

        [Fact]
        public async Task Incluir_QuandoMatriculaJaExiste_DeveRetornarInvalido()
        {
            // Arrange
            var funcionarioDTO = new FuncionarioDTO { NrMatricula = 123, NmEmail = "teste@teste.com" };
            _funcionarioRepositoryMock.Setup(r => r.MatriculaExisteAsync(funcionarioDTO.NrMatricula)).ReturnsAsync(true);
            _funcionarioRepositoryMock.Setup(r => r.EmailExisteAsync(funcionarioDTO.NmEmail)).ReturnsAsync(false);

            // Act
            var result = await _funcionarioService.Incluir(funcionarioDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Matrícula já cadastrada.", result.mensagem);
            _funcionarioRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Funcionario>()), Times.Never);
        }

        [Fact]
        public async Task Incluir_QuandoEmailJaExiste_DeveRetornarInvalido()
        {
            // Arrange
            var funcionarioDTO = new FuncionarioDTO { NrMatricula = 123, NmEmail = "teste@teste.com" };
            _funcionarioRepositoryMock.Setup(r => r.MatriculaExisteAsync(funcionarioDTO.NrMatricula)).ReturnsAsync(false);
            _funcionarioRepositoryMock.Setup(r => r.EmailExisteAsync(funcionarioDTO.NmEmail)).ReturnsAsync(true);

            // Act
            var result = await _funcionarioService.Incluir(funcionarioDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("E-mail já cadastrado.", result.mensagem);
            _funcionarioRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Funcionario>()), Times.Never);
        }

        [Fact]
        public async Task Incluir_QuandoMatriculaEEmailJaExistem_DeveRetornarInvalido()
        {
            // Arrange
            var funcionarioDTO = new FuncionarioDTO { NrMatricula = 123, NmEmail = "teste@teste.com" };
            _funcionarioRepositoryMock.Setup(r => r.MatriculaExisteAsync(funcionarioDTO.NrMatricula)).ReturnsAsync(true);
            _funcionarioRepositoryMock.Setup(r => r.EmailExisteAsync(funcionarioDTO.NmEmail)).ReturnsAsync(true);

            // Act
            var result = await _funcionarioService.Incluir(funcionarioDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Matrícula e E-mail já cadastrados.", result.mensagem);
            _funcionarioRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Funcionario>()), Times.Never);
        }

        // --- Testes para Alterar --- 

        [Fact]
        public async Task Alterar_QuandoIdValidoEFuncionarioExistente_DeveRetornarFuncionarioDTOAtualizado()
        {
            // Arrange
            var id = Guid.NewGuid();
            var funcionarioDTO = new FuncionarioDTO { IdFuncionario = id, NmNome = "Teste Atualizado" };
            var funcionarioExistente = new Infra.Data.EntitiesDefesaCivilMarica.Funcionario { IdFuncionario = id, NmNome = "Teste Original" };

            _funcionarioRepositoryMock
                .Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync(funcionarioExistente);

            _mapperMock
                .Setup(m => m.Map(funcionarioDTO, funcionarioExistente))
                .Returns(funcionarioExistente); // Simula atualização

            // Correção feita aqui:
            _funcionarioRepositoryMock
                .Setup(r => r.AlterarAsync(funcionarioExistente))
                .ReturnsAsync(funcionarioExistente); // ✅ correto

            _mapperMock
                .Setup(m => m.Map<FuncionarioDTO>(funcionarioExistente))
                .Returns(funcionarioDTO);

            // Act
            var result = await _funcionarioService.Alterar(id, funcionarioDTO);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.valido);
            Assert.Equal(funcionarioDTO.NmNome, result.NmNome);
            _funcionarioRepositoryMock.Verify(r => r.AlterarAsync(funcionarioExistente), Times.Once);
        }



        [Fact]
        public async Task Alterar_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Arrange
            var funcionarioDTO = new FuncionarioDTO();

            // Act
            var result = await _funcionarioService.Alterar(Guid.Empty, funcionarioDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Id fora do Range.", result.mensagem);
            _funcionarioRepositoryMock.Verify(r => r.AlterarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Funcionario>()), Times.Never);
        }

        [Fact]
        public async Task Alterar_QuandoFuncionarioNaoEncontrado_DeveRetornarInvalido()
        {
            // Arrange
            var id = Guid.NewGuid();
            var funcionarioDTO = new FuncionarioDTO();
            _funcionarioRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Infra.Data.EntitiesDefesaCivilMarica.Funcionario)null);

            // Act
            var result = await _funcionarioService.Alterar(id, funcionarioDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Funcionário não encontrado.", result.mensagem);
            _funcionarioRepositoryMock.Verify(r => r.AlterarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Funcionario>()), Times.Never);
        }

        // --- Testes para BuscarTodos --- 

        [Fact]
        public async Task BuscarTodos_QuandoExistemFuncionarios_DeveRetornarListaDTO()
        {
            // Arrange
            var funcionariosEntity = new List<Infra.Data.EntitiesDefesaCivilMarica.Funcionario> { new Infra.Data.EntitiesDefesaCivilMarica.Funcionario { IdFuncionario = Guid.NewGuid() } };
            var funcionariosDTO = new List<FuncionarioDTO> { new FuncionarioDTO { valido = true } };
            _funcionarioRepositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(funcionariosEntity);
            _mapperMock.Setup(m => m.Map<List<FuncionarioDTO>>(funcionariosEntity)).Returns(funcionariosDTO);

            // Act
            var result = await _funcionarioService.BuscarTodos();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result[0].valido);
        }

        [Fact]
        public async Task BuscarTodos_QuandoNaoExistemFuncionarios_DeveRetornarListaVazia()
        {
            // Arrange
            var funcionariosEntity = new List<Infra.Data.EntitiesDefesaCivilMarica.Funcionario>();
            var funcionariosDTO = new List<FuncionarioDTO>();
            _funcionarioRepositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(funcionariosEntity);
            _mapperMock.Setup(m => m.Map<List<FuncionarioDTO>>(funcionariosEntity)).Returns(funcionariosDTO);

            // Act
            var result = await _funcionarioService.BuscarTodos();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // --- Testes para BuscarTodosAtivos --- 

        [Fact]
        public async Task BuscarTodosAtivos_QuandoExistemFuncionariosAtivos_DeveRetornarListaDTO()
        {
            // Arrange
            var funcionariosEntity = new List<Infra.Data.EntitiesDefesaCivilMarica.Funcionario> { new Infra.Data.EntitiesDefesaCivilMarica.Funcionario { IdFuncionario = Guid.NewGuid(), IsAtivo = true } };
            var funcionariosDTO = new List<FuncionarioDTO> { new FuncionarioDTO { valido = true, IsAtivo = true } };
            _funcionarioRepositoryMock.Setup(r => r.ObterTodosAtivosAsync()).ReturnsAsync(funcionariosEntity);
            _mapperMock.Setup(m => m.Map<List<FuncionarioDTO>>(funcionariosEntity)).Returns(funcionariosDTO);

            // Act
            var result = await _funcionarioService.BuscarTodosAtivos();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result[0].valido);
            Assert.True(result[0].IsAtivo);
        }

        // --- Testes para Deletar --- 

        [Fact]
        public async Task Deletar_QuandoIdValidoEFuncionarioExistente_DeveRetornarSucesso()
        {
            // Arrange
            var id = Guid.NewGuid();
            var funcionarioExistente = new Infra.Data.EntitiesDefesaCivilMarica.Funcionario { IdFuncionario = id };
            _funcionarioRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(funcionarioExistente);
            _funcionarioRepositoryMock.Setup(r => r.RemoverAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _funcionarioService.Deletar(id);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.valido);
            Assert.Equal("Funcionário deletado com sucesso.", result.mensagem);
            _funcionarioRepositoryMock.Verify(r => r.RemoverAsync(id), Times.Once);
        }

        [Fact]
        public async Task Deletar_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Act
            var result = await _funcionarioService.Deletar(Guid.Empty);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Id fora do Range.", result.mensagem);
            _funcionarioRepositoryMock.Verify(r => r.RemoverAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Deletar_QuandoFuncionarioNaoEncontrado_DeveRetornarInvalido()
        {
            // Arrange
            var id = Guid.NewGuid();
            _funcionarioRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Infra.Data.EntitiesDefesaCivilMarica.Funcionario)null);

            // Act
            var result = await _funcionarioService.Deletar(id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Funcionário não encontrado.", result.mensagem);
            _funcionarioRepositoryMock.Verify(r => r.RemoverAsync(It.IsAny<Guid>()), Times.Never);
        }

        // --- Testes para IncluirLista --- 

        [Fact]
        public async Task IncluirLista_QuandoListaValida_DeveRetornarListaDTO()
        {
            // Arrange
            var funcionarioDTOs = new FuncionarioDTO[] { new FuncionarioDTO { NmNome = "Teste1" }, new FuncionarioDTO { NmNome = "Teste2" } };
            var funcionariosEntity = new Infra.Data.EntitiesDefesaCivilMarica.Funcionario[] { new Infra.Data.EntitiesDefesaCivilMarica.Funcionario { IdFuncionario = Guid.NewGuid() }, new Infra.Data.EntitiesDefesaCivilMarica.Funcionario { IdFuncionario = Guid.NewGuid() } };
            
            _mapperMock.Setup(m => m.Map<Infra.Data.EntitiesDefesaCivilMarica.Funcionario[]>(funcionarioDTOs)).Returns(funcionariosEntity);
            _funcionarioRepositoryMock.Setup(r => r.AdicionarListaAsync(funcionariosEntity)).ReturnsAsync(funcionariosEntity);
            _mapperMock.Setup(m => m.Map<FuncionarioDTO[]>(funcionariosEntity)).Returns(funcionarioDTOs);

            // Act
            var result = await _funcionarioService.IncluirLista(funcionarioDTOs);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Length);
            Assert.Equal("Teste1", result[0].NmNome);
            _funcionarioRepositoryMock.Verify(r => r.AdicionarListaAsync(funcionariosEntity), Times.Once);
        }

        [Fact]
        public async Task IncluirLista_QuandoListaNulaOuVazia_DeveRetornarInvalido()
        {
            // Arrange
            FuncionarioDTO[] funcionarioDTOs = null;

            // Act
            var resultNula = await _funcionarioService.IncluirLista(funcionarioDTOs);
            var resultVazia = await _funcionarioService.IncluirLista(new FuncionarioDTO[0]);

            // Assert
            Assert.NotNull(resultNula);
            Assert.Single(resultNula);
            Assert.False(resultNula[0].valido);
            Assert.Equal("Lista de funcionários vazia.", resultNula[0].mensagem);

            Assert.NotNull(resultVazia);
            Assert.Single(resultVazia);
            Assert.False(resultVazia[0].valido);
            Assert.Equal("Lista de funcionários vazia.", resultVazia[0].mensagem);

            _funcionarioRepositoryMock.Verify(r => r.AdicionarListaAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Funcionario[]>()), Times.Never);
        }

        // --- Testes para GetFcmTokenAsync --- 

        [Fact]
        public async Task GetFcmTokenAsync_QuandoIdValido_DeveRetornarToken()
        {
            // Arrange
            var idFuncionario = Guid.NewGuid();
            var expectedToken = "fcm_token_123";
            _funcionarioRepositoryMock.Setup(r => r.GetFcmTokenAsync(idFuncionario)).ReturnsAsync(expectedToken);

            // Act
            var result = await _funcionarioService.GetFcmTokenAsync(idFuncionario);

            // Assert
            Assert.Equal(expectedToken, result);
        }

        // --- Testes para SaveFcmTokenAsync --- 

        [Fact]
        public async Task SaveFcmTokenAsync_QuandoTokenValido_DeveChamarRepositorio()
        {
            // Arrange
            var idFuncionario = Guid.NewGuid();
            var fcmToken = "fcm_token_123";
            _funcionarioRepositoryMock.Setup(r => r.SaveFcmTokenAsync(idFuncionario, fcmToken)).Returns(Task.CompletedTask);

            // Act
            await _funcionarioService.SaveFcmTokenAsync(idFuncionario, fcmToken);

            // Assert
            _funcionarioRepositoryMock.Verify(r => r.SaveFcmTokenAsync(idFuncionario, fcmToken), Times.Once);
        }

        [Fact]
        public async Task SaveFcmTokenAsync_QuandoTokenVazio_DeveLancarArgumentException()
        {
            // Arrange
            var idFuncionario = Guid.NewGuid();
            string fcmToken = "";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _funcionarioService.SaveFcmTokenAsync(idFuncionario, fcmToken));
            Assert.Equal("O FCM Token não pode ser vazio.", exception.Message);
            _funcionarioRepositoryMock.Verify(r => r.SaveFcmTokenAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        // --- Testes para GetAdministradoresAsync --- 

        [Fact]
        public async Task GetAdministradoresAsync_QuandoExistemAdmins_DeveRetornarListaDTO()
        {
            // Arrange
            var adminsEntity = new List<Infra.Data.EntitiesDefesaCivilMarica.Funcionario> { new Infra.Data.EntitiesDefesaCivilMarica.Funcionario { IdFuncionario = Guid.NewGuid() } };
            var adminsDTO = new List<FuncionarioDTO> { new FuncionarioDTO { valido = true } };
            _funcionarioRepositoryMock.Setup(r => r.ObterAdministradoresAsync()).ReturnsAsync(adminsEntity);
            _mapperMock.Setup(m => m.Map<List<FuncionarioDTO>>(adminsEntity)).Returns(adminsDTO);

            // Act
            var result = await _funcionarioService.GetAdministradoresAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result[0].valido);
        }

        [Fact]
        public async Task GetAdministradoresAsync_QuandoNaoExistemAdmins_DeveRetornarListaComMensagem()
        {
            // Arrange
            var adminsEntity = new List<Infra.Data.EntitiesDefesaCivilMarica.Funcionario>();
            _funcionarioRepositoryMock.Setup(r => r.ObterAdministradoresAsync()).ReturnsAsync(adminsEntity);

            // Act
            var result = await _funcionarioService.GetAdministradoresAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.False(result[0].valido);
            Assert.Equal("Nenhum administrador encontrado.", result[0].mensagem);
        }

        [Fact]
        public async Task GetAdministradoresAsync_QuandoRepositorioFalha_DeveRetornarListaComErro()
        {
            // Arrange
            var exceptionMessage = "Erro no banco";
            _funcionarioRepositoryMock.Setup(r => r.ObterAdministradoresAsync()).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _funcionarioService.GetAdministradoresAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.False(result[0].valido);
            Assert.Contains(exceptionMessage, result[0].mensagem);
        }
    }
}

