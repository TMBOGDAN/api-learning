using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGameGenreId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GenreID",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                INSERT INTO Genres (Name)
                SELECT DISTINCT Genre FROM Games
                WHERE Genre NOT IN (SELECT Name FROM Genres);
                UPDATE Games SET GenreID = (
                    SELECT Id FROM Genres WHERE Genres.Name = Games.Genre LIMIT 1
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GenreID",
                table: "Games");
        }
    }
}

