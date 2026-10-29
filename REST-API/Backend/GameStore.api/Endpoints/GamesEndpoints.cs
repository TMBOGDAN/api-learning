using GameStore.api.Data;
using GameStore.api.Dtos;
using GameStore.api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.api.Endpoints;

public static class GamesEndpoints
{
    private const string GetGameEndpointName = "GetGame";

    public static void MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games");

        group.MapGet("/", async (GameStoreContext db) =>
            await db.Games.AsNoTracking().Select(game => new GamesDto(
                game.Id, game.Name, game.Genre, game.Price, game.ReleaseDate, game.GenreID)).ToListAsync());

        group.MapGet("/{id:int}", async (int id, GameStoreContext db) =>
            await db.Games.AsNoTracking().SingleOrDefaultAsync(game => game.Id == id) is Games game
                ? Results.Ok(ToDto(game))
                : Results.NotFound())
            .WithName(GetGameEndpointName);

        group.MapPost("/", async (GamesDto input, GameStoreContext db) =>
        {
            var genre = await db.Genres.FindAsync(input.GenreID);
            if (genre is null)
                return Results.BadRequest("GenreID does not identify an existing genre.");

            var game = new Games
            {
                Name = input.Name,
                Genre = genre.Name,
                GenreID = genre.Id,
                Price = input.Price,
                ReleaseDate = input.ReleaseDate
            };
            db.Games.Add(game);
            await db.SaveChangesAsync();
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, ToDto(game));
        });

        group.MapPut("/{id:int}", async (int id, GamesDto input, GameStoreContext db) =>
        {
            var game = await db.Games.FindAsync(id);
            if (game is null)
                return Results.NotFound();
            var genre = await db.Genres.FindAsync(input.GenreID);
            if (genre is null)
                return Results.BadRequest("GenreID does not identify an existing genre.");

            game.Name = input.Name;
            game.Genre = genre.Name;
            game.GenreID = genre.Id;
            game.Price = input.Price;
            game.ReleaseDate = input.ReleaseDate;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("/{id:int}", async (int id, GameStoreContext db) =>
        {
            var game = await db.Games.FindAsync(id);
            if (game is null)
                return Results.NotFound();
            db.Games.Remove(game);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }

    private static GamesDto ToDto(Games game) =>
        new(game.Id, game.Name, game.Genre, game.Price, game.ReleaseDate, game.GenreID);
}
