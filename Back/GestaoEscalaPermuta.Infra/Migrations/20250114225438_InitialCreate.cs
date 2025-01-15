using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestaoEscalaPermutas.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cargos",
                columns: table => new
                {
                    IdCargos = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NmNome = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: false),
                    NmDescricao = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: true),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    DtCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdCargos", x => x.IdCargos);
                });

            migrationBuilder.CreateTable(
                name: "Departamento",
                columns: table => new
                {
                    IdDepartamento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NmNome = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: false),
                    NmDescricao = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: true),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    DtCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdDepartamento", x => x.IdDepartamento);
                });

            migrationBuilder.CreateTable(
                name: "EMPRESA",
                columns: table => new
                {
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NmNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NmEndereco = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NrCNPJ = table.Column<int>(type: "integer", nullable: false),
                    NrTelefone = table.Column<int>(type: "integer", nullable: false),
                    NmEmail = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdEmpresa", x => x.IdEmpresa);
                });

            migrationBuilder.CreateTable(
                name: "Escala",
                columns: table => new
                {
                    IdEscala = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdDepartamento = table.Column<int>(type: "integer", nullable: false),
                    NmNomeEscala = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: true),
                    IdTipoEscala = table.Column<int>(type: "integer", nullable: false),
                    DtCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NrMesReferencia = table.Column<int>(type: "integer", nullable: false),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    IsGerada = table.Column<bool>(type: "boolean", nullable: false),
                    NrPessoaPorPosto = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdEscala", x => x.IdEscala);
                });

            migrationBuilder.CreateTable(
                name: "EscalaPronta",
                columns: table => new
                {
                    IdEscalaPronta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEscala = table.Column<int>(type: "integer", nullable: false),
                    IdPostoTrabalho = table.Column<int>(type: "integer", nullable: false),
                    IdFuncionario = table.Column<int>(type: "integer", nullable: false),
                    DtDataServico = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DtCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscalaPronta", x => x.IdEscalaPronta);
                });

            migrationBuilder.CreateTable(
                name: "Funcionalidades",
                columns: table => new
                {
                    IdFuncionalidade = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NmNome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NmDescricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdFuncionalidade", x => x.IdFuncionalidade);
                });

            migrationBuilder.CreateTable(
                name: "Funcionario",
                columns: table => new
                {
                    IdFuncionario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NmNome = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: false),
                    NrMatricula = table.Column<int>(type: "integer", nullable: false),
                    NrTelefone = table.Column<long>(type: "bigint", nullable: true),
                    NmEndereco = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: false),
                    IdCargos = table.Column<int>(type: "integer", nullable: false),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    NmEmail = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    NmSenha = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdFuncionario", x => x.IdFuncionario);
                });

            migrationBuilder.CreateTable(
                name: "Perfis",
                columns: table => new
                {
                    IdPerfil = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NmNome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdPerfil", x => x.IdPerfil);
                });

            migrationBuilder.CreateTable(
                name: "Permuta",
                columns: table => new
                {
                    IdPermuta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEscala = table.Column<int>(type: "integer", nullable: false),
                    NmNomeSolicitante = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    NmNomeSolicitado = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    NrMatriculaSolicitante = table.Column<int>(type: "integer", nullable: false),
                    NrMatriculaSolicitado = table.Column<int>(type: "integer", nullable: false),
                    DtSolicitacao = table.Column<DateOnly>(type: "date", nullable: true),
                    DtDataSolicitadaTroca = table.Column<DateOnly>(type: "date", nullable: true),
                    NmNomeAprovador = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    NrMatriculaAprovador = table.Column<int>(type: "integer", nullable: false),
                    DtAprovacao = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdPermuta", x => x.IdPermuta);
                });

            migrationBuilder.CreateTable(
                name: "PostoTrabalho",
                columns: table => new
                {
                    IdPostoTrabalho = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NmNome = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: false),
                    NmEnderco = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: true),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    DtCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdDepartamento = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdPostoTrabalho", x => x.IdPostoTrabalho);
                });

            migrationBuilder.CreateTable(
                name: "TipoEscala",
                columns: table => new
                {
                    IdTipoEscala = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NmNome = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: false),
                    NmDescricao = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: true),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    DtCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NrHorasTrabalhada = table.Column<int>(type: "integer", nullable: false),
                    NrHorasFolga = table.Column<int>(type: "integer", nullable: false),
                    IsExpediente = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdTipoEscala", x => x.IdTipoEscala);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdFuncionario = table.Column<int>(type: "integer", nullable: false),
                    FuncionarioIdFuncionario = table.Column<int>(type: "integer", nullable: false),
                    IdFuncionalidade = table.Column<int>(type: "integer", nullable: false),
                    FuncionalidadeIdFuncionalidade = table.Column<int>(type: "integer", nullable: false),
                    Inputs = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TipoErro = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DataHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogID);
                    table.ForeignKey(
                        name: "FK_Logs_Funcionalidades_FuncionalidadeIdFuncionalidade",
                        column: x => x.FuncionalidadeIdFuncionalidade,
                        principalTable: "Funcionalidades",
                        principalColumn: "IdFuncionalidade",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Logs_Funcionario_FuncionarioIdFuncionario",
                        column: x => x.FuncionarioIdFuncionario,
                        principalTable: "Funcionario",
                        principalColumn: "IdFuncionario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FuncionariosPerfis",
                columns: table => new
                {
                    IdFuncionario = table.Column<int>(type: "integer", nullable: false),
                    IdPerfil = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuncionariosPerfis", x => new { x.IdFuncionario, x.IdPerfil });
                    table.ForeignKey(
                        name: "FK_FuncionariosPerfis_Funcionario_IdFuncionario",
                        column: x => x.IdFuncionario,
                        principalTable: "Funcionario",
                        principalColumn: "IdFuncionario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FuncionariosPerfis_Perfis_IdPerfil",
                        column: x => x.IdPerfil,
                        principalTable: "Perfis",
                        principalColumn: "IdPerfil",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerfisFuncionalidades",
                columns: table => new
                {
                    IdPerfil = table.Column<int>(type: "integer", nullable: false),
                    IdFuncionalidade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfisFuncionalidades", x => new { x.IdPerfil, x.IdFuncionalidade });
                    table.ForeignKey(
                        name: "FK_PerfisFuncionalidades_Funcionalidades_IdFuncionalidade",
                        column: x => x.IdFuncionalidade,
                        principalTable: "Funcionalidades",
                        principalColumn: "IdFuncionalidade",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfisFuncionalidades_Perfis_IdPerfil",
                        column: x => x.IdPerfil,
                        principalTable: "Perfis",
                        principalColumn: "IdPerfil",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FuncionariosPerfis_IdPerfil",
                table: "FuncionariosPerfis",
                column: "IdPerfil");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_FuncionalidadeIdFuncionalidade",
                table: "Logs",
                column: "FuncionalidadeIdFuncionalidade");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_FuncionarioIdFuncionario",
                table: "Logs",
                column: "FuncionarioIdFuncionario");

            migrationBuilder.CreateIndex(
                name: "IX_PerfisFuncionalidades_IdFuncionalidade",
                table: "PerfisFuncionalidades",
                column: "IdFuncionalidade");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cargos");

            migrationBuilder.DropTable(
                name: "Departamento");

            migrationBuilder.DropTable(
                name: "EMPRESA");

            migrationBuilder.DropTable(
                name: "Escala");

            migrationBuilder.DropTable(
                name: "EscalaPronta");

            migrationBuilder.DropTable(
                name: "FuncionariosPerfis");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "PerfisFuncionalidades");

            migrationBuilder.DropTable(
                name: "Permuta");

            migrationBuilder.DropTable(
                name: "PostoTrabalho");

            migrationBuilder.DropTable(
                name: "TipoEscala");

            migrationBuilder.DropTable(
                name: "Funcionario");

            migrationBuilder.DropTable(
                name: "Funcionalidades");

            migrationBuilder.DropTable(
                name: "Perfis");
        }
    }
}
