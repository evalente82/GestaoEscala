using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Dominio.Services.Departamento;
using GestaoEscalaPermutas.Dominio.Interfaces.Departamento;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using GestaoEscalaPermutas.Dominio.Services.Cargos;
using GestaoEscalaPermutas.Dominio.Interfaces.Cargos;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Dominio.Services.Funcionario;
using GestaoEscalaPermutas.Dominio.Services.Escala;
using GestaoEscalaPermutas.Dominio.Services.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.Services.TipoEscala;
using GestaoEscalaPermutas.Dominio.Interfaces.Escala;
using GestaoEscalaPermutas.Dominio.Interfaces.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.Interfaces.TipoEscala;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaPronta;
using GestaoEscalaPermutas.Dominio.Services.EscalaPronta;

var builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration.GetConnectionString("EmUso");

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();
builder.Services.AddSingleton(configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Gest�o Escala Permutas",
        Description = "WebAPI com JWT. \n\n# Introdu��o\nEsta API est� documentada no formato **OpenAPI format** e � baseada na " +
        "\n[Integra��o Swagger](/swagger/index.html) tamb�m fornecida pela equipe da [VCorp Sistem]. " +
        "\n\n# Especifica��o da Integra��o\nA seguinte imagem ilustra o funcionamento da Aplica��o." +
        "\n\n# Cross-Origin Resource Sharing\nEsta API utiliza Cross-Origin Resource Sharing (CORS) implementado em conformidade com as especifica��es [W3C](https://www.w3.org/TR/cors/)." +
        "\nE isso permite que recursos restritos em uma p�gina da web sejam recuperados por outro dom�nio fora do dom�nio ao qual pertence o recurso que ser� recuperado."
    });

    //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //{
    //    Scheme = "Bearer",
    //    BearerFormat = "JWT",
    //    In = ParameterLocation.Header,
    //    Name = "Authorization",
    //    Description = "Bearer com autentica��o JWT Token",
    //    Type = SecuritySchemeType.Http
    //});
    //options.AddSecurityRequirement(new OpenApiSecurityRequirement {
    //    {
    //        new OpenApiSecurityScheme {
    //            Reference = new OpenApiReference {
    //                Id = "Bearer",
    //                    Type = ReferenceType.SecurityScheme
    //            }
    //        },
    //        new List < string > ()
    //    }
    //});
});

builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//builder.Services.AddDbContext<DefesaCivilMaricaContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString(connString ?? string.Empty)));
builder.Services.AddDbContext<DefesaCivilMaricaContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("EmUso")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddResponseCompression();

builder.Services.AddTransient<IDepartamentoService, DepartamentoService>();
builder.Services.AddTransient<ICargoService, CargoService>();
builder.Services.AddTransient<IFuncionarioService, FuncionarioService>();
builder.Services.AddTransient<IEscalaService, EscalaService>();
builder.Services.AddTransient<IPostoTrabalhoService, PostoTrabalhoService>();
builder.Services.AddTransient<ITipoEscalaService, TipoEscalaService>();
builder.Services.AddTransient<IEscalaProntaService, EscalaProntaService>();

var app = builder.Build();
app.Use(async (context, next) =>
{
    try
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await next();
            return;
        }
        await next();
    }
    catch (Exception)
    {
        await context.Response.WriteAsync(JsonConvert.SerializeObject("teste."));
    }
});

app.UseCors(builder =>
{
    builder.WithOrigins("https://localhost:5173") 
           .WithMethods("GET", "POST", "PUT","PATCH", "DELETE") // Especifique os m�todos HTTP permitidos
           .WithHeaders("Content-Type", "Authorization"); // Especifique os cabe�alhos permitidos
});

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseDeveloperExceptionPage();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    });
}

app.UseHttpsRedirection();
//app.UseAuthorization();
app.MapControllers();
//app.MapFallbackToFile("/index.html");
app.Run();
