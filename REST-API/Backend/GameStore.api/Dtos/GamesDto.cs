using System.ComponentModel.DataAnnotations;

namespace GameStore.api.Dtos;

public record GamesDto(
    int Id,
    [Required, StringLength(50)] string Name,
    [Required] string Genre,
    [Range(1, 100)] decimal Price,
    DateOnly ReleaseDate,
    [Range(1, int.MaxValue)] int GenreID
);
