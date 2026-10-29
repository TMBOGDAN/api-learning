using GameStore.api.Data;
using GameStore.api.Dtos;
using Microsoft.EntityFrameworkCore;
using GameStore.api.Models;

namespace GameStore.api.Endpoints;

public static class Genres
{

    public static void MapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/genres");

        app.MapGet("/genres", async (GameStoreContext db) =>
         await db.Genres
         .AsNoTracking()
         .Select(genre => new GenreDto(genre.Id, genre.Name))
         .ToListAsync());
    }


}
