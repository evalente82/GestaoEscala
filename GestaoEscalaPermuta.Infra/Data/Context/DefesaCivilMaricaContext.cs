using System;
using System.Collections.Generic;
using GestaoEscalaPermuta.Infra.Data.EntitiesDefesaCivilMarica;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace GestaoEscalaPermuta.Infra.Data.Context;

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
           

        optionsBuilder.UseSqlServer(connStringEmUso);

    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cargo>(entity =>
        {
            entity.HasKey(e => e.IdCargos).HasName("PK_IdCargos");
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.IdDepartamento).HasName("PK_IdDepartamento");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.IdEmpresa).HasName("PK_IdEmpresa");
        });

        modelBuilder.Entity<Escala>(entity =>
        {
            entity.HasKey(e => e.IdEscala).HasName("PK_IdEscala");

        });

        //modelBuilder.Entity<Departamento>()
        //.Property(e => e.DtCriacao)
        //.HasColumnType("date")
        //.HasConversion(
        //    v => v,
        //    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        //);


        modelBuilder.Entity<Funcionario>(entity =>
        {
            entity.HasKey(e => e.IdFuncionario).HasName("PK_IdFuncionario");

        });

        modelBuilder.Entity<Permuta>(entity =>
        {
            entity.HasKey(e => e.IdPermuta).HasName("PK_IdPermuta");

        });

        modelBuilder.Entity<PostoTrabalho>(entity =>
        {
            entity.HasKey(e => e.IdPostoTrabalho).HasName("PK_IdPostoTrabalho");

        });

        modelBuilder.Entity<TipoEscala>(entity =>
        {
            entity.HasKey(e => e.IdTipoEscala).HasName("PK_IdTipoEscala");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
