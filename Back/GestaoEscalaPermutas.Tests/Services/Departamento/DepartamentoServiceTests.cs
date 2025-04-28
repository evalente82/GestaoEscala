using Xunit;
using Moq;
using AutoMapper;
using GestaoEscalaPermutas.Dominio.Services.Departamento;
using GestaoEscalaPermutas.Dominio.Interfaces.Departamento;
using GestaoEscalaPermutas.Dominio.DTO.Departamento;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestaoEscalaPermutas.Repository.Interfaces;

namespace GestaoEscalaPermutas.Tests.Services.Departamento
{
    public class DepartamentoServiceTests
    {
        private readonly Mock<IDepartamentoRepository> _departamentoRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly DepartamentoService _departamentoService;

        public DepartamentoServiceTests()
        {
            _departamentoRepositoryMock = new Mock<IDepartamentoRepository>();
            _mapperMock = new Mock<IMapper>();
            _departamentoService = new DepartamentoService(_departamentoRepositoryMock.Object, _mapperMock.Object);
        }

        // --- Testes para Incluir --- 

        [Fact]
        public async Task Incluir_QuandoDepartamentoValido_DeveRetornarDepartamentoDTOValido()
        {
            // Arrange
            var departamentoDTO = new DepartamentoDTO { NmNome = "RH" };
            var departamentoEntity = new Infra.Data.EntitiesDefesaCivilMarica.Departamento {IdDepartamento = Guid.NewGuid(),NmNome = "RH" };

            _mapperMock.Setup(m => m.Map<Infra.Data.EntitiesDefesaCivilMarica.Departamento>(departamentoDTO)).Returns(departamentoEntity);
            _departamentoRepositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Departamento>())).ReturnsAsync(departamentoEntity);
            _mapperMock.Setup(m => m.Map<DepartamentoDTO>(departamentoEntity)).Returns(departamentoDTO);

