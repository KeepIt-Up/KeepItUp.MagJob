using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Ocetlot configuration
builder.Configuration.AddJsonFile("gatewayConfiguration.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.Audience = builder.Configuration["Authentication:Audience"];
    options.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"]!;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Authentication:ValidIssuer"],
    };
});
// Cors configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("localhost",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();

app.MapControllers();

app.UseCors("localhost");

await app.UseOcelot();

app.Run();
