using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GameLoggerIden.Models
{
    public class User
    {
        public int Id { get; set; }
        public int SteamId { get; set; }
        public string Email { get; set; }
        public string PSNUsername { get; set; }
    }
}