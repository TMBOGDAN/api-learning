using GameStore.api.Data;
using GameStore.api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtensions
{

    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider
            .GetRequiredService<GameStoreContext>();
        dbContext.Database.Migrate();

    }

    public static void AddGameStoreDb(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("GameStore");
        // builder.Services.AddScoped<GameStoreContext>();
        builder.Services.AddSqlite<GameStoreContext>(
            connectionString,
            optionsAction: options => options.UseSeeding((context, _) =>
            {
                if (!context.Set<Genre>().Any())
                {
                    context.Set<Genre>().AddRange(
                    new Genre { Name = "Action" },
                    new Genre { Name = "Adventure" },
                    new Genre { Name = "Drama" },
                    new Genre { Name = "Horror" }
                    );

                    context.SaveChanges();
                }
            })
            );

    }
}