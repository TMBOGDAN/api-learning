using GameStore.api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.api.Data;

public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
{
    public DbSet<Games> Games => Set<Games>();
    public DbSet<Genre> Genres => Set<Genre>();
}
