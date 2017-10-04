using GameLoggerIden.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameLoggerIden.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Gathers necessary data about user's backlog to populate dashboard.
        /// </summary>
        [HttpGet]
        public JsonResult GetBacklogInfo()
        {
            string user = User.Identity.Name;

            List<BackloggedGame> backloggedGames = new List<BackloggedGame>();
            List<Backlog> backlogs = new List<Backlog>();
            List<Game> games = new List<Game>();
            BackloggedGame currPlaying = new BackloggedGame();

            using (BacklogDataContext db = new BacklogDataContext())
            {
                // Select all games in a user's backlog
                backlogs = db.Backlogs.AsNoTracking().Where(m => m.UserName == user).ToList();

                if (backlogs.Count > 0)
                {
                    games = db.Games.AsNoTracking().ToList();

                    for (int i = 0; i < backlogs.Count; i++)
                    {
                        // Get game information
                        int gameid = backlogs[i].GameId;
                        Game game = games.First(m => m.Id == gameid);

                        // Pass all game and backlog information into new object
                        BackloggedGame backloggedGame = new BackloggedGame();
                        backloggedGame.Id = game.Id;
                        backloggedGame.Name = game.Name;
                        backloggedGame.BoxArt = game.Box_Art;
                        backloggedGame.DetailsUrl = game.Details_Url;
                        backloggedGame.MainStoryLength = game.Main_Story_Length;
                        backloggedGame.MainExtraLength = game.Main_Extra_Length;
                        backloggedGame.CompletionistLength = game.Completionist_Length;
                        backloggedGame.CombinedLength = game.Combined_Length;
                        backloggedGame.ReviewScore = game.IGDB_Review_Score;
                        backloggedGame.CompletedGame = backlogs[i].Completed_Game;
                        backloggedGame.PlatinumStatus = backlogs[i].Platinum_Status;
                        backloggedGame.PlayTime = backlogs[i].PlayTime;

                        backloggedGames.Add(backloggedGame);
                    }

                    if (games.Count == 0)
                    {
                        return Json("{\"success\": false}", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("{\"success\": false}", JsonRequestBehavior.AllowGet);
                }
            }

            // Get game user is currently playing TODO: make this less messy
            using (PlayingDataContext db = new PlayingDataContext())
            {
                PlayedGame playing = db.PlayedGames.FirstOrDefault(m => m.UserName == user);

                if (playing != null)
                {
                    for (int i = 0; i < games.Count; i++)
                    {
                        if (games[i].Id == playing.GameId)
                        {
                            BackloggedGame newGame = new BackloggedGame();
                            newGame.Id = games[i].Id;
                            newGame.Name = games[i].Name;
                            newGame.BoxArt = games[i].Box_Art;
                            newGame.DetailsUrl = games[i].Details_Url;
                            newGame.MainStoryLength = games[i].Main_Story_Length;
                            newGame.MainExtraLength = games[i].Main_Extra_Length;
                            newGame.CompletionistLength = games[i].Completionist_Length;
                            newGame.CombinedLength = games[i].Combined_Length;
                            newGame.ReviewScore = games[i].IGDB_Review_Score;

                            currPlaying = newGame;
                        }
                    }

                    for (int i = 0; i < backlogs.Count; i++)
                    {
                        if (currPlaying.Id == backlogs[i].GameId)
                        {
                            currPlaying.CompletedGame = backlogs[i].Completed_Game;
                            currPlaying.PlatinumStatus = backlogs[i].Platinum_Status;
                            currPlaying.PlayTime = backlogs[i].PlayTime;
                        }
                    }
                }
            }

            // Backlog statistics
            double backlogLength = 0;
            int currGameLength = (int)currPlaying.MainStoryLength;

            if (currGameLength < 0)
            {
                currGameLength = 12;
            }

            // Percentage statistics
            int numCompleted = 0;
            double percentageCompleted = 0;
            double percentageCompletedCurrGame = 0;

            // General game statistics
            int numShortGames = 0;
            int numMedGames = 0;
            int numLongGames = 0;

            for (int i = 0; i < backloggedGames.Count; i++)
            {
                double gameLength = backloggedGames[i].MainStoryLength;

                // If game doesn't have a length listed in the database, assume it will take 12 hours to finish
                if (gameLength < 0)
                {
                    gameLength = 12;
                }

                // Get number of completed games
                if (backloggedGames[i].CompletedGame)
                {
                    numCompleted++;
                }

                if (backloggedGames[i].MainStoryLength <= 8)
                {
                    numShortGames++;
                }
                else if (backloggedGames[i].MainStoryLength >= 40)
                {
                    numLongGames++;
                }
                else
                {
                    numMedGames++;
                }

                backlogLength += gameLength;
            }

            // Calculate percent of games completed and percent of current game completed
            percentageCompleted = Math.Round(((double)numCompleted / backloggedGames.Count) * 100, 1);
			
			if (currGameLength <= 0) 
			{
				currGameLength = 1;
			}

            percentageCompletedCurrGame = Math.Round((double)((currPlaying.PlayTime) / (currGameLength * 60)) * 100, 1);

            // Invalid value check
            if (percentageCompletedCurrGame < 0 || Double.IsNaN(percentageCompletedCurrGame))
            {
                percentageCompletedCurrGame = 0;
            }

            // Get most played game
            List<BackloggedGame> sortedPlaytime = backloggedGames.OrderByDescending(m => m.PlayTime).ToList();

            // Create json response
            string jsonStatistics = "{";
            jsonStatistics += "\"" + "num_backlogged_games\"" + ":" + backloggedGames.Count + ",";
            jsonStatistics += "\"" + "finish_backlog_time\"" + ":" + backlogLength + ",";
            jsonStatistics += "\"" + "num_completed\"" + ":" + numCompleted + ",";
            jsonStatistics += "\"" + "perc_completed\"" + ":" + percentageCompleted + ",";
            jsonStatistics += "\"" + "curr_game_name\"" + ":" + "\"" + currPlaying.Name + "\"" + ",";
            jsonStatistics += "\"" + "curr_game_length\"" + ":" + currGameLength + ",";
            jsonStatistics += "\"" + "curr_game_play_time\"" + ":" + (currPlaying.PlayTime/60) + ",";
            jsonStatistics += "\"" + "perc_completed_curr_game\"" + ":" + percentageCompletedCurrGame + ",";
            jsonStatistics += "\"" + "num_short\"" + ":" + numShortGames + ",";
            jsonStatistics += "\"" + "num_med\"" + ":" + numMedGames + ",";
            jsonStatistics += "\"" + "num_long\"" + ":" + numLongGames + ",";

            jsonStatistics += "\"" + "most_played\"" + ":" + "[";
            jsonStatistics += "{\"" + "name\"" + ":" + "\"" + sortedPlaytime[0].Name + "\","; // first most played game
            jsonStatistics += "\"" + "play_time\"" + ":" + sortedPlaytime[0].PlayTime + "}";

            if (sortedPlaytime.Count > 1)
            {
                if (sortedPlaytime[1] != null)
                {

                    jsonStatistics += ",{\"" + "name\"" + ":" + "\"" + sortedPlaytime[1].Name + "\","; // second most played game
                    jsonStatistics += "\"" + "play_time\"" + ":" + sortedPlaytime[1].PlayTime + "},";
                }
            }

            if (sortedPlaytime.Count > 2)
            {
                if (sortedPlaytime[2] != null)
                {
                    jsonStatistics += "{\"" + "name\"" + ":" + "\"" + sortedPlaytime[2].Name + "\","; // third most played game
                    jsonStatistics += "\"" + "play_time\"" + ":" + sortedPlaytime[2].PlayTime + "}";
                }
            }

            jsonStatistics += "]}";

            return Json(jsonStatistics, JsonRequestBehavior.AllowGet);
        }
    }
}