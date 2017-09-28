using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameLoggerIden.Models
{
    public class Backlog
    {
        [Key]
        [Column("UserName", Order = 0)]
        public string UserName { get; set; }
        [Key]
        [Column("GAME_ID", Order = 1)]
        public int GameId { get; set; }

        public bool Completed_Game { get; set; }
        public bool Platinum_Status { get; set; }
        public int? PlayTime { get; set; }
        public DateTime? InsertionTime { get; set; }

        public Backlog()
        {
            InsertionTime = DateTime.Now;
        }
    }
}