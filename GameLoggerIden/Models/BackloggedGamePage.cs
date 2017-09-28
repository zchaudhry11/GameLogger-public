using GameLoggerIden.Util;
using System.Collections.Generic;

namespace GameLoggerIden.Models
{
    public class BackloggedGamePage
    {
        public IEnumerable<BackloggedGame> Games { get; set; }
        public Pager Pager { get; set; }
    }
}