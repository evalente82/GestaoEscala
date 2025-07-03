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
using System.Threading.RateLimiting;
using GestaoEscalaPermutas.Dominio.Interfaces.Feriados;
using GestaoEscalaPermutas.Dominio.Services.Feriados;

var cultureInfo = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
            policy.WithOrigins(
            //"https://dcmarica.vcorpsistem.com",
            //"https://appdcmarica.vcorpsistem.com",
            //"https://front-gestao-escala.fly.dev",
            //"https://mobile-gestao-escala.fly.dev"
            "http://192.168.0.10:8080", // Backend local.

            "http://172.17.16.1:8080", // Backend local
            "http://10.0.2.2:8080",   // Emulador Android
            "http://localhost:5173",   // Frontend
            "http://localhost:8080",   // Swagger local

            "http://10.0.2.2:7207",   // Emulador Android
            "http://localhost:5173",   // Frontend
            "http://localhost:8080",   // Swagger local
            "http://localhost:3000"    // Flutter Web
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

//segurança -  limitação de taxa nativa do ASP.NET Core
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

// Configurar autenticação JWT
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
builder.Services.AddScoped<IEscalaExtraService, CriacaoEscalaExtraService>();
builder.Services.AddScoped<IEscalaExtraRepository, EscalaExtraRepository>();
builder.Services.AddScoped<ISolicitacaoEscalaExtraService, SolicitacaoEscalaExtraService>();
builder.Services.AddScoped<ISolicitacaoEscalaExtraRepository, SolicitacaoEscalaExtraRepository>();
builder.Services.AddScoped<IEscalaExtraCargoRepository, EscalaExtraCargoRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddRepositoryServices();
builder.Services.AddHostedService<PermutasMessageConsumer>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFeriadoRepository, FeriadoRepository>();
builder.Services.AddScoped<IFeriadoService, FeriadoService>();

// Configurar o HttpClientFactory
builder.Services.AddHttpClient(); // Adiciona IHttpClientFactory ao container

#endregion

// --- Configuração do RabbitMQ Simplificada ---
builder.Services.AddSingleton<IMessageBus>(sp =>
{
    var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");
    var hostName = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME") ?? rabbitMqConfig["HostName"];

    if (string.IsNullOrEmpty(hostName))
    {
        Console.WriteLine("Configuração do RabbitMQ não encontrada. Mensageria desativada.");
        return null;
    }

    try
    {
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = rabbitMqConfig["UserName"],
            Password = rabbitMqConfig["Password"],
            VirtualHost = rabbitMqConfig["VirtualHost"],
            Port = 5672
        };
        var connection = factory.CreateConnection();
        Console.WriteLine($"Conexão com RabbitMQ em '{hostName}' estabelecida com sucesso!");
        return new RabbitMqMessageBus(connection);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao conectar ao RabbitMQ em '{hostName}': {ex.Message}. Mensageria desativada.");
        return null;
    }
});

builder.Services.AddHostedService<UsuarioMessageConsumer>();

// Configurar RecaptchaSettings
builder.Services.AddOptions<RecaptchaSettings>()
    .Bind(builder.Configuration.GetSection("RecaptchaSettings"));
builder.Services.Configure<RecaptchaSettings>(builder.Configuration.GetSection("Recaptcha"));
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

// Usamos .Value aqui para injetar diretamente o objeto RecaptchaSettings
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<RecaptchaSettings>>().Value);
// Registrar o RecaptchaService
builder.Services.AddTransient<RecaptchaService>(); // Use Transient ou Scoped, dependendo do ciclo de vida desejado

// Configurar autorização global (protegendo todas as rotas por padrão)
//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = new AuthorizationPolicyBuilder()
//        .RequireAuthenticatedUser()
//        .Build();
//});

//gerarChave teste = new();
//teste.teste();
try
{
    var app = builder.Build();
    app.UseRateLimiter(); //segurança
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
    Console.WriteLine(app.UseCors("AllowSpecificOrigin"));

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