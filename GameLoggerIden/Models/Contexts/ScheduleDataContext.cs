using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Diagnostics;
using Microsoft.AspNet.Identity;
using System.Security.Principal;

namespace GameLoggerIden.Models
{
    public class ScheduleDataContext : DbContext
    {
        public ScheduleDataContext() : base("DefaultConnection"){}

        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<PlayedGame> PlayedGames { get; set; }
        public DbSet<Game> Games { get; set; }
    }
}