            // Act
            var result = await _departamentoService.Incluir(departamentoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.valido);
            Assert.Equal(departamentoDTO.NmNome, result.NmNome);
            _departamentoRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Departamento>()), Times.Once);
        }

        [Fact]
        public async Task Incluir_QuandoDepartamentoNulo_DeveRetornarInvalido()
        {
            // Act
            var result = await _departamentoService.Incluir(null);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Objeto não preenchido.", result.mensagem);
            _departamentoRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Departamento>()), Times.Never);
        }

        [Fact]
        public async Task Incluir_QuandoRepositorioFalha_DeveRetornarErro()
        {
            // Arrange
            var departamentoDTO = new DepartamentoDTO { NmNome = "RH" };
            var departamentoEntity = new Infra.Data.EntitiesDefesaCivilMarica.Departamento();
            var exceptionMessage = "Erro no banco";

            _mapperMock.Setup(m => m.Map<Infra.Data.EntitiesDefesaCivilMarica.Departamento>(departamentoDTO)).Returns(departamentoEntity);
            _departamentoRepositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Departamento>())).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _departamentoService.Incluir(departamentoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Contains(exceptionMessage, result.mensagem);
        }

        // --- Testes para Alterar --- 

        [Fact]
        public async Task Alterar_QuandoIdValidoEDepartamentoExistente_DeveRetornarDepartamentoDTOAtualizado()
        {
            // Arrange
            var id = Guid.NewGuid();
            var departamentoDTO = new DepartamentoDTO { IdDepartamento = id, NmNome = "Recursos Humanos" };
            var departamentoExistente = new Infra.Data.EntitiesDefesaCivilMarica.Departamento { IdDepartamento = id, NmNome = "RH" };

            _departamentoRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(departamentoExistente);
            _mapperMock.Setup(m => m.Map(departamentoDTO, departamentoExistente)); // Simula atualização via AutoMapper
            _departamentoRepositoryMock.Setup(r => r.AtualizarAsync(departamentoExistente)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<DepartamentoDTO>(departamentoExistente)).Returns(departamentoDTO);

            // Act
            var result = await _departamentoService.Alterar(id, departamentoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.valido);
            Assert.Equal(departamentoDTO.NmNome, result.NmNome);
            _departamentoRepositoryMock.Verify(r => r.AtualizarAsync(departamentoExistente), Times.Once);
        }

        [Fact]
        public async Task Alterar_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Arrange
            var departamentoDTO = new DepartamentoDTO();

            // Act
            var result = await _departamentoService.Alterar(Guid.Empty, departamentoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Id fora do Range.", result.mensagem);
            _departamentoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Departamento>()), Times.Never);
        }

        [Fact]
        public async Task Alterar_QuandoDepartamentoNaoEncontrado_DeveRetornarInvalido()
        {
            // Arrange
            var id = Guid.NewGuid();
            var departamentoDTO = new DepartamentoDTO();
            _departamentoRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Infra.Data.EntitiesDefesaCivilMarica.Departamento)null);

            // Act
            var result = await _departamentoService.Alterar(id, departamentoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Departamento não encontrado.", result.mensagem);
            _departamentoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Departamento>()), Times.Never);
        }
        
        [Fact]
        public async Task Alterar_QuandoRepositorioFalha_DeveRetornarErro()
        {
            // Arrange
            var id = Guid.NewGuid();
            var departamentoDTO = new DepartamentoDTO { IdDepartamento = id, NmNome = "Recursos Humanos" };
            var departamentoExistente = new Infra.Data.EntitiesDefesaCivilMarica.Departamento { IdDepartamento = id, NmNome = "RH" };
            var exceptionMessage = "Erro no banco";

            _departamentoRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(departamentoExistente);
            _mapperMock.Setup(m => m.Map(departamentoDTO, departamentoExistente));
            _departamentoRepositoryMock.Setup(r => r.AtualizarAsync(departamentoExistente)).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _departamentoService.Alterar(id, departamentoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Contains(exceptionMessage, result.mensagem);
        }

        // --- Testes para BuscarTodos --- 

        [Fact]
        public async Task BuscarTodos_QuandoExistemDepartamentos_DeveRetornarListaDTO()
        {
            // Arrange
            var departamentosEntity = new List<Infra.Data.EntitiesDefesaCivilMarica.Departamento> { new Infra.Data.EntitiesDefesaCivilMarica.Departamento { IdDepartamento = Guid.NewGuid() } };
            var departamentosDTO = new List<DepartamentoDTO> { new DepartamentoDTO { valido = true } }; // Assume que o mapper preenche valido
            _departamentoRepositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(departamentosEntity);
            _mapperMock.Setup(m => m.Map<List<DepartamentoDTO>>(departamentosEntity)).Returns(departamentosDTO);

            // Act
            var result = await _departamentoService.BuscarTodos();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result[0].valido);
        }

        [Fact]
        public async Task BuscarTodos_QuandoNaoExistemDepartamentos_DeveRetornarListaVazia()
        {
            // Arrange
            var departamentosEntity = new List<Infra.Data.EntitiesDefesaCivilMarica.Departamento>();
            var departamentosDTO = new List<DepartamentoDTO>();
            _departamentoRepositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(departamentosEntity);
            _mapperMock.Setup(m => m.Map<List<DepartamentoDTO>>(departamentosEntity)).Returns(departamentosDTO);

            // Act
            var result = await _departamentoService.BuscarTodos();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task BuscarTodos_QuandoRepositorioFalha_DeveLancarExcecao()
        {
            // Arrange
            var exceptionMessage = "Erro no banco";
            _departamentoRepositoryMock.Setup(r => r.ObterTodosAsync()).ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _departamentoService.BuscarTodos());
            Assert.Contains(exceptionMessage, exception.Message);
        }

        // --- Testes para Deletar --- 

        [Fact]
        public async Task Deletar_QuandoIdValidoEDepartamentoExistente_DeveRetornarSucesso()
        {
            // Arrange
            var id = Guid.NewGuid();
            var departamentoExistente = new Infra.Data.EntitiesDefesaCivilMarica.Departamento { IdDepartamento = id };
            _departamentoRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(departamentoExistente);
            _departamentoRepositoryMock.Setup(r => r.RemoverAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _departamentoService.Deletar(id);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.valido);
            Assert.Equal("Departamento deletado com sucesso.", result.mensagem);
            _departamentoRepositoryMock.Verify(r => r.RemoverAsync(id), Times.Once);
        }

        [Fact]
        public async Task Deletar_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Act
            var result = await _departamentoService.Deletar(Guid.Empty);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Id fora do Range.", result.mensagem);
            _departamentoRepositoryMock.Verify(r => r.RemoverAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Deletar_QuandoDepartamentoNaoEncontrado_DeveRetornarInvalido()
        {
            // Arrange
            var id = Guid.NewGuid();
            _departamentoRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Infra.Data.EntitiesDefesaCivilMarica.Departamento)null);

            // Act
            var result = await _departamentoService.Deletar(id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Departamento não encontrado.", result.mensagem);
            _departamentoRepositoryMock.Verify(r => r.RemoverAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Deletar_QuandoRepositorioFalha_DeveRetornarErro()
        {
            // Arrange
            var id = Guid.NewGuid();
            var departamentoExistente = new Infra.Data.EntitiesDefesaCivilMarica.Departamento { IdDepartamento = id };
            var exceptionMessage = "Erro no banco";

            _departamentoRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(departamentoExistente);
            _departamentoRepositoryMock.Setup(r => r.RemoverAsync(id)).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _departamentoService.Deletar(id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Contains(exceptionMessage, result.mensagem);
        }
    }
}

