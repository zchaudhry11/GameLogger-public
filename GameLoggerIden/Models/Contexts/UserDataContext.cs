using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace GameLoggerIden.Models
{
    public class UserDataContext : DbContext
    {
        public UserDataContext() : base("DefaultConnection"){}

        public DbSet<User> Users { get; set; }
    }
}