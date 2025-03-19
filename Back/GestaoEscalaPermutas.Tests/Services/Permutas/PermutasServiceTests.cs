using Xunit;
using Moq;
using AutoMapper;
using GestaoEscalaPermutas.Dominio.Services.Permutas;
using GestaoEscalaPermutas.Dominio.DTO.Permutas;
using GestaoEscalaPermutas.Repository.Interfaces;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Tests.Services
{
    public class PermutasServiceTests
    {
        private readonly Mock<IPermutasRepository> _permutasRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly PermutasService _permutasService;

        public PermutasServiceTests()
        {
            _permutasRepositoryMock = new Mock<IPermutasRepository>();
            _mapperMock = new Mock<IMapper>();
            _permutasService = new PermutasService(_permutasRepositoryMock.Object, _mapperMock.Object);
        }

        // Testes para Incluir
        [Fact]
        public async Task Incluir_QuandoPermutaValida_DeveRetornarPermutaDTOValida()
        {
            // Arrange
            var permutaDTO = new PermutasDTO
            {
                IdEscala = Guid.NewGuid(),
                IdFuncionarioSolicitante = Guid.NewGuid(),
                NmNomeSolicitante = "João",
                IdFuncionarioSolicitado = Guid.NewGuid(),
                NmNomeSolicitado = "Maria",
                DtSolicitacao = DateTime.UtcNow,
                DtDataSolicitadaTroca = DateTime.UtcNow.AddDays(1)
            };
            var permutaEntity = new Permuta { IdPermuta = Guid.NewGuid() };
            _mapperMock.Setup(m => m.Map<Permuta>(permutaDTO)).Returns(permutaEntity);
            _mapperMock.Setup(m => m.Map<PermutasDTO>(permutaEntity)).Returns(permutaDTO);
            _permutasRepositoryMock.Setup(r => r.IncluirAsync(It.IsAny<Permuta>())).ReturnsAsync(permutaEntity);

            // Act
            var result = await _permutasService.Incluir(permutaDTO);

            // Assert
            Assert.True(result.valido);
            Assert.Equal(permutaDTO.NmNomeSolicitante, result.NmNomeSolicitante);
        }

        [Fact]
        public async Task Incluir_QuandoPermutaNula_DeveRetornarInvalido()
        {
            // Act
            var result = await _permutasService.Incluir(null);

            // Assert
            Assert.False(result.valido);
            Assert.Equal("Objeto não preenchido.", result.mensagem);
        }

        [Fact]
        public async Task Incluir_QuandoRepositoryFalha_DeveRetornarErro()
        {
            // Arrange
            var permutaDTO = new PermutasDTO { NmNomeSolicitante = "João" };
            var permutaEntity = new Permuta();
            _mapperMock.Setup(m => m.Map<Permuta>(permutaDTO)).Returns(permutaEntity);
            _permutasRepositoryMock.Setup(r => r.IncluirAsync(It.IsAny<Permuta>())).ThrowsAsync(new Exception("Erro no banco"));

            // Act
            var result = await _permutasService.Incluir(permutaDTO);

            // Assert
            Assert.False(result.valido);
            Assert.Contains("Erro ao incluir permuta", result.mensagem);
        }

        // Testes para Alterar
        [Fact]
        public async Task Alterar_QuandoIdValidoEExistente_DeveRetornarPermutaAtualizada()
        {
            // Arrange
            var id = Guid.NewGuid();
            var permutaDTOInput = new PermutasDTO
            {
                IdEscala = Guid.NewGuid(),
                IdFuncionarioSolicitante = Guid.NewGuid(),
                NmNomeSolicitante = "João Atualizado",
                IdFuncionarioSolicitado = Guid.NewGuid(),
                NmNomeSolicitado = "Maria",
                DtSolicitacao = DateTime.UtcNow,
                DtDataSolicitadaTroca = DateTime.UtcNow.AddDays(1)
            };
            var permutaExistente = new Permuta
            {
                IdPermuta = id,
                IdEscala = Guid.NewGuid(),
                IdFuncionarioSolicitante = Guid.NewGuid(),
                NmNomeSolicitante = "João",
                IdFuncionarioSolicitado = Guid.NewGuid(),
                NmNomeSolicitado = "Maria",
                DtSolicitacao = DateTime.UtcNow,
                DtDataSolicitadaTroca = DateTime.UtcNow.AddDays(1)
            };
            var permutaAtualizada = new Permuta
            {
                IdPermuta = id,
                IdEscala = permutaDTOInput.IdEscala,
                IdFuncionarioSolicitante = permutaDTOInput.IdFuncionarioSolicitante,
                NmNomeSolicitante = "João Atualizado",
                IdFuncionarioSolicitado = permutaDTOInput.IdFuncionarioSolicitado,
                NmNomeSolicitado = "Maria",
                DtSolicitacao = permutaDTOInput.DtSolicitacao,
                DtDataSolicitadaTroca = permutaDTOInput.DtDataSolicitadaTroca
            };
            var permutaDTOResult = new PermutasDTO
            {
                IdPermuta = id,
                IdEscala = permutaDTOInput.IdEscala,
                IdFuncionarioSolicitante = permutaDTOInput.IdFuncionarioSolicitante,
                NmNomeSolicitante = "João Atualizado",
                IdFuncionarioSolicitado = permutaDTOInput.IdFuncionarioSolicitado,
                NmNomeSolicitado = "Maria",
                DtSolicitacao = permutaDTOInput.DtSolicitacao,
                DtDataSolicitadaTroca = permutaDTOInput.DtDataSolicitadaTroca,
                valido = true,
                mensagem = "Registro atualizado com sucesso" // Adicionando mensagem para consistência
            };

            _permutasRepositoryMock.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(permutaExistente);
            _mapperMock.Setup(m => m.Map(It.IsAny<PermutasDTO>(), It.IsAny<Permuta>())).Returns(permutaAtualizada);
            _permutasRepositoryMock.Setup(r => r.AlterarAsync(It.IsAny<Permuta>())).ReturnsAsync(permutaAtualizada);
            _mapperMock.Setup(m => m.Map<PermutasDTO>(It.IsAny<Permuta>())).Returns(permutaDTOResult);

            // Act
            var result = await _permutasService.Alterar(id, permutaDTOInput);

            // Assert
            Assert.NotNull(result); // Verifica se o resultado não é nulo
            Assert.True(result.valido, "O resultado deveria ser válido");
            Assert.Equal("João Atualizado", result.NmNomeSolicitante);
        }
        [Fact]
        public async Task Alterar_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Act
            var result = await _permutasService.Alterar(Guid.Empty, new PermutasDTO());

            // Assert
            Assert.False(result.valido);
            Assert.Equal("Id fora do Range.", result.mensagem);
        }

        [Fact]
        public async Task Alterar_QuandoPermutaNaoExiste_DeveRetornarInvalido()
        {
            // Arrange
            var id = Guid.NewGuid();
            _permutasRepositoryMock.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync((Permuta)null);

            // Act
            var result = await _permutasService.Alterar(id, new PermutasDTO());

            // Assert
            Assert.False(result.valido);
            Assert.Equal("Permuta não encontrada.", result.mensagem);
        }

        [Fact]
        public async Task Alterar_QuandoRepositoryFalha_DeveLancarExcecao()
        {
            // Arrange
            var id = Guid.NewGuid();
            var permutaExistente = new Permuta { IdPermuta = id };
            _permutasRepositoryMock.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(permutaExistente);
            _permutasRepositoryMock.Setup(r => r.AlterarAsync(It.IsAny<Permuta>())).ThrowsAsync(new Exception("Erro no banco"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _permutasService.Alterar(id, new PermutasDTO()));
            Assert.Equal("Erro ao alterar permuta: Erro no banco", exception.Message);
        }

        // Testes para BuscarPorId
        [Fact]
        public async Task BuscarPorId_QuandoIdValido_DeveRetornarPermuta()
        {
            // Arrange
            var id = Guid.NewGuid();
            var permutaEntity = new Permuta { IdPermuta = id, NmNomeSolicitante = "João" };
            var permutaDTO = new PermutasDTO { NmNomeSolicitante = "João" };
            _permutasRepositoryMock.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(permutaEntity);
            _mapperMock.Setup(m => m.Map<PermutasDTO>(permutaEntity)).Returns(permutaDTO);

            // Act
            var result = await _permutasService.BuscarPorId(id);

            // Assert
            Assert.True(result.valido);
            Assert.Equal("João", result.NmNomeSolicitante);
        }

        [Fact]
        public async Task BuscarPorId_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Act
            var result = await _permutasService.BuscarPorId(Guid.Empty);

            // Assert
            Assert.False(result.valido);
            Assert.Equal("Id fora do Range.", result.mensagem);
        }

        [Fact]
        public async Task BuscarPorId_QuandoNaoEncontrado_DeveRetornarInvalido()
        {
            // Arrange
            var id = Guid.NewGuid();
            _permutasRepositoryMock.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync((Permuta)null);

            // Act
            var result = await _permutasService.BuscarPorId(id);

            // Assert
            Assert.False(result.valido);
            Assert.Equal("Permuta não encontrada.", result.mensagem);
        }

        // Testes para BuscarTodos
        [Fact]
        public async Task BuscarTodos_QuandoExistemPermutas_DeveRetornarLista()
        {
            // Arrange
            var permutasEntity = new List<Permuta> { new Permuta { IdPermuta = Guid.NewGuid() } };
            var permutasDTO = new List<PermutasDTO> { new PermutasDTO { valido = true } };
            _permutasRepositoryMock.Setup(r => r.BuscarTodosAsync()).ReturnsAsync(permutasEntity);
            _mapperMock.Setup(m => m.Map<List<PermutasDTO>>(permutasEntity)).Returns(permutasDTO);

            // Act
            var result = await _permutasService.BuscarTodos();

            // Assert
            Assert.NotEmpty(result);
            Assert.True(result[0].valido);
        }

        [Fact]
        public async Task BuscarTodos_QuandoRepositoryFalha_DeveLancarExcecao()
        {
            // Arrange
            _permutasRepositoryMock.Setup(r => r.BuscarTodosAsync()).ThrowsAsync(new Exception("Erro no banco"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _permutasService.BuscarTodos());
            Assert.Equal("Erro ao buscar todas as permutas: Erro no banco", exception.Message);
        }

        // Testes para Deletar
        [Fact]
        public async Task Deletar_QuandoIdValido_DeveRetornarSucesso()
        {
            // Arrange
            var id = Guid.NewGuid();
            _permutasRepositoryMock.Setup(r => r.DeletarAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _permutasService.Deletar(id);

            // Assert
            Assert.True(result.valido);
            Assert.Equal("Permuta deletada com sucesso.", result.mensagem);
        }

        [Fact]
        public async Task Deletar_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Act
            var result = await _permutasService.Deletar(Guid.Empty);

            // Assert
            Assert.False(result.valido);
            Assert.Equal("Id fora do Range.", result.mensagem);
        }

        [Fact]
        public async Task Deletar_QuandoNaoEncontrado_DeveRetornarInvalido()
        {
            // Arrange
            var id = Guid.NewGuid();
            _permutasRepositoryMock.Setup(r => r.DeletarAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _permutasService.Deletar(id);

            // Assert
            Assert.False(result.valido);
            Assert.Equal("Permuta não encontrada.", result.mensagem);
        }

        // Testes para BuscarFuncPorId
        [Fact]
        public async Task BuscarFuncPorId_QuandoIdValido_DeveRetornarLista()
        {
            // Arrange
            var id = Guid.NewGuid();
            var permutasEntity = new List<Permuta> { new Permuta { IdPermuta = Guid.NewGuid() } };
            var permutasDTO = new List<PermutasDTO> { new PermutasDTO { valido = true } };
            _permutasRepositoryMock.Setup(r => r.BuscarFuncPorIdAsync(id)).ReturnsAsync(permutasEntity);
            _mapperMock.Setup(m => m.Map<List<PermutasDTO>>(permutasEntity)).Returns(permutasDTO);

            // Act
            var result = await _permutasService.BuscarFuncPorId(id);

            // Assert
            Assert.NotEmpty(result);
            Assert.True(result[0].valido);
        }

        [Fact]
        public async Task BuscarFuncPorId_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Act
            var result = await _permutasService.BuscarFuncPorId(Guid.Empty);

            // Assert
            Assert.Single(result);
            Assert.False(result[0].valido);
            Assert.Equal("Id fora do Range.", result[0].mensagem);
        }

        [Fact]
        public async Task BuscarFuncPorId_QuandoNaoEncontrado_DeveRetornarInvalido()
        {
            // Arrange
            var id = Guid.NewGuid();
            _permutasRepositoryMock.Setup(r => r.BuscarFuncPorIdAsync(id)).ReturnsAsync(new List<Permuta>());

            // Act
            var result = await _permutasService.BuscarFuncPorId(id);

            // Assert
            Assert.Single(result);
            Assert.False(result[0].valido);
            Assert.Equal("Permutas não encontradas.", result[0].mensagem);
        }

        // Testes para BuscarSolicitacoesPorId
        [Fact]
        public async Task BuscarSolicitacoesPorId_QuandoIdValido_DeveRetornarLista()
        {
            // Arrange
            var id = Guid.NewGuid();
            var permutasEntity = new List<Permuta> { new Permuta { IdPermuta = Guid.NewGuid() } };
            var permutasDTO = new List<PermutasDTO> { new PermutasDTO { valido = true } };
            _permutasRepositoryMock.Setup(r => r.BuscaSolicitacoesPorIdAsync(id)).ReturnsAsync(permutasEntity);
            _mapperMock.Setup(m => m.Map<List<PermutasDTO>>(permutasEntity)).Returns(permutasDTO);

            // Act
            var result = await _permutasService.BuscarSolicitacoesPorId(id);

            // Assert
            Assert.NotEmpty(result);
            Assert.True(result[0].valido);
        }

        [Fact]
        public async Task BuscarSolicitacoesPorId_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Act
            var result = await _permutasService.BuscarSolicitacoesPorId(Guid.Empty);

            // Assert
            Assert.Single(result);
            Assert.False(result[0].valido);
            Assert.Equal("Id fora do Range.", result[0].mensagem);
        }

        // Testes para BuscarSolicitacoesFuncPorId
        [Fact]
        public async Task BuscarSolicitacoesFuncPorId_QuandoIdValido_DeveRetornarLista()
        {
            // Arrange
            var id = Guid.NewGuid();
            var permutasEntity = new List<Permuta> { new Permuta { IdPermuta = Guid.NewGuid() } };
            var permutasDTO = new List<PermutasDTO> { new PermutasDTO { valido = true } };
            _permutasRepositoryMock.Setup(r => r.BuscarSolicitacoesFuncPorIdAsync(id)).ReturnsAsync(permutasEntity);
            _mapperMock.Setup(m => m.Map<List<PermutasDTO>>(permutasEntity)).Returns(permutasDTO);

            // Act
            var result = await _permutasService.BuscarSolicitacoesFuncPorId(id);

            // Assert
            Assert.NotEmpty(result);
            Assert.True(result[0].valido);
        }

        [Fact]
        public async Task BuscarSolicitacoesFuncPorId_QuandoIdVazio_DeveRetornarInvalido()
        {
            // Act
            var result = await _permutasService.BuscarSolicitacoesFuncPorId(Guid.Empty);

            // Assert
            Assert.Single(result);
            Assert.False(result[0].valido);
            Assert.Equal("Id fora do Range.", result[0].mensagem);
        }
    }
}