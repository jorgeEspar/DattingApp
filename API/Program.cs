using API.Data;
using API.Extensions;
using API.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// a continuación la extensión de servicios creada \API\Extensions\ApplicationServiceExtensions.cs
builder.Services.AddApplicationServices(builder.Configuration);
// a continuación la extensión de servicios creada \API\Extensions\IdentityServiceExtensions.cs
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

// JorgeEsp -> siguiente línea es para indicar que se confía el acceso a localhost:4200 desde localhost:5001
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

app.UseAuthentication(); // esto preguntará si hay un Token válido
app.UseAuthorization();  // esto pregunta si el user está autorizado

app.MapControllers();

// a continuación para cargar la BBDD con los usuarios del fichero json
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(context);
}
catch (Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration datausers into BBDD");
}
// fin creación usuarios en BBDD del fichero json

app.Run();
