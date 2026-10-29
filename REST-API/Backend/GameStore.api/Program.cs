using GameStore.api.Data;
using GameStore.api.Endpoints;
using GameStore.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

builder.AddGameStoreDb();

var app = builder.Build();

app.MapGamesEndpoints();
app.MapGenresEndpoints();

app.MigrateDb();

app.Run();


//https://youtu.be/YbRe4iIVYJk?t=7547 2h 32



