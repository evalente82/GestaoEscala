using Xunit;
using Moq;
using AutoMapper;
using GestaoEscalaPermutas.Dominio.Services.Escala;
using GestaoEscalaPermutas.Dominio.Interfaces.Escala;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestaoEscalaPermutas.Repository.Interfaces;

namespace GestaoEscalaPermutas.Tests.Services.Escala
{
    public class EscalaServiceTests
    {
        private readonly Mock<IEscalaRepository> _escalaRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly EscalaService _escalaService;

        public EscalaServiceTests()
        {
            _escalaRepositoryMock = new Mock<IEscalaRepository>();
            _mapperMock = new Mock<IMapper>();
            _escalaService = new EscalaService(_escalaRepositoryMock.Object, _mapperMock.Object);
        }

        // --- Testes para Incluir --- 

        [Fact]
        public async Task Incluir_QuandoEscalaValida_DeveRetornarEscalaDTOValida()
        {
            // Arrange
            var escalaDTO = new EscalaDTO {NmNomeEscala = "Escala Teste" };
            var escalaEntity = new Infra.Data.EntitiesDefesaCivilMarica.Escala { IdEscala = Guid.NewGuid(), NmNomeEscala = "Escala Teste" };

            _mapperMock.Setup(m => m.Map<Infra.Data.EntitiesDefesaCivilMarica.Escala>(escalaDTO)).Returns(escalaEntity);
            _escalaRepositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Escala>())).ReturnsAsync(escalaEntity);
            _mapperMock.Setup(m => m.Map<EscalaDTO>(escalaEntity)).Returns(escalaDTO);

