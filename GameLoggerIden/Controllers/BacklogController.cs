using GameLoggerIden.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GameLoggerIden.Util;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace GameLoggerIden.Controllers
{
    [Authorize]
    public class BacklogController : Controller
    {
        /// <summary>
        /// Obtain a list of the games that are in a user's backlog.
        /// </summary>
        /// <param name="page">An integer that represents what page in a user's backlog list to start at.</param>
        public ActionResult Index(int? page)
        {
            string user = User.Identity.Name;

            using (BacklogDataContext db = new BacklogDataContext())
            {
                // Get all of a user's game
                IQueryable<Backlog> query = db.Backlogs.AsNoTracking();
                var totalCount = query.Count();

                query = query
                    .Where(b => b.UserName == user && b.Completed_Game == false);

                var pager = new Pager(query.Count(), page, 30); // Pager used to only display a subset of games at any given moment

                // Order results by most recent
                query = query.OrderByDescending(o => o.InsertionTime).ThenByDescending(o => o.PlayTime);

                // Get the games based on what page of games is currently selected
                query = query.Skip((pager.CurrPage - 1) * pager.NumResults).Take(pager.NumResults);

                var data = query.Select(backlog => new
                {
                    GameID = backlog.GameId,
                    Completed = backlog.Completed_Game,
                    Platinum = backlog.Platinum_Status,
                    PlayTime = backlog.PlayTime
                }).ToList();

                // Create a List of all the games in the backlog with their relevant data
                List<BackloggedGame> backloggedGames = new List<BackloggedGame>();

                for (int i = 0; i < data.Count; i++)
                {
                    int id = data[i].GameID;
                    Game game = db.Games.First(g => g.Id == id); // Get game with specific id

                    // Get all relevant information about a game and add it to list
                    BackloggedGame newGame = new BackloggedGame();
                    newGame.Id = game.Id;
                    newGame.Name = game.Name;
                    newGame.BoxArt = game.Box_Art;
                    newGame.DetailsUrl = game.Details_Url;
                    newGame.MainStoryLength = game.Main_Story_Length;
                    newGame.MainExtraLength = game.Main_Extra_Length;
                    newGame.CompletionistLength = game.Completionist_Length;
                    newGame.CombinedLength = game.Combined_Length;
                    newGame.ReviewScore = game.IGDB_Review_Score;
                    newGame.CompletedGame = data[i].Completed;
                    newGame.PlatinumStatus = data[i].Platinum;
                    newGame.PlayTime = data[i].PlayTime;

                    // If the game isn't in the list already then add it
                    if (!backloggedGames.Contains(newGame))
                    {
                        backloggedGames.Add(newGame);
                    }
                }

                // Pass the final results to the client
                BackloggedGamePage pagedResults = new BackloggedGamePage();
                pagedResults.Games = backloggedGames;
                pagedResults.Pager = pager;

                return View(pagedResults);
            }
        }

        /// <summary>
        /// Displays the list of games that a user has already completed.
        /// </summary>
        /// <param name="page">An integer that represents what page in a user's backlog list to start at.</param>
        public ActionResult Completed(int? page)
        {
            string user = User.Identity.Name;

            using (BacklogDataContext db = new BacklogDataContext())
            {
                // Get all of a user's game
                IQueryable<Backlog> query = db.Backlogs.AsNoTracking();
                var totalCount = query.Count();

                query = query
                    .Where(b => b.UserName == user && b.Completed_Game == true);

                var pager = new Pager(query.Count(), page, 30); // Pager used to only display a subset of games at any given moment

                // Order results by most recent
                query = query.OrderByDescending(o => o.InsertionTime).ThenByDescending(o => o.PlayTime);

                // Get the games based on what page of games is currently selected
                query = query.Skip((pager.CurrPage - 1) * pager.NumResults).Take(pager.NumResults);

                var data = query.Select(backlog => new
                {
                    GameID = backlog.GameId,
                    Completed = backlog.Completed_Game,
                    Platinum = backlog.Platinum_Status,
                    PlayTime = backlog.PlayTime
                }).ToList();

                // Create a List of all the games in the backlog with their relevant data
                List<BackloggedGame> backloggedGames = new List<BackloggedGame>();

                for (int i = 0; i < data.Count; i++)
                {
                    int id = data[i].GameID;
                    Game game = db.Games.First(g => g.Id == id); // Get game with specific id

                    // Get all relevant information about a game and add it to list
                    BackloggedGame newGame = new BackloggedGame();
                    newGame.Id = game.Id;
                    newGame.Name = game.Name;
                    newGame.BoxArt = game.Box_Art;
                    newGame.DetailsUrl = game.Details_Url;
                    newGame.MainStoryLength = game.Main_Story_Length;
                    newGame.MainExtraLength = game.Main_Extra_Length;
                    newGame.CompletionistLength = game.Completionist_Length;
                    newGame.CombinedLength = game.Combined_Length;
                    newGame.ReviewScore = game.IGDB_Review_Score;
                    newGame.CompletedGame = data[i].Completed;
                    newGame.PlatinumStatus = data[i].Platinum;
                    newGame.PlayTime = data[i].PlayTime;

                    // If the game isn't in the list already then add it
                    if (!backloggedGames.Contains(newGame))
                    {
                        backloggedGames.Add(newGame);
                    }
                }

                // Pass the final results to the client
                BackloggedGamePage pagedResults = new BackloggedGamePage();
                pagedResults.Games = backloggedGames;
                pagedResults.Pager = pager;

                return View(pagedResults);
            }
        }

        /// <summary>
        /// Displays a form that allows a user to add a game to their backlog or completed game lists.
        /// </summary>
        public ActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Retrieves information about a selected game from the IGDB API.
        /// </summary>
        /// <param name="gameName">The name of the game to search for.</param>
        [HttpGet]
        public JsonResult GetGameInfo(string gameName)
        {
            // Create GET request
            string fields = "name%2Caggregated_rating%2Crating%2Csummary%2Crelease_dates%2Cscreenshots%2Cvideos&limit=";
            int limit = 50;
            string offset_order = "&offset=0" + "&search=";
            string url = "https://igdbcom-internet-game-database-v1.p.mashape.com/games/?fields=" + fields + limit + offset_order + gameName;

            string result = string.Empty;

            WebRequest request = WebRequest.Create(url);

            request.Method = "GET";
            request.Headers.Add("X-Mashape-Key", "xrNUIa2p8GmshKheCkYR0Q1bgpyLp1JiSKGjsnxihpXvfUlD4v");

            // Get the response 
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Retrieves a list of all games that start with the input letter.
        /// </summary>
        /// <param name="letter">String containing a character to search for.</param>
        [HttpGet]
        public JsonResult GetGamesList(string letter)
        {
            string search = letter.ToLower();
            List<string> gameNames = new List<string>();

            using (BacklogDataContext db = new BacklogDataContext())
            {
                List<Game> games = db.Games.AsNoTracking().Where(m => m.Name.ToLower().StartsWith(search)).ToList();

                for (int i = 0; i < games.Count; i++)
                {
                    gameNames.Add(games[i].Name);
                }

                string gamesList = JsonConvert.SerializeObject(gameNames);


                return Json(gamesList, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Add a game to the user's backlog.
        /// </summary>
        [HttpPost]
        public JsonResult Add(string gameName, bool completed, string playTime)
        {
            if (gameName == "")
            {
                return Json("{\"success\": false}", JsonRequestBehavior.AllowGet);
            }

            using (BacklogDataContext db = new BacklogDataContext())
            {
                if (gameName != null)
                {
                    // Look up ID of input game name
                    Game game = db.Games.First(g => g.Name == gameName);

                    // Check to make sure game doesn't exist in backlog
                    int id = game.Id;

                    // If game doesn't exist in backlog, add it, otherwise update the entry
                    if (!db.Backlogs.Any(b => b.GameId == id))
                    {
                        // Add game to backlog list and return success
                        Backlog backlog = new Backlog();
                        backlog.UserName = User.Identity.Name; // Set username to currently logged in user
                        backlog.GameId = game.Id;
                        backlog.Completed_Game = completed;

                        // Ensure playtime is valid
                        int time = 0;
                        Int32.TryParse((string)playTime.ToString(), out time);

                        if (time < 0)
                        {
                            time = 0;
                        }

                        backlog.PlayTime = (time * 60); // Convert playtime to minutes
                        backlog.Platinum_Status = false;

                        db.Backlogs.Add(backlog);
                        db.SaveChanges();

                        return Json("{\"success\": true}", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        // Remove old entry
                        Backlog oldEntry = db.Backlogs.First(m => m.GameId == id);
                        db.Backlogs.Remove(oldEntry);

                        // Add game to backlog list and return success
                        Backlog backlog = new Backlog();
                        backlog.UserName = User.Identity.Name; // Set username to currently logged in user
                        backlog.GameId = game.Id;
                        backlog.Completed_Game = completed;

                        // Ensure playtime is valid
                        int time = 0;
                        Int32.TryParse((string)playTime.ToString(), out time);

                        if (time < 0)
                        {
                            time = 0;
                        }

                        backlog.PlayTime = (time * 60); // Convert playtime to minutes
                        backlog.Platinum_Status = false;

                        db.Backlogs.Add(backlog);
                        db.SaveChanges();

                        return Json("{\"success\": true}", JsonRequestBehavior.AllowGet);
                    }
                }

                return Json("{\"success\": false}", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Updates the currently played game for the logged in user.
        /// </summary>
        [HttpGet]
        public JsonResult PlayGame(int gameId)
        {
            using (PlayingDataContext db = new PlayingDataContext())
            {
                var playingGame = db.PlayedGames.SingleOrDefault(m => m.UserName == User.Identity.Name);

                if (playingGame != null)
                {
                    PlayedGame game = new PlayedGame();
                    game.UserName = User.Identity.Name;
                    game.GameId = gameId;

                    db.PlayedGames.Remove(playingGame);
                    db.PlayedGames.Add(game);
                    db.SaveChanges();
                }
                else
                {
                    PlayedGame game = new PlayedGame();
                    game.UserName = User.Identity.Name;
                    game.GameId = gameId;

                    db.PlayedGames.Add(game);
                    db.SaveChanges();
                }
            }

            return Json("{\"success\": true}", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Updates the input game to be marked as completed for the current user.
        /// </summary>
        /// <param name="gameId">ID of the game to be marked as completed.</param>
        [HttpPost]
        public JsonResult CompletedGame(int gameId, bool completed)
        {
            string user = User.Identity.Name;

            using (BacklogDataContext db = new BacklogDataContext())
            {
                Backlog backloggedGame = db.Backlogs.FirstOrDefault(m => m.UserName == user && m.GameId == gameId);

                Backlog updatedEntry = new Backlog();
                updatedEntry.UserName = user;
                updatedEntry.GameId = gameId;
                updatedEntry.Completed_Game = completed;
                updatedEntry.Platinum_Status = backloggedGame.Platinum_Status;
                updatedEntry.PlayTime = backloggedGame.PlayTime;
                updatedEntry.InsertionTime = DateTime.Now;

                db.Backlogs.Remove(backloggedGame);
                db.Backlogs.Add(updatedEntry);
                db.SaveChanges();

                return Json("{\"success\": true}", JsonRequestBehavior.AllowGet);
            }
        }
    }
}