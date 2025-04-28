using Xunit;
using Moq;
using AutoMapper;
using GestaoEscalaPermutas.Dominio.Services.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestaoEscalaPermutas.Repository.Interfaces;

namespace GestaoEscalaPermutas.Tests.Services.PostoTrabalho
{
    public class PostoTrabalhoServiceTests
    {
        private readonly Mock<IPostoTrabalhoRepository> _postoTrabalhoRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly PostoTrabalhoService _postoTrabalhoService;

        public PostoTrabalhoServiceTests()
        {
            _postoTrabalhoRepositoryMock = new Mock<IPostoTrabalhoRepository>();
            _mapperMock = new Mock<IMapper>();
            _postoTrabalhoService = new PostoTrabalhoService(_postoTrabalhoRepositoryMock.Object, _mapperMock.Object);
        }

        // --- Testes para Incluir --- 

        [Fact]
        public async Task Incluir_QuandoPostoValido_DeveRetornarPostoDTOValido()
        {
            // Arrange
            var postoDTO = new PostoTrabalhoDTO { NmNome = "Posto A" };
            var postoEntity = new Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho { IdPostoTrabalho = Guid.NewGuid(), NmNome = "Posto A" };

            _mapperMock.Setup(m => m.Map<Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho>(postoDTO)).Returns(postoEntity);
            _postoTrabalhoRepositoryMock.Setup(r => r.IncluirAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho>())).ReturnsAsync(postoEntity);
            _mapperMock.Setup(m => m.Map<PostoTrabalhoDTO>(postoEntity)).Returns(postoDTO);

