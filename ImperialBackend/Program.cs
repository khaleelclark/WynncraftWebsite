
using Microsoft.EntityFrameworkCore;
using ImperialBackend.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
}

builder.Services.AddDbContext<ImperialDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHostedService<ImperialBackend.Services.GuildMemberSyncService>();
//builder.Services.AddSingleton<ImperialBackend.Services.GuildMemberSyncService>();


builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Keep most logs, but silence EF SQL command spam
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

// (Optional) also reduce general EF noise
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
