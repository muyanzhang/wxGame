using aspnetapp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<GameContext>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();