using Xunit;
using Moq;
using AutoMapper;
using GestaoEscalaPermutas.Dominio.Services.Cargos;
using GestaoEscalaPermutas.Dominio.Interfaces.Cargos;
using GestaoEscalaPermutas.Dominio.DTO.Cargo;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestaoEscalaPermutas.Repository.Interfaces;

namespace GestaoEscalaPermutas.Tests.Services.Cargo
{
    public class CargoServiceTests
    {
        private readonly Mock<ICargoRepository> _cargoRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CargoService _cargoService;

        public CargoServiceTests()
        {
            _cargoRepositoryMock = new Mock<ICargoRepository>();
            _mapperMock = new Mock<IMapper>();
            _cargoService = new CargoService(_cargoRepositoryMock.Object, _mapperMock.Object);
        }

        // --- Testes para Incluir --- 

        [Fact]
        public async Task Incluir_QuandoCargoValido_DeveRetornarCargoDTOValido()
        {
            // Arrange
            var cargoDTO = new CargoDTO { NmNome = "Analista" };
            var cargoEntity = new Infra.Data.EntitiesDefesaCivilMarica.Cargo {IdCargo = Guid.NewGuid(), NmNome = "Analista" };

            _mapperMock.Setup(m => m.Map<Infra.Data.EntitiesDefesaCivilMarica.Cargo>(cargoDTO)).Returns(cargoEntity);
            _cargoRepositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Cargo>())).ReturnsAsync(cargoEntity);
            _mapperMock.Setup(m => m.Map<CargoDTO>(cargoEntity)).Returns(cargoDTO);

