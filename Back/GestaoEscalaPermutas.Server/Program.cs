
#region using
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Dominio.Services.Departamento;
using GestaoEscalaPermutas.Dominio.Interfaces.Departamento;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using GestaoEscalaPermutas.Dominio.Services.Cargos;
using GestaoEscalaPermutas.Dominio.Interfaces.Cargos;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
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
using System.Globalization;
using GestaoEscalaPermutas.Dominio.Services.Mensageria;
using GestaoEscalaPermutas.Dominio.Interfaces.Mensageria;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using GestaoEscalaPermutas.Dominio.Mapping;
using RabbitMQ.Client;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra;
using GestaoEscalaPermutas.Dominio.Services.EscalaExtra;
using GestaoEscalaPermutas.Repository.Implementations;
using GestaoEscalaPermutas.Repository.Interfaces;
using GestaoEscalaPermutas.Server.Settings;
using Microsoft.Extensions.Options;
using GestaoEscalaPermutas.Dominio.Services.Recaptcha.SeuNamespace.Services;
using GestaoEscalaPermutas.Dominio.Interfaces.LOGs;
using GestaoEscalaPermutas.Dominio.Services.LOGs;
using Microsoft.AspNetCore.Authorization;
using System.Threading.RateLimiting;
using GestaoEscalaPermutas.Dominio.Interfaces.Feriados;
using GestaoEscalaPermutas.Dominio.Services.Feriados;
#endregion

// --- Configuração de Cultura (Globalização) ---
var cultureInfo = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var builder = WebApplication.CreateBuilder(args);

// =================================================================
// SEÇÃO DE REGISTRO DE SERVIÇOS (Injeção de Dependência)
// =================================================================

// --- Serviços Essenciais do ASP.NET Core ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpClient();

// --- Configuração do Swagger ---
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

// --- Configuração de CORS (Cross-Origin Resource Sharing) ---
// 1. Leia a seção "AllowedCorsOrigins" do seu appsettings.json
//    O .NET automaticamente pega o de Development ou Production.
var allowedOrigins = builder.Configuration.GetSection("AllowedCorsOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        // Verifica se a lista não é nula ou vazia antes de usar
        if (allowedOrigins != null && allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins) // Usa as origens do appsettings
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
        // Opcional: Adicionar uma política mais restritiva se nenhuma origem for configurada
        // else { /* ... */ }
    });
});

// --- Configuração de Segurança (Rate Limiting, JWT, Autorização) ---
builder.Services.AddRateLimiter(options =>
{
    // Política global para todas as requisições
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Request.Headers.Host.ToString(), // Pode usar IP ou outro identificador
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100, // Ex: 100 requisições
                Window = TimeSpan.FromMinutes(1) // a cada 1 minuto
            });
    });

    // Política específica e mais restrita para o endpoint de login
    options.AddPolicy("LoginPolicy", httpContext =>
    {
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(), // Limita por IP
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5, // Apenas 5 tentativas de login
                Window = TimeSpan.FromMinutes(1) // por minuto
            });
    });

    options.RejectionStatusCode = 429; // Too Many Requests
});

// Configurar autenticação JWT
var jwtSecretKey = builder.Configuration["JwtSettings:Secret"];
if (string.IsNullOrEmpty(jwtSecretKey))
{
    throw new InvalidOperationException("A chave secreta do JWT (JwtSettings:Secret) não foi configurada. " +
        "Defina-a no appsettings.Development.json para desenvolvimento ou como um segredo no ambiente de produção.");
}

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
    };
});

// Configurar autorização global (protegendo todas as rotas por padrão)
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// --- Infraestrutura (Banco de Dados, Firebase, RabbitMQ, etc.) ---
builder.Services.AddDbContext<DefesaCivilMaricaContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("EmUso")));

// Configurar Firebase Admin SDK
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "firebase-adminsdk.json"))
});

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

// --- Serviços da Aplicação (Injeção de Dependência dos seus serviços e repositórios) ---
#region Injecao de dependencias
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
builder.Services.AddScoped<IEscalaExtraService, CriacaoEscalaExtraService>();
builder.Services.AddScoped<IEscalaExtraRepository, EscalaExtraRepository>();
builder.Services.AddScoped<ISolicitacaoEscalaExtraService, SolicitacaoEscalaExtraService>();
builder.Services.AddScoped<ISolicitacaoEscalaExtraRepository, SolicitacaoEscalaExtraRepository>();
builder.Services.AddScoped<IEscalaExtraCargoRepository, EscalaExtraCargoRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddRepositoryServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFeriadoRepository, FeriadoRepository>();
builder.Services.AddScoped<IFeriadoService, FeriadoService>();
#endregion

// --- Serviços em Background (Hosted Services) ---
builder.Services.AddHostedService<PermutasMessageConsumer>();
builder.Services.AddHostedService<UsuarioMessageConsumer>();

// --- Configurações Específicas (reCAPTCHA, Kestrel) ---
// Configurar RecaptchaSettings
builder.Services.AddOptions<RecaptchaSettings>()
    .Bind(builder.Configuration.GetSection("RecaptchaSettings"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<RecaptchaSettings>>().Value);
builder.Services.AddTransient<RecaptchaService>();

builder.Services.Configure<RecaptchaSettings>(builder.Configuration.GetSection("Recaptcha"));

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});



//gerarChave teste = new();
//teste.teste();

// =================================================================
// SEÇÃO DE CONFIGURAÇÃO DO PIPELINE HTTP
// =================================================================
try
{
    var app = builder.Build();

    app.UseRateLimiter();// Aplica o limitador de requisições
    app.UseDefaultFiles();
    app.UseStaticFiles();

    // Em ambiente de desenvolvimento, mostrar exceções detalhadas e Swagger
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        });
    }

    app.UseRouting();
    app.UseCors("AllowSpecificOrigin");

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseMiddleware<PermissaoMiddleware>();

    app.MapControllers();


    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Erro crítico na inicialização: {ex.Message}");
    throw;
}