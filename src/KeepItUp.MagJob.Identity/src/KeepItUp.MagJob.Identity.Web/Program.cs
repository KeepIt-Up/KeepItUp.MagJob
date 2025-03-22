using KeepItUp.MagJob.Identity.Web.Configurations;

var builder = WebApplication.CreateBuilder(args);

var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

logger.Information("Starting web host");

builder.AddLoggerConfigs();

var appLogger = new SerilogLoggerFactory(logger)
    .CreateLogger<Program>();

// Dodaj konfigurację opcji
builder.Services.AddOptionConfigs(builder.Configuration, appLogger, builder);

// Dodaj konfigurację usług
builder.Services.AddServiceConfigs(appLogger, builder);

// Dodaj konfigurację Swagger
builder.Services.AddSwaggerConfig(appLogger);

// Dodaj uwierzytelnianie Keycloak
builder.Services.AddKeycloakAuthentication();

var app = builder.Build();

// Skonfiguruj middleware
await app.UseAppMiddlewareAndSeedDatabase();

// Użyj konfiguracji Swagger
app.UseSwaggerConfig(appLogger);

app.Run();

// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building
public partial class Program { }