            // Act
            var result = await _cargoService.Incluir(cargoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.valido);
            Assert.Equal(cargoDTO.NmNome, result.NmNome);
            _cargoRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Cargo>()), Times.Once);
        }

        [Fact]
        public async Task Incluir_QuandoCargoNulo_DeveRetornarInvalido()
        {
            // Act
            var result = await _cargoService.Incluir(null);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Objeto não preenchido.", result.mensagem);
            _cargoRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Cargo>()), Times.Never);
        }

        [Fact]
        public async Task Incluir_QuandoRepositorioFalha_DeveRetornarErro()
        {
            // Arrange
            var cargoDTO = new CargoDTO { NmNome = "Analista" };
            var cargoEntity = new Infra.Data.EntitiesDefesaCivilMarica.Cargo();
            var exceptionMessage = "Erro no banco";

            _mapperMock.Setup(m => m.Map<Infra.Data.EntitiesDefesaCivilMarica.Cargo>(cargoDTO)).Returns(cargoEntity);
            _cargoRepositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Cargo>())).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _cargoService.Incluir(cargoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Contains(exceptionMessage, result.mensagem);
        }

        // --- Testes para Alterar --- 

        [Fact]
        public async Task Alterar_QuandoIdCargoValidoECargoExistente_DeveRetornarCargoDTOAtualizado()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cargoDTO = new CargoDTO {IdCargo = id, NmNome = "Analista Senior" };
            var cargoExistente = new Infra.Data.EntitiesDefesaCivilMarica.Cargo { IdCargo = id, NmNome = "Analista" };

            _cargoRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(cargoExistente);
            _mapperMock.Setup(m => m.Map(cargoDTO, cargoExistente)); // Simula atualização via AutoMapper
            _cargoRepositoryMock.Setup(r => r.AtualizarAsync(cargoExistente)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<CargoDTO>(cargoExistente)).Returns(cargoDTO);

            // Act
            var result = await _cargoService.Alterar(id, cargoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.valido);
            Assert.Equal(cargoDTO.NmNome, result.NmNome);
            _cargoRepositoryMock.Verify(r => r.AtualizarAsync(cargoExistente), Times.Once);
        }

        [Fact]
        public async Task Alterar_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Arrange
            var cargoDTO = new CargoDTO();

            // Act
            var result = await _cargoService.Alterar(Guid.Empty, cargoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Id fora do Range.", result.mensagem);
            _cargoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Cargo>()), Times.Never);
        }

        [Fact]
        public async Task Alterar_QuandoCargoNaoEncontrado_DeveRetornarInvalido()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cargoDTO = new CargoDTO();
            _cargoRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Infra.Data.EntitiesDefesaCivilMarica.Cargo)null);

            // Act
            var result = await _cargoService.Alterar(id, cargoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Cargo não encontrado.", result.mensagem);
            _cargoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Cargo>()), Times.Never);
        }

        [Fact]
        public async Task Alterar_QuandoRepositorioFalha_DeveLancarExcecao()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cargoDTO = new CargoDTO {IdCargo = id, NmNome = "Analista Senior" };
            var cargoExistente = new Infra.Data.EntitiesDefesaCivilMarica.Cargo {IdCargo = id, NmNome = "Analista" };
            var exceptionMessage = "Erro no banco";

            _cargoRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(cargoExistente);
            _mapperMock.Setup(m => m.Map(cargoDTO, cargoExistente));
            _cargoRepositoryMock.Setup(r => r.AtualizarAsync(cargoExistente)).ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _cargoService.Alterar(id, cargoDTO));
            Assert.Contains(exceptionMessage, exception.Message);
        }

        // --- Testes para BuscarTodos --- 

        [Fact]
        public async Task BuscarTodos_QuandoExistemCargos_DeveRetornarListaDTO()
        {
            // Arrange
            var cargosEntity = new List<Infra.Data.EntitiesDefesaCivilMarica.Cargo> { new Infra.Data.EntitiesDefesaCivilMarica.Cargo {IdCargo = Guid.NewGuid(), NmNome = "Analista" } };
            var cargosDTO = new List<CargoDTO> { new CargoDTO { valido = true, NmNome = "Analista" } }; 
            _cargoRepositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(cargosEntity);
            _mapperMock.Setup(m => m.Map<List<CargoDTO>>(cargosEntity)).Returns(cargosDTO);

            // Act
            var result = await _cargoService.BuscarTodos();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result[0].valido);
            Assert.Equal("Analista", result[0].NmNome);
        }

        [Fact]
        public async Task BuscarTodos_QuandoNaoExistemCargos_DeveRetornarListaVazia()
        {
            // Arrange
            var cargosEntity = new List<Infra.Data.EntitiesDefesaCivilMarica.Cargo>();
            var cargosDTO = new List<CargoDTO>();
            _cargoRepositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(cargosEntity);
            _mapperMock.Setup(m => m.Map<List<CargoDTO>>(cargosEntity)).Returns(cargosDTO);

            // Act
            var result = await _cargoService.BuscarTodos();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task BuscarTodos_QuandoRepositorioFalha_DeveLancarExcecao()
        {
            // Arrange
            var exceptionMessage = "Erro no banco";
            _cargoRepositoryMock.Setup(r => r.ObterTodosAsync()).ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _cargoService.BuscarTodos());
            Assert.Contains(exceptionMessage, exception.Message);
        }

        // --- Testes para Deletar --- 

        [Fact]
        public async Task Deletar_QuandoIdValidoECargoExistente_DeveRetornarSucesso()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cargoExistente = new Infra.Data.EntitiesDefesaCivilMarica.Cargo {IdCargo = id };
            _cargoRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(cargoExistente);
            _cargoRepositoryMock.Setup(r => r.RemoverAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _cargoService.Deletar(id);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.valido);
            Assert.Equal("Cargo deletado com sucesso.", result.mensagem);
            _cargoRepositoryMock.Verify(r => r.RemoverAsync(id), Times.Once);
        }

        [Fact]
        public async Task Deletar_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Act
            var result = await _cargoService.Deletar(Guid.Empty);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Id fora do Range.", result.mensagem);
            _cargoRepositoryMock.Verify(r => r.RemoverAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Deletar_QuandoCargoNaoEncontrado_DeveRetornarInvalido()
        {
            // Arrange
            var id = Guid.NewGuid();
            _cargoRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Infra.Data.EntitiesDefesaCivilMarica.Cargo)null);

            // Act
            var result = await _cargoService.Deletar(id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Cargo não encontrado.", result.mensagem);
            _cargoRepositoryMock.Verify(r => r.RemoverAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Deletar_QuandoRepositorioFalha_DeveLancarExcecao()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cargoExistente = new Infra.Data.EntitiesDefesaCivilMarica.Cargo {IdCargo = id };
            var exceptionMessage = "Erro no banco";

            _cargoRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(cargoExistente);
            _cargoRepositoryMock.Setup(r => r.RemoverAsync(id)).ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _cargoService.Deletar(id));
            Assert.Contains(exceptionMessage, exception.Message);
        }
    }
}

