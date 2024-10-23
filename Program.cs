using Serilog;
using ASP.NETCoreWebAPI_Sample.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Serilog-Konfiguration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()    // Ausgabe in der Konsole
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)  // Ausgabe in einer Datei
    .CreateLogger();

builder.Host.UseSerilog();  // Verwende Serilog als Logging-Framework

// Füge CORS-Konfiguration hinzu
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Konfiguriere Swagger und füge API-Key-Authentifizierung hinzu
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-Api-Key",
        Type = SecuritySchemeType.ApiKey,
        Description = "API Key Authentication"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new string[] {}
        }
    });
});

// Konfiguriere die Datenbank
builder.Services.AddDbContextPool<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Verwende HTTPS-Umleitung
app.UseHttpsRedirection();

// Verwende CORS
app.UseCors("AllowAll");

// Serilog Middleware: Logging für jede Anfrage und Antwort
app.Use(async (context, next) =>
{
    Log.Information($"Anfrage: {context.Request.Method} {context.Request.Path}");
    try
    {
        await next.Invoke();
        Log.Information($"Antwort: {context.Response.StatusCode}");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Fehler während der Anfragebearbeitung");
        throw;
    }
});

// Swagger aktivieren
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
