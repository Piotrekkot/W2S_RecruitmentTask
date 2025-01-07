using Hangfire;
using Hangfire.SQLite;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Data;
using System.Reflection;
using System.Text;
using Way2SendApi.Infrastructure.DB;
using Way2SendApi.Infrastructure.Repository;
using Way2SendApi.Infrastructure.Repository.Interfaces;
using Way2SendApi.Infrastructure.Services;
using Way2SendApi.Infrastructure.Services.Interfaces;
using Way2SendApi.Infrastructure.Settings;
using Way2SendAPI.ErrorHandler;

var builder = WebApplication.CreateBuilder(args);

//U¿ycie seriloga wraz z szybkimi ustawieniami ¿eby nie zaœmiecaæ logów
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Http", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .MinimumLevel.Override("Hangfire", LogEventLevel.Warning)
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

//po³¹czenia do bazy danych
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqliteConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

//w wiêkszych projektach mo¿na to wynosiæ do innych folderów/klas np. RepositoryRegister,HandlerRegister,ServiceRegister
//Rejestracja repozytoriów
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
//rejestracja services
builder.Services.AddScoped<IRemindService,RemindService>();
//transient bo emailService nie przechowuje stanu i mo¿e byæ tworzony wielokrotnie bez konsekwencji
builder.Services.AddTransient<IEmailService, EmailService>();


//Rejestracja Hangfire z SQLite
builder.Services.AddSingleton(new BackgroundJobServerOptions
{
    WorkerCount = 1,
    ServerName = "TaskHangfireServer"
});

builder.Services.AddHangfire(configuration =>
    configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSQLiteStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

//JWT
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//dokumentacja swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Way2Send API",
        Description = "API do zarz¹dzania zadaniami, wykonane na potrzeby zadania rekrutacyjnego W2S"
    });
    //dla testowania JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "WprowadŸ token JWT (Bearer [token])",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            Array.Empty<string>()
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
//konfiguracja do wysy³ki mail
builder.Services.Configure<SMTP>(builder.Configuration.GetSection("Smtp"));

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHangfireDashboard();

//ustawienie joba na uruchamianie co 5 min cron
RecurringJob.AddOrUpdate<RemindService>(
    "RemindJob",
    service => service.CheckForDueTasks(),
    "*/5 * * * *");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();