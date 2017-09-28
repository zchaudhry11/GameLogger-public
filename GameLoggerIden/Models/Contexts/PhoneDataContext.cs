using System.Data.Entity;

namespace GameLoggerIden.Models
{
    public class PhoneDataContext : DbContext
    {
        public PhoneDataContext() : base("DefaultConnection"){}

        public DbSet<Phone> Phones { get; set; }
    }
}