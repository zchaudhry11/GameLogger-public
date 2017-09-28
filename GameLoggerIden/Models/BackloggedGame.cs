using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameLoggerIden.Models
{
    public class BackloggedGame
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar")]
        public string Name { get; set; }
        [Column(TypeName = "varchar")]
        public string BoxArt { get; set; }
        [Column(TypeName = "varchar")]
        public string DetailsUrl { get; set; }
        public double MainStoryLength { get; set; }
        public double MainExtraLength { get; set; }
        public double CompletionistLength { get; set; }
        public double CombinedLength { get; set; }
        public double? ReviewScore { get; set; }

        public bool CompletedGame { get; set; }
        public bool PlatinumStatus { get; set; }
        public int? PlayTime { get; set; }

        public BackloggedGame()
        {
            Name = "";
            BoxArt = "";
            DetailsUrl = "";
            PlayTime = 0;
            ReviewScore = 0;
        }

    }
}