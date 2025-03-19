using Xunit;
using Microsoft.EntityFrameworkCore;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Implementations;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace GestaoEscalaPermutas.Tests.Repositories
{
    public class PermutasRepositoryTests
    {
        private readonly DbContextOptions<DefesaCivilMaricaContext> _options;

        public PermutasRepositoryTests()
        {
            // Configura o banco de dados em memória
            _options = new DbContextOptionsBuilder<DefesaCivilMaricaContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Nome único para cada teste
                .Options;
        }

        private DefesaCivilMaricaContext CreateContext()
        {
            return new DefesaCivilMaricaContext(_options);
        }

        // Testes para IncluirAsync
        [Fact]
        public async Task IncluirAsync_QuandoPermutaValida_DeveIncluirNoBanco()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new PermutasRepository(context);
            var permuta = new Permuta
            {
                IdPermuta = Guid.NewGuid(),
                IdEscala = Guid.NewGuid(),
                IdFuncionarioSolicitante = Guid.NewGuid(),
                NmNomeSolicitante = "João",
                IdFuncionarioSolicitado = Guid.NewGuid(),
                NmNomeSolicitado = "Maria",
                DtSolicitacao = DateTime.UtcNow,
                DtDataSolicitadaTroca = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = await repository.IncluirAsync(permuta);

            // Assert
            var permutaNoBanco = await context.Permuta.FindAsync(permuta.IdPermuta);
            Assert.NotNull(permutaNoBanco);
            Assert.Equal(permuta.NmNomeSolicitante, permutaNoBanco.NmNomeSolicitante);
            Assert.Equal(permuta.IdPermuta, result.IdPermuta);
        }

        // Testes para AlterarAsync
        [Fact]
        public async Task AlterarAsync_QuandoPermutaExistente_DeveAtualizarNoBanco()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new PermutasRepository(context);
            var permuta = new Permuta
            {
                IdPermuta = Guid.NewGuid(),
                IdEscala = Guid.NewGuid(),
                IdFuncionarioSolicitante = Guid.NewGuid(),
                NmNomeSolicitante = "João",
                IdFuncionarioSolicitado = Guid.NewGuid(),
                NmNomeSolicitado = "Maria",
                DtSolicitacao = DateTime.UtcNow,
                DtDataSolicitadaTroca = DateTime.UtcNow.AddDays(1)
            };
            await context.Permuta.AddAsync(permuta);
            await context.SaveChangesAsync();

            permuta.NmNomeSolicitante = "João Atualizado";

            // Act
            var result = await repository.AlterarAsync(permuta);

            // Assert
            var permutaNoBanco = await context.Permuta.FindAsync(permuta.IdPermuta);
            Assert.NotNull(permutaNoBanco);
            Assert.Equal("João Atualizado", permutaNoBanco.NmNomeSolicitante);
            Assert.Equal(permuta.IdPermuta, result.IdPermuta);
        }

        // Testes para BuscarPorIdAsync
        [Fact]
        public async Task BuscarPorIdAsync_QuandoIdExistente_DeveRetornarPermuta()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new PermutasRepository(context);
            var permuta = new Permuta
            {
                IdPermuta = Guid.NewGuid(),
                IdEscala = Guid.NewGuid(),
                IdFuncionarioSolicitante = Guid.NewGuid(),
                NmNomeSolicitante = "João",
                IdFuncionarioSolicitado = Guid.NewGuid(),
                NmNomeSolicitado = "Maria", // Adicionado
                DtSolicitacao = DateTime.UtcNow,
                DtDataSolicitadaTroca = DateTime.UtcNow.AddDays(1)
            };
            await context.Permuta.AddAsync(permuta);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.BuscarPorIdAsync(permuta.IdPermuta);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(permuta.IdPermuta, result.IdPermuta);
            Assert.Equal("João", result.NmNomeSolicitante);
        }
        [Fact]
        public async Task BuscarPorIdAsync_QuandoIdNaoExiste_DeveRetornarNulo()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new PermutasRepository(context);

            // Act
            var result = await repository.BuscarPorIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        // Testes para BuscarTodosAsync
        [Fact]
        public async Task BuscarTodosAsync_QuandoExistemPermutas_DeveRetornarLista()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new PermutasRepository(context);

            var permutas = new List<Permuta>
    {
        new Permuta
        {
            IdPermuta = Guid.NewGuid(),
            IdEscala = Guid.NewGuid(),
            IdFuncionarioSolicitante = Guid.NewGuid(),
            NmNomeSolicitante = "João",
            IdFuncionarioSolicitado = Guid.NewGuid(),
            NmNomeSolicitado = "Pedro", // Adicionado para evitar erro
            DtSolicitacao = DateTime.UtcNow,
            DtDataSolicitadaTroca = DateTime.UtcNow.AddDays(1)
        },
        new Permuta
        {
            IdPermuta = Guid.NewGuid(),
            IdEscala = Guid.NewGuid(),
            IdFuncionarioSolicitante = Guid.NewGuid(),
            NmNomeSolicitante = "Maria",
            IdFuncionarioSolicitado = Guid.NewGuid(),
            NmNomeSolicitado = "Carlos", // Adicionado para evitar erro
            DtSolicitacao = DateTime.UtcNow,
            DtDataSolicitadaTroca = DateTime.UtcNow.AddDays(1)
        }
    };

            await context.Permuta.AddRangeAsync(permutas);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.BuscarTodosAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.NmNomeSolicitante == "João");
            Assert.Contains(result, p => p.NmNomeSolicitante == "Maria");
        }

        [Fact]
        public async Task BuscarTodosAsync_QuandoNaoExistemPermutas_DeveRetornarListaVazia()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new PermutasRepository(context);

            // Act
            var result = await repository.BuscarTodosAsync();

            // Assert
            Assert.Empty(result);
        }

        // Testes para DeletarAsync
        [Fact]
        public async Task DeletarAsync_QuandoIdExistente_DeveRemoverERetornarTrue()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new PermutasRepository(context);
            var permuta = new Permuta
            {
                IdPermuta = Guid.NewGuid(),
                IdEscala = Guid.NewGuid(),
                IdFuncionarioSolicitante = Guid.NewGuid(),
                NmNomeSolicitante = "João",
                IdFuncionarioSolicitado = Guid.NewGuid(),
                NmNomeSolicitado = "Carlos", // Corrigido
                DtSolicitacao = DateTime.UtcNow,
                DtDataSolicitadaTroca = DateTime.UtcNow.AddDays(1)
            };

            await context.Permuta.AddAsync(permuta);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.DeletarAsync(permuta.IdPermuta);

            // Assert
            Assert.True(result);
            var permutaNoBanco = await context.Permuta.FindAsync(permuta.IdPermuta);
            Assert.Null(permutaNoBanco);
        }

        [Fact]
        public async Task DeletarAsync_QuandoIdNaoExiste_DeveRetornarFalse()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new PermutasRepository(context);

            // Act
            var result = await repository.DeletarAsync(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }

        // Testes para BuscarFuncPorIdAsync
        [Fact]
        public async Task BuscarFuncPorIdAsync_QuandoIdValido_DeveRetornarPermutasDoSolicitante()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new PermutasRepository(context);
            var idFuncionario = Guid.NewGuid();
            var permutas = new List<Permuta>
    {
        new Permuta
        {
            IdPermuta = Guid.NewGuid(),
            IdEscala = Guid.NewGuid(),
            IdFuncionarioSolicitante = idFuncionario,
            NmNomeSolicitante = "João",
            IdFuncionarioSolicitado = Guid.NewGuid(),
            NmNomeSolicitado = "Maria", // Adicionado
            DtSolicitacao = DateTime.UtcNow,
            DtDataSolicitadaTroca = DateTime.UtcNow.AddDays(1)
        },
        new Permuta
        {
            IdPermuta = Guid.NewGuid(),
            IdEscala = Guid.NewGuid(),
            IdFuncionarioSolicitante = Guid.NewGuid(),
            NmNomeSolicitante = "Maria",
            IdFuncionarioSolicitado = Guid.NewGuid(),
            NmNomeSolicitado = "José", // Adicionado
            DtSolicitacao = DateTime.UtcNow,
            DtDataSolicitadaTroca = DateTime.UtcNow.AddDays(1)
        }
    };
            await context.Permuta.AddRangeAsync(permutas);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.BuscarFuncPorIdAsync(idFuncionario);

            // Assert
            Assert.Single(result);
            Assert.Equal("João", result[0].NmNomeSolicitante);
        }

        [Fact]
        public async Task BuscarFuncPorIdAsync_QuandoIdVazio_DeveRetornarListaVazia()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new PermutasRepository(context);

            // Act
            var result = await repository.BuscarFuncPorIdAsync(Guid.Empty);

            // Assert
            Assert.Empty(result);
        }

        // Testes para BuscaSolicitacoesPorIdAsync
        [Fact]
        public async Task BuscaSolicitacoesPorIdAsync_QuandoIdValido_DeveRetornarPermutasRelacionadas()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new PermutasRepository(context);
            var idFuncionario = Guid.NewGuid();

            var permutas = new List<Permuta>
    {
        new Permuta
        {
            IdPermuta = Guid.NewGuid(),
            IdEscala = Guid.NewGuid(),
            IdFuncionarioSolicitante = idFuncionario,
            NmNomeSolicitante = "João",
            IdFuncionarioSolicitado = Guid.NewGuid(),
            NmNomeSolicitado = "Carlos", // Corrigido
            DtSolicitacao = DateTime.UtcNow,
            DtDataSolicitadaTroca = DateTime.UtcNow.AddDays(1)
        },
        new Permuta
        {
            IdPermuta = Guid.NewGuid(),
            IdEscala = Guid.NewGuid(),
            IdFuncionarioSolicitante = Guid.NewGuid(),
            NmNomeSolicitante = "Maria",
            IdFuncionarioSolicitado = idFuncionario,
            NmNomeSolicitado = "Pedro", // Corrigido
            DtSolicitacao = DateTime.UtcNow,
            DtDataSolicitadaTroca = DateTime.UtcNow.AddDays(1)
        }
    };

            await context.Permuta.AddRangeAsync(permutas);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.BuscaSolicitacoesPorIdAsync(idFuncionario);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.NmNomeSolicitante == "João");
            Assert.Contains(result, p => p.NmNomeSolicitante == "Maria");
        }

        [Fact]
        public async Task BuscaSolicitacoesPorIdAsync_QuandoIdVazio_DeveRetornarListaVazia()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new PermutasRepository(context);

            // Act
            var result = await repository.BuscaSolicitacoesPorIdAsync(Guid.Empty);

            // Assert
            Assert.Empty(result);
        }

        // Testes para BuscarSolicitacoesFuncPorIdAsync
        [Fact]
        public async Task BuscarSolicitacoesFuncPorIdAsync_QuandoIdValidoEStatusNulo_DeveRetornarPermutas()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new PermutasRepository(context);
            var idFuncionario = Guid.NewGuid();
            var permutas = new List<Permuta>
    {
        new Permuta
        {
            IdPermuta = Guid.NewGuid(),
            IdEscala = Guid.NewGuid(),
            IdFuncionarioSolicitante = Guid.NewGuid(),
            NmNomeSolicitante = "João",
            IdFuncionarioSolicitado = idFuncionario,
            NmNomeSolicitado = "José", // Adicionado
            DtSolicitacao = DateTime.UtcNow,
            DtDataSolicitadaTroca = DateTime.UtcNow.AddDays(1),
            NmStatus = null
        },
        new Permuta
        {
            IdPermuta = Guid.NewGuid(),
            IdEscala = Guid.NewGuid(),
            IdFuncionarioSolicitante = Guid.NewGuid(),
            NmNomeSolicitante = "Maria",
            IdFuncionarioSolicitado = idFuncionario,
            NmNomeSolicitado = "Pedro", // Adicionado
            DtSolicitacao = DateTime.UtcNow,
            DtDataSolicitadaTroca = DateTime.UtcNow.AddDays(1),
            NmStatus = "Aprovada"
        }
    };
            await context.Permuta.AddRangeAsync(permutas);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.BuscarSolicitacoesFuncPorIdAsync(idFuncionario);

            // Assert
            Assert.Single(result);
            Assert.Equal("João", result[0].NmNomeSolicitante);
            Assert.Null(result[0].NmStatus);
        }
        [Fact]
        public async Task BuscarSolicitacoesFuncPorIdAsync_QuandoIdVazio_DeveRetornarListaVazia()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new PermutasRepository(context);

            // Act
            var result = await repository.BuscarSolicitacoesFuncPorIdAsync(Guid.Empty);

            // Assert
            Assert.Empty(result);
        }
    }
}