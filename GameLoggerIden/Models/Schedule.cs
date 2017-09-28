using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameLoggerIden.Models
{
    public class Schedule
    {
        // User Id
        [Key]
        [Column("UserName", Order = 0)]
        public string UserName { get; set; }

        // The free days available to a user
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public Schedule()
        {
            Monday = true;
            Tuesday = true;
            Wednesday = true;
            Thursday = true;
            Friday = true;
            Saturday = true;
            Sunday = true;
        }
    }
}