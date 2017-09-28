using System.Data.Entity;

namespace GameLoggerIden.Models
{
    public class PlayingDataContext : DbContext
    {
        public PlayingDataContext() : base("DefaultConnection"){}

        public DbSet<PlayedGame> PlayedGames { get; set; }
    }
}