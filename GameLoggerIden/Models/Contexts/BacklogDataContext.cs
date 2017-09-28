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
    public class BacklogDataContext : DbContext
    {
        public BacklogDataContext() : base("DefaultConnection"){}

        public DbSet<User> Users { get; set; }
        public DbSet<Backlog> Backlogs { get; set; }
        public DbSet<Game> Games { get; set; }

        public System.Data.Entity.DbSet<GameLoggerIden.Models.Schedule> Schedules { get; set; }
    }
}