            // Act
            var result = await _postoTrabalhoService.Incluir(postoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.valido);
            Assert.Equal(postoDTO.NmNome, result.NmNome);
            _postoTrabalhoRepositoryMock.Verify(r => r.IncluirAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho>()), Times.Once);
        }

        [Fact]
        public async Task Incluir_QuandoPostoNulo_DeveRetornarInvalido()
        {
            // Act
            var result = await _postoTrabalhoService.Incluir(null);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Objeto não preenchido.", result.mensagem);
            _postoTrabalhoRepositoryMock.Verify(r => r.IncluirAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho>()), Times.Never);
        }

        [Fact]
        public async Task Incluir_QuandoRepositorioFalha_DeveRetornarErro()
        {
            // Arrange
            var postoDTO = new PostoTrabalhoDTO { NmNome = "Posto A" };
            var postoEntity = new Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho();
            var exceptionMessage = "Erro no banco";

            _mapperMock.Setup(m => m.Map<Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho>(postoDTO)).Returns(postoEntity);
            _postoTrabalhoRepositoryMock.Setup(r => r.IncluirAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho>())).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _postoTrabalhoService.Incluir(postoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Contains(exceptionMessage, result.mensagem);
        }

        // --- Testes para Alterar --- 

        [Fact]
        public async Task Alterar_QuandoIdValidoEPostoExistente_DeveRetornarPostoDTOAtualizado()
        {
            // Arrange
            var id = Guid.NewGuid();
            var postoDTO = new PostoTrabalhoDTO { IdPostoTrabalho = id, NmNome = "Posto B" };
            var postoExistente = new Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho { IdPostoTrabalho = id, NmNome = "Posto A" };

            _postoTrabalhoRepositoryMock.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(postoExistente);
            _mapperMock.Setup(m => m.Map(postoDTO, postoExistente)); // Simula atualização via AutoMapper
            _postoTrabalhoRepositoryMock.Setup(r => r.AlterarAsync(postoExistente)).ReturnsAsync(postoExistente); // Assume que AlterarAsync retorna a entidade atualizada
            _mapperMock.Setup(m => m.Map<PostoTrabalhoDTO>(postoExistente)).Returns(postoDTO);

            // Act
            var result = await _postoTrabalhoService.Alterar(id, postoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.valido);
            Assert.Equal(postoDTO.NmNome, result.NmNome);
            _postoTrabalhoRepositoryMock.Verify(r => r.AlterarAsync(postoExistente), Times.Once);
        }

        [Fact]
        public async Task Alterar_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Arrange
            var postoDTO = new PostoTrabalhoDTO();

            // Act
            var result = await _postoTrabalhoService.Alterar(Guid.Empty, postoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Id fora do Range.", result.mensagem);
            _postoTrabalhoRepositoryMock.Verify(r => r.AlterarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho>()), Times.Never);
        }

        [Fact]
        public async Task Alterar_QuandoPostoNaoEncontrado_DeveRetornarInvalido()
        {
            // Arrange
            var id = Guid.NewGuid();
            var postoDTO = new PostoTrabalhoDTO();
            _postoTrabalhoRepositoryMock.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync((Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho)null);

            // Act
            var result = await _postoTrabalhoService.Alterar(id, postoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Posto não encontrado.", result.mensagem);
            _postoTrabalhoRepositoryMock.Verify(r => r.AlterarAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho>()), Times.Never);
        }

        [Fact]
        public async Task Alterar_QuandoRepositorioFalha_DeveLancarExcecao()
        {
            // Arrange
            var id = Guid.NewGuid();
            var postoDTO = new PostoTrabalhoDTO { IdPostoTrabalho = id, NmNome = "Posto B" };
            var postoExistente = new Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho { IdPostoTrabalho = id, NmNome = "Posto A" };
            var exceptionMessage = "Erro no banco";

            _postoTrabalhoRepositoryMock.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(postoExistente);
            _mapperMock.Setup(m => m.Map(postoDTO, postoExistente));
            _postoTrabalhoRepositoryMock.Setup(r => r.AlterarAsync(postoExistente)).ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _postoTrabalhoService.Alterar(id, postoDTO));
            Assert.Contains(exceptionMessage, exception.Message);
        }

        // --- Testes para BuscarTodos --- 

        [Fact]
        public async Task BuscarTodos_QuandoExistemPostos_DeveRetornarListaDTO()
        {
            // Arrange
            var postosEntity = new List<Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho> { new Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho { IdPostoTrabalho = Guid.NewGuid() } };
            var postosDTO = new List<PostoTrabalhoDTO> { new PostoTrabalhoDTO { valido = true } }; // Assume que o mapper preenche valido
            _postoTrabalhoRepositoryMock.Setup(r => r.BuscarTodosAsync()).ReturnsAsync(postosEntity);
            _mapperMock.Setup(m => m.Map<List<PostoTrabalhoDTO>>(postosEntity)).Returns(postosDTO);

            // Act
            var result = await _postoTrabalhoService.BuscarTodos();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result[0].valido);
        }

        [Fact]
        public async Task BuscarTodos_QuandoNaoExistemPostos_DeveRetornarListaVazia()
        {
            // Arrange
            var postosEntity = new List<Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho>();
            var postosDTO = new List<PostoTrabalhoDTO>();
            _postoTrabalhoRepositoryMock.Setup(r => r.BuscarTodosAsync()).ReturnsAsync(postosEntity);
            _mapperMock.Setup(m => m.Map<List<PostoTrabalhoDTO>>(postosEntity)).Returns(postosDTO);

            // Act
            var result = await _postoTrabalhoService.BuscarTodos();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task BuscarTodos_QuandoRepositorioFalha_DeveLancarExcecao()
        {
            // Arrange
            var exceptionMessage = "Erro no banco";
            _postoTrabalhoRepositoryMock.Setup(r => r.BuscarTodosAsync()).ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _postoTrabalhoService.BuscarTodos());
            Assert.Contains(exceptionMessage, exception.Message);
        }

        // --- Testes para Deletar --- 

        [Fact]
        public async Task Deletar_QuandoIdValidoEPostoExistente_DeveRetornarSucesso()
        {
            // Arrange
            var id = Guid.NewGuid();
            _postoTrabalhoRepositoryMock.Setup(r => r.DeletarAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _postoTrabalhoService.Deletar(id);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.valido);
            Assert.Equal("Posto de trabalho deletado com sucesso.", result.mensagem);
            _postoTrabalhoRepositoryMock.Verify(r => r.DeletarAsync(id), Times.Once);
        }

        [Fact]
        public async Task Deletar_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Act
            var result = await _postoTrabalhoService.Deletar(Guid.Empty);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Id fora do Range.", result.mensagem);
            _postoTrabalhoRepositoryMock.Verify(r => r.DeletarAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Deletar_QuandoPostoNaoEncontrado_DeveRetornarInvalido()
        {
            // Arrange
            var id = Guid.NewGuid();
            _postoTrabalhoRepositoryMock.Setup(r => r.DeletarAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _postoTrabalhoService.Deletar(id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.valido);
            Assert.Equal("Posto não encontrado.", result.mensagem);
            _postoTrabalhoRepositoryMock.Verify(r => r.DeletarAsync(id), Times.Once);
        }

        [Fact]
        public async Task Deletar_QuandoRepositorioFalha_DeveLancarExcecao()
        {
            // Arrange
            var id = Guid.NewGuid();
            var exceptionMessage = "Erro no banco";
            _postoTrabalhoRepositoryMock.Setup(r => r.DeletarAsync(id)).ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _postoTrabalhoService.Deletar(id));
            Assert.Contains(exceptionMessage, exception.Message);
        }

        // --- Testes para IncluirLista --- 

        [Fact]
        public async Task IncluirLista_QuandoListaValida_DeveRetornarListaDTO()
        {
            // Arrange
            var postoDTOs = new PostoTrabalhoDTO[] { new PostoTrabalhoDTO { NmNome = "Posto1" }, new PostoTrabalhoDTO { NmNome = "Posto2" } };
            var postosEntity = new Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho[] { new Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho { IdPostoTrabalho = Guid.NewGuid() }, new Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho { IdPostoTrabalho = Guid.NewGuid() } };
            
            _mapperMock.Setup(m => m.Map<Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho[]>(postoDTOs)).Returns(postosEntity);
            _postoTrabalhoRepositoryMock.Setup(r => r.IncluirListaAsync(postosEntity)).ReturnsAsync(postosEntity);
            _mapperMock.Setup(m => m.Map<PostoTrabalhoDTO[]>(postosEntity)).Returns(postoDTOs);

            // Act
            var result = await _postoTrabalhoService.IncluirLista(postoDTOs);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Length);
            Assert.Equal("Posto1", result[0].NmNome);
            _postoTrabalhoRepositoryMock.Verify(r => r.IncluirListaAsync(postosEntity), Times.Once);
        }

        [Fact]
        public async Task IncluirLista_QuandoListaNula_DeveRetornarInvalido()
        {
            // Arrange
            PostoTrabalhoDTO[] postoDTOs = null;

            // Act
            var result = await _postoTrabalhoService.IncluirLista(postoDTOs);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.False(result[0].valido);
            Assert.Equal("Lista de postos vazia.", result[0].mensagem);
            _postoTrabalhoRepositoryMock.Verify(r => r.IncluirListaAsync(It.IsAny<Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho[]>()), Times.Never);
        }
        
        [Fact]
        public async Task IncluirLista_QuandoRepositorioFalha_DeveRetornarErro()
        {
            // Arrange
            var postoDTOs = new PostoTrabalhoDTO[] { new PostoTrabalhoDTO { NmNome = "Posto1" } };
            var postosEntity = new Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho[] { new Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho() };
            var exceptionMessage = "Erro no banco";

            _mapperMock.Setup(m => m.Map<Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho[]>(postoDTOs)).Returns(postosEntity);
            _postoTrabalhoRepositoryMock.Setup(r => r.IncluirListaAsync(postosEntity)).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _postoTrabalhoService.IncluirLista(postoDTOs);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.False(result[0].valido);
            Assert.Contains(exceptionMessage, result[0].mensagem);
        }

        // --- Testes para BuscarTodosAtivos --- 

        [Fact]
        public async Task BuscarTodosAtivos_QuandoExistemPostosAtivos_DeveRetornarListaDTO()
        {
            // Arrange
            var postosEntity = new List<Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho> { new Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho { IdPostoTrabalho = Guid.NewGuid(), IsAtivo = true } };
            var postosDTO = new List<PostoTrabalhoDTO> { new PostoTrabalhoDTO { valido = true, IsAtivo = true } };
            _postoTrabalhoRepositoryMock.Setup(r => r.BuscarTodosAtivosAsync()).ReturnsAsync(postosEntity);
            _mapperMock.Setup(m => m.Map<List<PostoTrabalhoDTO>>(postosEntity)).Returns(postosDTO);

            // Act
            var result = await _postoTrabalhoService.BuscarTodosAtivos();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result[0].valido);
            Assert.True(result[0].IsAtivo);
        }

        [Fact]
        public async Task BuscarTodosAtivos_QuandoNaoExistemPostosAtivos_DeveRetornarListaVazia()
        {
            // Arrange
            var postosEntity = new List<Infra.Data.EntitiesDefesaCivilMarica.PostoTrabalho>();
            var postosDTO = new List<PostoTrabalhoDTO>();
            _postoTrabalhoRepositoryMock.Setup(r => r.BuscarTodosAtivosAsync()).ReturnsAsync(postosEntity);
            _mapperMock.Setup(m => m.Map<List<PostoTrabalhoDTO>>(postosEntity)).Returns(postosDTO);

            // Act
            var result = await _postoTrabalhoService.BuscarTodosAtivos();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task BuscarTodosAtivos_QuandoRepositorioFalha_DeveLancarExcecao()
        {
            // Arrange
            var exceptionMessage = "Erro no banco";
            _postoTrabalhoRepositoryMock.Setup(r => r.BuscarTodosAtivosAsync()).ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _postoTrabalhoService.BuscarTodosAtivos());
            Assert.Contains(exceptionMessage, exception.Message);
        }
    }
}

