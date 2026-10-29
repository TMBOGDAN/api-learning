namespace GameStore.api.Models;

public class Games
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public required string Genre { get; set; }
    public int GenreID { get; set; }
}
