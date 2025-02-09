using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Organizations.Application;
using Organizations.Application.Common.Interfaces;
using Organizations.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepositories();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserAccessor, UserAccessor>();

builder.Services.AddControllers();

builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithKeycloak(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.Audience = builder.Configuration["Keycloak:Audience"];
    options.MetadataAddress = builder.Configuration["Keycloak:MetadataUrl"] ?? throw new Exception("Keycloak:MetadataUrl is not set");
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Keycloak:ValidateIssuer"] ?? throw new Exception("Keycloak:ValidateIssuer is not set")
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCorsPolicies(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("OrganizationConnection") ??
            throw new Exception("Connection string 'OrganizationConnection' not found."),
        name: "postgresql",
        tags: new[] { "db", "sql", "postgresql" });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors("default");
}

app.ApplyMigrationsAndSeedData(app.Environment);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
