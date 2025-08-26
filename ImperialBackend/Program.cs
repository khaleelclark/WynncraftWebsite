
using Microsoft.EntityFrameworkCore;
using ImperialBackend.Models;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddOData(opt =>
{
    var odataBuilder = new ODataConventionModelBuilder();
    odataBuilder.EntitySet<Rank>("Ranks");
    odataBuilder.EntitySet<Raid>("Raids");
    odataBuilder.EntitySet<RaidCompleted>("RaidsCompleted");
    odataBuilder.EntitySet<PlayerHistoricalStat>("PlayerHistoricalStats");
    odataBuilder.EntitySet<Event>("Events");
    odataBuilder.EntitySet<GuildMember>("GuildMembers");
    odataBuilder.EntitySet<Game>("Games");
    odataBuilder.EntitySet<GuildMemberGame>("GuildMemberGames");
    odataBuilder.EntitySet<Medal>("Medals");
    odataBuilder.EntitySet<GuildMemberMedal>("GuildMemberMedals");
    opt.AddRouteComponents("odata", odataBuilder.GetEdmModel());
    opt.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100);
});
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
