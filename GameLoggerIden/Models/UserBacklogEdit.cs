using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GameLoggerIden.Models
{
    public class UserBacklogEdit
    {
        [Key]
        [Column("UserName", Order = 0)]
        public string UserName { get; set; }
        [Key]
        [Column("GAME_ID", Order = 1)]
        public int GameId { get; set; }

        public string GameName { get; set; }

        public IEnumerable<SelectListItem> GamesList { get; set; }


        public bool Completed_Game { get; set; }


        public int? PlayTime { get; set; }

        public UserBacklogEdit()
        {
            PlayTime = 0;
        }
    }
}