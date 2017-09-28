using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameLoggerIden.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Box_Art { get; set; }
        public string Details_Url { get; set; }
        public double Main_Story_Length { get; set; }
        public double Main_Extra_Length { get; set; }
        public double Completionist_Length { get; set; }
        public double Combined_Length { get; set; }
        public int Steam_Appid { get; set; }
        public int IGDB_Id { get; set; }
        public double? IGDB_Review_Score { get; set; }
    }
}