using System.Text;
using API.Data;
using API.Extensions;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// a continuación la extensión de servicios creada \API\Extensions\ApplicationServiceExtensions.cs
builder.Services.AddApplicationServices(builder.Configuration);
// a continuación la extensión de servicios creada \API\Extensions\IdentityServiceExtensions.cs
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
// JorgeEsp -> siguiente línea es para indicar que se confía el acceso a localhost:4200 desde localhost:5001
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

app.UseAuthentication(); // esto preguntará si hay un Token válido
app.UseAuthorization();  // esto pregunta si el user está autorizado

app.MapControllers();

app.Run();
