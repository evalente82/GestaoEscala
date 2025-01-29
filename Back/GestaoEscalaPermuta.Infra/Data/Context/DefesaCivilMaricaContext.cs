using System;
using System.Collections.Generic;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace GestaoEscalaPermutas.Infra.Data.Context;

public partial class DefesaCivilMaricaContext : DbContext
{
    public DefesaCivilMaricaContext()
    {
    }
    public DefesaCivilMaricaContext(DbContextOptions<DefesaCivilMaricaContext> options) : base(options)
    {

    }

    public virtual DbSet<Cargo> Cargos { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Escala> Escalas { get; set; }

    public virtual DbSet<EscalaPronta> EscalaPronta { get; set; }

    public virtual DbSet<Funcionario> Funcionarios { get; set; }

    public virtual DbSet<Permuta> Permuta { get; set; }

    public virtual DbSet<PostoTrabalho> PostoTrabalhos { get; set; }

    public virtual DbSet<TipoEscala> TipoEscalas { get; set; }

    public DbSet<Funcionalidade> Funcionalidades { get; set; }
    public DbSet<Login> Login { get; set; }
    public DbSet<Usuarios> Usuario { get; set; }
    public DbSet<Perfil> Perfil { get; set; }
    public DbSet<PerfisFuncionalidades> PerfisFuncionalidades { get; set; }
    public DbSet<CargoPerfis> FuncionariosPerfis { get; set; }




    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=localhost;Database=DefesaCivilMarica;User Id=sa;Password=R2d2c3po;TrustServerCertificate=True;");



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        var hostEnvironment = Host.CreateDefaultBuilder().Build().Services.GetRequiredService<IHostEnvironment>();

        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(hostEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connStringEmUso = configuration.GetConnectionString("EmUso");

        optionsBuilder.UseNpgsql(connStringEmUso);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region CARGO
        modelBuilder.Entity<Cargo>(entity =>
        {
            entity.HasKey(e => e.IdCargo).HasName("PK_IdCargos");
        });

        modelBuilder.Entity<Cargo>()
        .Property(e => e.IdCargo)
        .HasColumnType("uuid")
        .HasDefaultValueSql("uuid_generate_v4()");

        modelBuilder.Entity<Cargo>()
            .Property(e => e.DtCriacao)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        #endregion

        #region DEPARTAMENTO
        modelBuilder.Entity<Departamento>()
        .Property(e => e.IdDepartamento)
        .HasColumnType("uuid")
        .HasDefaultValueSql("uuid_generate_v4()");

        modelBuilder.Entity<Departamento>()
            .Property(e => e.DtCriacao)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.IdDepartamento).HasName("PK_IdDepartamento");
        });
        #endregion

        #region EMPRESA
        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.IdEmpresa).HasName("PK_IdEmpresa");
        });

        modelBuilder.Entity<Empresa>()
        .Property(e => e.IdEmpresa)
        .HasColumnType("uuid")
        .HasDefaultValueSql("uuid_generate_v4()");
        #endregion

        #region ESCALA
        modelBuilder.Entity<Escala>(entity =>
        {
            entity.HasKey(e => e.IdEscala).HasName("PK_IdEscala");
        });

        modelBuilder.Entity<Escala>()
        .Property(e => e.IdEscala)
        .HasColumnType("uuid")
        .HasDefaultValueSql("uuid_generate_v4()");

        modelBuilder.Entity<Escala>()
            .Property(e => e.DtCriacao)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        #endregion

        #region EscalaPronta
        modelBuilder.Entity<EscalaPronta>(entity =>
        {
            entity.HasKey(e => e.IdEscalaPronta);

            entity.Property(e => e.DtDataServico)
                  .HasColumnType("date");

            entity.Property(e => e.DtCriacao)
                  .HasColumnType("timestamptz");
        });
        #endregion

        #region PERMUTA
        modelBuilder.Entity<Permuta>(entity =>
        {
            entity.HasKey(e => e.IdPermuta).HasName("PK_IdPermuta");
        });

        modelBuilder.Entity<Permuta>()
        .Property(e => e.IdPermuta)
        .HasColumnType("uuid")
        .HasDefaultValueSql("uuid_generate_v4()");
        #endregion

        #region POSTO_TRABALHO
        modelBuilder.Entity<PostoTrabalho>(entity =>
        {
            entity.HasKey(e => e.IdPostoTrabalho).HasName("PK_IdPostoTrabalho");
        });

        modelBuilder.Entity<PostoTrabalho>()
        .Property(e => e.IdPostoTrabalho)
        .HasColumnType("uuid")
        .HasDefaultValueSql("uuid_generate_v4()");

        modelBuilder.Entity<PostoTrabalho>()
            .Property(e => e.DtCriacao)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        #endregion

        #region FUNCIONARIO
        modelBuilder.Entity<Funcionario>(entity =>
        {
            entity.HasKey(e => e.IdFuncionario).HasName("PK_IdFuncionario");
        });

        modelBuilder.Entity<Funcionario>()
        .Property(e => e.IdFuncionario)
        .HasColumnType("uuid")
        .HasDefaultValueSql("uuid_generate_v4()");

        modelBuilder.Entity<Departamento>()
            .Property(e => e.DtCriacao)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        #endregion

        #region FUNCIONARIOPERFIL
        modelBuilder.Entity<CargoPerfis>()
            .HasKey(fp => new { fp.IdCargo, fp.IdPerfil });
        #endregion

        #region TIPO_ESCALA
        modelBuilder.Entity<TipoEscala>(entity =>
        {
            entity.HasKey(e => e.IdTipoEscala).HasName("PK_IdTipoEscala");
        });

        modelBuilder.Entity<TipoEscala>()
        .Property(e => e.IdTipoEscala)
        .HasColumnType("uuid")
        .HasDefaultValueSql("uuid_generate_v4()");

        modelBuilder.Entity<TipoEscala>()
            .Property(e => e.DtCriacao)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        #endregion

        #region PERFIL_FUNCIONALIDADE
        modelBuilder.Entity<PerfisFuncionalidades>()
            .HasKey(pf => new { pf.IdPerfil, pf.IdFuncionalidade });
        #endregion

        #region FUNCIONALIDADE
        modelBuilder.Entity<Funcionalidade>().ToTable("Funcionalidades");
        modelBuilder.Entity<Funcionalidade>()
            .HasKey(f => f.IdFuncionalidade);

        modelBuilder.Entity<Funcionalidade>()
            .Property(f => f.Nome)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Funcionalidade>()
            .Property(f => f.Descricao)
            .HasMaxLength(255);
        #endregion

        #region PERFIL
        modelBuilder.Entity<Perfil>(entity =>
        {
            entity.ToTable("Perfis"); // Nome da tabela no banco de dados
            entity.HasKey(p => p.IdPerfil);
            entity.Property(p => p.Nome).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Descricao).HasMaxLength(255);
        });
        #endregion

        #region LOGIN
        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Usuario).IsRequired();
            entity.Property(e => e.SenhaHash).IsRequired();
        });
        #endregion

        #region USUARIO
        modelBuilder.Entity<Usuarios>(entity =>
        {
            modelBuilder.Entity<Usuarios>()
        .ToTable("usuarios"); // Inclua as aspas duplas para corresponder ao nome sensível a case
        });
        #endregion

        #region PERFIL_FUNCIONALIDADE
        modelBuilder.Entity<PerfisFuncionalidades>()
            .HasKey(pf => new { pf.IdPerfil, pf.IdFuncionalidade });

        modelBuilder.Entity<PerfisFuncionalidades>()
            .HasOne(pf => pf.Perfil)
            .WithMany(p => p.PerfisFuncionalidades)
            .HasForeignKey(pf => pf.IdPerfil)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PerfisFuncionalidades>()
            .HasOne(pf => pf.Funcionalidade)
            .WithMany(f => f.PerfisFuncionalidades)
            .HasForeignKey(pf => pf.IdFuncionalidade)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion

        #region CARGO_PERFIL
        modelBuilder.Entity<CargoPerfis>()
            .HasKey(fp => new { fp.IdCargo, fp.IdPerfil });

        modelBuilder.Entity<CargoPerfis>()
            .HasOne(cp => cp.Cargo)
            .WithMany(c => c.Perfis)
            .HasForeignKey(cp => cp.IdCargo)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CargoPerfis>()
            .HasOne(cp => cp.Perfil)
            .WithMany()
            .HasForeignKey(cp => cp.IdPerfil)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
