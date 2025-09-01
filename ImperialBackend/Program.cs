
using Microsoft.EntityFrameworkCore;
using ImperialBackend.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ImperialDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=localhost;Database=ImperialDb;Trusted_Connection=True;TrustServerCertificate=True;"));

builder.Services.AddHostedService<ImperialBackend.Services.GuildMemberSyncService>();
builder.Services.AddSingleton<ImperialBackend.Services.GuildMemberSyncService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
