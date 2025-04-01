using KeepItUp.MagJob.APIGateway;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Ocetlot configuration
builder.Configuration.AddJsonFile("gatewayConfiguration.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

// Cors configuration
builder.Services.AddCorsConfig(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["JwtSettings:Authority"];
        options.Audience = builder.Configuration["JwtSettings:Audience"];
        options.RequireHttpsMetadata = bool.Parse(builder.Configuration["JwtSettings:RequireHttpsMetadata"] ?? "false");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Authority"]
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors(CorsConfig.CorsPolicyName);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();