            // Act
            var result = await _escalaService.Incluir(escalaDTO);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.valido);
            Assert.Equal(escalaDTO.NmNomeEscala, result.NmNomeEscala);
            _escalaRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Escala>()), Times.Once);
        }

        [Fact]
        public async Task Incluir_QuandoEscalaNula_DeveRetornarInvalido()
        {
            // Act
            var result = await _escalaService.Incluir(null);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Objeto não preenchido.", result.mensagem);
            _escalaRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Escala>()), Times.Never);
        }

        [Fact]
        public async Task Incluir_QuandoRepositorioFalha_DeveRetornarErro()
        {
            // Arrange
            var escalaDTO = new EscalaDTO { NmNomeEscala = "Escala Teste" };
            var escalaEntity = new Infra.Data.EntitiesDefesaCivilMarica.Escala();
            var exceptionMessage = "Erro no banco";

            _mapperMock.Setup(m => m.Map<Infra.Data.EntitiesDefesaCivilMarica.Escala>(escalaDTO)).Returns(escalaEntity);
            _escalaRepositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Escala>())).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _escalaService.Incluir(escalaDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Contains(exceptionMessage, result.mensagem);
        }

        // --- Testes para Alterar --- 

        [Fact]
        public async Task Alterar_QuandoIdValidoEEscalaExistente_DeveRetornarEscalaDTOAtualizada()
        {
            // Arrange
            var id = Guid.NewGuid();
            var escalaDTO = new EscalaDTO { IdEscala = id, NmNomeEscala = "Escala Atualizada" };
            var escalaExistente = new Infra.Data.EntitiesDefesaCivilMarica.Escala { IdEscala = id, NmNomeEscala = "Escala Original" };

            _escalaRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(escalaExistente);
            _mapperMock.Setup(m => m.Map(escalaDTO, escalaExistente)); // Simula atualização via AutoMapper
            _escalaRepositoryMock.Setup(r => r.AtualizarAsync(escalaExistente)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<EscalaDTO>(escalaExistente)).Returns(escalaDTO);

            // Act
            var result = await _escalaService.Alterar(id, escalaDTO);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.valido);
            Assert.Equal(escalaDTO.NmNomeEscala, result.NmNomeEscala);
            _escalaRepositoryMock.Verify(r => r.AtualizarAsync(escalaExistente), Times.Once);
        }

        [Fact]
        public async Task Alterar_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Arrange
            var escalaDTO = new EscalaDTO();

            // Act
            var result = await _escalaService.Alterar(Guid.Empty, escalaDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Id fora do Range.", result.mensagem);
            _escalaRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Escala>()), Times.Never);
        }

        [Fact]
        public async Task Alterar_QuandoEscalaNaoEncontrada_DeveRetornarInvalido()
        {
            // Arrange
            var id = Guid.NewGuid();
            var escalaDTO = new EscalaDTO();
            _escalaRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Infra.Data.EntitiesDefesaCivilMarica.Escala)null);

            // Act
            var result = await _escalaService.Alterar(id, escalaDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Escala não encontrada.", result.mensagem);
            _escalaRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.Escala>()), Times.Never);
        }

        [Fact]
        public async Task Alterar_QuandoRepositorioFalha_DeveRetornarErro()
        {
            // Arrange
            var id = Guid.NewGuid();
            var escalaDTO = new EscalaDTO { IdEscala = id, NmNomeEscala = "Escala Atualizada" };
            var escalaExistente = new Infra.Data.EntitiesDefesaCivilMarica.Escala { IdEscala = id, NmNomeEscala = "Escala Original" };
            var exceptionMessage = "Erro no banco";

            _escalaRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(escalaExistente);
            _mapperMock.Setup(m => m.Map(escalaDTO, escalaExistente));
            _escalaRepositoryMock.Setup(r => r.AtualizarAsync(escalaExistente)).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _escalaService.Alterar(id, escalaDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Contains(exceptionMessage, result.mensagem);
        }

        // --- Testes para BuscarTodos --- 

        [Fact]
        public async Task BuscarTodos_QuandoExistemEscalas_DeveRetornarListaDTO()
        {
            // Arrange
            var escalasEntity = new List<Infra.Data.EntitiesDefesaCivilMarica.Escala> { new Infra.Data.EntitiesDefesaCivilMarica.Escala { IdEscala = Guid.NewGuid() } };
            var escalasDTO = new List<EscalaDTO> { new EscalaDTO { valido = true } }; // Assume que o mapper preenche valido
            _escalaRepositoryMock.Setup(r => r.ObterTodasAsync()).ReturnsAsync(escalasEntity);
            _mapperMock.Setup(m => m.Map<List<EscalaDTO>>(escalasEntity)).Returns(escalasDTO);

            // Act
            var result = await _escalaService.BuscarTodos();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result[0].valido);
        }

        [Fact]
        public async Task BuscarTodos_QuandoNaoExistemEscalas_DeveRetornarListaVazia()
        {
            // Arrange
            var escalasEntity = new List<Infra.Data.EntitiesDefesaCivilMarica.Escala>();
            var escalasDTO = new List<EscalaDTO>();
            _escalaRepositoryMock.Setup(r => r.ObterTodasAsync()).ReturnsAsync(escalasEntity);
            _mapperMock.Setup(m => m.Map<List<EscalaDTO>>(escalasEntity)).Returns(escalasDTO);

            // Act
            var result = await _escalaService.BuscarTodos();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task BuscarTodos_QuandoRepositorioFalha_DeveLancarExcecao()
        {
            // Arrange
            var exceptionMessage = "Erro no banco";
            _escalaRepositoryMock.Setup(r => r.ObterTodasAsync()).ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _escalaService.BuscarTodos());
            Assert.Contains(exceptionMessage, exception.Message);
        }

        // --- Testes para Deletar --- 

        [Fact]
        public async Task Deletar_QuandoIdValidoEEscalaExistente_DeveRetornarSucesso()
        {
            // Arrange
            var id = Guid.NewGuid();
            var escalaExistente = new Infra.Data.EntitiesDefesaCivilMarica.Escala { IdEscala = id };
            _escalaRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(escalaExistente);
            _escalaRepositoryMock.Setup(r => r.RemoverEscalasProntasPorEscalaId(id)).Returns(Task.CompletedTask);
            _escalaRepositoryMock.Setup(r => r.RemoverAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _escalaService.Deletar(id);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.valido);
            Assert.Equal("Escala deletada com sucesso.", result.mensagem);
            _escalaRepositoryMock.Verify(r => r.RemoverEscalasProntasPorEscalaId(id), Times.Once);
            _escalaRepositoryMock.Verify(r => r.RemoverAsync(id), Times.Once);
        }

        [Fact]
        public async Task Deletar_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Act
            var result = await _escalaService.Deletar(Guid.Empty);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Id fora do Range.", result.mensagem);
            _escalaRepositoryMock.Verify(r => r.RemoverAsync(It.IsAny<Guid>()), Times.Never);
            _escalaRepositoryMock.Verify(r => r.RemoverEscalasProntasPorEscalaId(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Deletar_QuandoEscalaNaoEncontrada_DeveRetornarInvalido()
        {
            // Arrange
            var id = Guid.NewGuid();
            _escalaRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Infra.Data.EntitiesDefesaCivilMarica.Escala)null);

            // Act
            var result = await _escalaService.Deletar(id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Escala não encontrada.", result.mensagem);
            _escalaRepositoryMock.Verify(r => r.RemoverAsync(It.IsAny<Guid>()), Times.Never);
            _escalaRepositoryMock.Verify(r => r.RemoverEscalasProntasPorEscalaId(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Deletar_QuandoRepositorioFalha_DeveRetornarErro()
        {
            // Arrange
            var id = Guid.NewGuid();
            var escalaExistente = new Infra.Data.EntitiesDefesaCivilMarica.Escala { IdEscala = id };
            var exceptionMessage = "Erro no banco";

            _escalaRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(escalaExistente);
            // Simula falha em qualquer uma das operações de remoção
            _escalaRepositoryMock.Setup(r => r.RemoverEscalasProntasPorEscalaId(id)).ThrowsAsync(new Exception(exceptionMessage)); 
            // Ou: _escalaRepositoryMock.Setup(r => r.RemoverAsync(id)).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _escalaService.Deletar(id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Contains(exceptionMessage, result.mensagem);
        }

        // --- Testes para BuscarPorId --- 

        [Fact]
        public async Task BuscarPorId_QuandoIdValidoEEscalaExistente_DeveRetornarEscalaDTO()
        {
            // Arrange
            var id = Guid.NewGuid();
            var escalaEntity = new Infra.Data.EntitiesDefesaCivilMarica.Escala { IdEscala = id, NmNomeEscala = "Escala Teste" };
            var escalaDTO = new EscalaDTO { IdEscala = id, NmNomeEscala = "Escala Teste", valido = true };
            _escalaRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(escalaEntity);
            _mapperMock.Setup(m => m.Map<EscalaDTO>(escalaEntity)).Returns(escalaDTO);

            // Act
            var result = await _escalaService.BuscarPorId(id);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.valido);
            Assert.Equal(id, result.IdEscala);
            Assert.Equal("Escala Teste", result.NmNomeEscala);
        }

        [Fact]
        public async Task BuscarPorId_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Act
            var result = await _escalaService.BuscarPorId(Guid.Empty);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Id fora do Range.", result.mensagem);
        }

        [Fact]
        public async Task BuscarPorId_QuandoEscalaNaoEncontrada_DeveRetornarInvalido()
        {
            // Arrange
            var id = Guid.NewGuid();
            _escalaRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Infra.Data.EntitiesDefesaCivilMarica.Escala)null);

            // Act
            var result = await _escalaService.BuscarPorId(id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Escala não encontrada.", result.mensagem);
        }

        [Fact]
        public async Task BuscarPorId_QuandoRepositorioFalha_DeveLancarExcecao()
        {
            // Arrange
            var id = Guid.NewGuid();
            var exceptionMessage = "Erro no banco";
            _escalaRepositoryMock.Setup(r => r.ObterPorIdAsync(id)).ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _escalaService.BuscarPorId(id));
            Assert.Contains(exceptionMessage, exception.Message);
        }
    }
}

