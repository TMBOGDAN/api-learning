using System.ComponentModel.DataAnnotations;

namespace GameStore.api.Models;

public class Genre
{
    public Genre()
    {
    }

    public Genre(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}
