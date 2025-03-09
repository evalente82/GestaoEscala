using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Dominio.Services.Departamento;
using GestaoEscalaPermutas.Dominio.Interfaces.Departamento;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using GestaoEscalaPermutas.Dominio.Services.Cargos;
using GestaoEscalaPermutas.Dominio.Interfaces.Cargos;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Dominio.Services.Funcionario;
using GestaoEscalaPermutas.Dominio.Services.Escala;
using GestaoEscalaPermutas.Dominio.Services.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.Interfaces.Escala;
using GestaoEscalaPermutas.Dominio.Interfaces.TipoEscala;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaPronta;
using GestaoEscalaPermutas.Dominio.Services.EscalaPronta;
using GestaoEscalaPermutas.Dominio.Interfaces.Permutas;
using GestaoEscalaPermutas.Dominio.Services.Permutas;
using GestaoEscalaPermutas.Dominio.Interfaces.Login;
using GestaoEscalaPermutas.Dominio.Services.Login;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GestaoEscalaPermutas.Dominio.Interfaces.Usuario;
using GestaoEscalaPermutas.Dominio.Services.Usuarios;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfilFuncionalidades;
using GestaoEscalaPermutas.Dominio.Services.PerfilFuncionalidades;
using GestaoEscalaPermutas.Dominio.Interfaces.PerfisFuncionalidades;
using GestaoEscalaPermutas.Dominio.Services.PerfisFuncionalidades;
using GestaoEscalaPermutas.Dominio.Services.CargoPerfis;
using GestaoEscalaPermutas.Dominio.Interfaces.Email;
using GestaoEscalaPermutas.Dominio.Services.Setor;
using GestaoEscalaPermutas.Dominio.Interfaces.Setor;
using GestaoEscalaPermutas.Repository.DependencyInjection;
using GestaoEscalaPermutas.Dominio.Services.Funcionario.GestaoEscalaPermutas.Dominio.Services.Funcionario;
using GestaoEscalaPermutas.Dominio.Services.TipoEscala.GestaoEscalaPermutas.Dominio.Services;
using GestaoEscalaPermutas.Dominio.Services.Funcionalidade;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using GestaoEscalaPermutas.Dominio.Services.Mensageria;
using GestaoEscalaPermutas.Dominio.Interfaces.Mensageria;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using GestaoEscalaPermutas.Dominio.Mapping;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;


var cultureInfo = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

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
        Title = "Gestão Escala Permutas",
        Description = "WebAPI com JWT. \n\n# Introdução\nEsta API está documentada no formato **OpenAPI format** e é baseada na " +
        "\nIntegração Swagger também fornecida pela equipe da [VCorp Sistem]. " +
        "\n\n# Especificação da Integração\nA seguinte imagem ilustra o funcionamento da Aplicação." +
        "\n\n# Cross-Origin Resource Sharing\nEsta API utiliza Cross-Origin Resource Sharing (CORS) implementado em conformidade com as especificações W3C." +
        "\nE isso permite que recursos restritos em uma página da web sejam recuperados por outro domínio fora do domínio ao qual pertence o recurso que será recuperado."
    });

    //  Configura��o para permitir autentica��o via JWT no Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite 'Bearer' + espa�o + seu token JWT."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

// Configurar Firebase Admin SDK
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "firebase-adminsdk.json"))
});

#region Injecao de dependencias
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDbContext<DefesaCivilMaricaContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("EmUso")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddResponseCompression();

builder.Services.AddScoped<IDepartamentoService, DepartamentoService>();
builder.Services.AddScoped<ICargoService, CargoService>();
builder.Services.AddScoped<IFuncionarioService, FuncionarioService>();
builder.Services.AddScoped<IEscalaService, EscalaService>();
builder.Services.AddScoped<IPostoTrabalhoService, PostoTrabalhoService>();
builder.Services.AddScoped<ITipoEscalaService, TipoEscalaService>();
builder.Services.AddScoped<IEscalaProntaService, EscalaProntaService>();
builder.Services.AddScoped<IPermutasService, PermutasService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IFuncionalidadeService, FuncionalidadeService>();
builder.Services.AddScoped<IPerfisFuncionalidadesService, PerfisFuncionalidadesService>();
builder.Services.AddScoped<ICargoPerfisService, CargoPerfisService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISetorService, SetorService>();
builder.Services.AddRepositoryServices();
builder.Services.AddHostedService<PermutasMessageConsumer>();
#endregion


builder.Services.AddSingleton<IMessageBus>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    // Priorizar variáveis de ambiente do Fly.io sobre appsettings
    var hostName = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME") ?? configuration["RabbitMQ:HostName"] ?? "localhost";
    var userName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? configuration["RabbitMQ:UserName"] ?? "guest";
    var password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? configuration["RabbitMQ:Password"] ?? "guest";
    var portStr = Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? configuration["RabbitMQ:Port"] ?? "5672";
    var vhost = Environment.GetEnvironmentVariable("RABBITMQ_VHOST") ?? configuration["RabbitMQ:VirtualHost"] ?? "/";

    // Converter porta para int com tratamento de erro
    if (!int.TryParse(portStr, out int port))
    {
        port = 5672; // Porta padrão do RabbitMQ
        Console.WriteLine($"Porta inválida '{portStr}'. Usando padrão: 5672.");
    }

    Console.WriteLine($"Tentando conectar ao RabbitMQ - Host: {hostName}, User: {userName}, Port: {port}, VHost: {vhost}");

    try
    {
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password,
            Port = port,
            VirtualHost = vhost
        };
        var connection = factory.CreateConnection();
        Console.WriteLine("Conexão com RabbitMQ estabelecida com sucesso!");
        return new RabbitMqMessageBus(connection); // Passe a conexão, não apenas o hostname
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao conectar ao RabbitMQ: {ex.Message}. Continuando sem mensageria.");
        return null; // Fallback para evitar crash
    }
});

builder.Services.AddHostedService<UsuarioMessageConsumer>();


// Definir ambiente de produção
var environment = builder.Environment.EnvironmentName;
var configuracoes = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
            policy.WithOrigins(
            "http://192.168.0.4:8080", // Backend local
            "http://10.0.2.2:8080",   // Emulador Android
            "http://localhost:5173",   // Frontend
            "http://localhost:8080"   // Swagger local
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Configurar autentica��o JWT
var chaveSecreta = "g9h0N7quw2S8mJAF8LKxUF0Os3leG+NDJoypOcWohOEa"; // Mesma chave usada no LoginService

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "gestao-escala-backend",  // Mesmo valor usado no token JWT
        ValidAudience = "gestao-escala-frontend",  // Mesmo valor usado no token JWT
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSecreta))
    };
});

// Configurar autorização global (protegendo todas as rotas por padrão)

//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = new AuthorizationPolicyBuilder()
//        .RequireAuthenticatedUser()
//        .Build();
//});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080); // Isso permite conexões de qualquer IP
});

//gerarChave teste = new();
//teste.teste();
try
{
    var app = builder.Build();
    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.UseDeveloperExceptionPage();

    //if (app.Environment.IsDevelopment())
    //{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    });
    //}

    app.UseMiddleware<PermissaoMiddleware>();
    app.UseRouting();

    app.UseCors("AllowSpecificOrigin");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Erro crítico na inicialização: {ex.Message}");
    throw;
}