using GameLoggerIden.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GameLoggerIden.Controllers
{
    [Authorize]
    public class SteamController : Controller
    {
        //private BacklogDataContext db = new BacklogDataContext();

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Gets a user's SteamID if they have one linked to their account.
        /// </summary>
        public string GetSteamID()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new GameLoggerIden.Models.ApplicationDbContext()));
            var CurrentUser = manager.FindById(User.Identity.GetUserId());

            if (User.Identity.Name != "")
            {
                string url = "";

                // If user has a steam profile linked
                if (CurrentUser.Logins.Count > 0)
                {
                    for (int i = 0; i < CurrentUser.Logins.Count; i++)
                    {
                        if (CurrentUser.Logins.ToList()[i].LoginProvider == "Steam")
                        {
                            url = CurrentUser.Logins.ToList()[i].ProviderKey;

                            ViewBag.steamid = url.Split('/')[5]; // Get user's Steam ID
                        }
                    }
                }
            }
            else
            {
                ViewBag.steamid = "";
            }

            return ViewBag.steamid;
        }

        /// <summary>
        /// Gets a user's Steam profile information if they have a Steam account linked.
        /// </summary>
        [HttpGet]
        public JsonResult GetProfile()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new GameLoggerIden.Models.ApplicationDbContext()));
            var CurrentUser = manager.FindById(User.Identity.GetUserId());

            // If current user has a Steam account linked
            if (CurrentUser.Logins.Count > 0)
            {
                for (int i = 0; i < CurrentUser.Logins.Count; i++)
                {
                    if (CurrentUser.Logins.ToList()[i].LoginProvider == "Steam")
                    {
                        // Get Steam User information for currently logged in user
                        string url = string.Format("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=BAC162F90A60259B7E7EB20A4EB46B63&steamids={0}", this.GetSteamID());
                        string result = null;

                        using (var client = new WebClient())
                        {
                            result = client.DownloadString(url);
                        }

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            return Json("{\"success\": false}", JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// Gets a user's Steam library information if they have a Steam account linked.
        /// </summary>
        [HttpGet]
        public JsonResult GetGames()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new GameLoggerIden.Models.ApplicationDbContext()));
            var CurrentUser = manager.FindById(User.Identity.GetUserId());

            // If current user has a Steam account linked
            if (CurrentUser.Logins.Count > 0)
            {
                for (int q = 0; q < CurrentUser.Logins.Count; q++)
                {
                    if (CurrentUser.Logins.ToList()[q].LoginProvider == "Steam")
                    {
                        string url = string.Format("http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=BAC162F90A60259B7E7EB20A4EB46B63&steamid={0}&include_appinfo=1&format=json", this.GetSteamID());
                        string result = null;

                        using (var client = new WebClient())
                        {
                            result = client.DownloadString(url);
                        }

                        JObject jsonResults = JObject.Parse(result);
                        JObject steamGames = JObject.Parse(jsonResults.GetValue("response").ToString());

                        JArray games = JArray.Parse(steamGames.GetValue("games").ToString());

                        using (BacklogDataContext db = new BacklogDataContext())
                        {
                            List<Game> dbGames = db.Games.AsNoTracking().ToList();

                            // Loop through all of user's Steam games and add them to backlog
                            for (int i = 0; i < games.Count; i++)
                            {
                                // Get game name
                                string gameName = games[i]["name"].ToString();

                                // Check to see if game exists in database
                                //Game dbGame = db.Games.AsNoTracking().FirstOrDefault(u => u.Name == gameName);
                                for (int x = 0; x < dbGames.Count; x++)
                                {
                                    if (dbGames[x].Name == gameName)
                                    {
                                        Game dbGame = dbGames[x];

                                        // Add game to backlog if it doesn't already exist
                                        if (dbGame != null)
                                        {
                                            Backlog backlogEntry = db.Backlogs.Find(User.Identity.Name, dbGame.Id); // Check to see if current user has this game in their backlog

                                            // If game doesn't exist in backlog, add it
                                            if (backlogEntry == null)
                                            {
                                                // Get game playtime
                                                int playTime = 0;
                                                Int32.TryParse(games[i]["playtime_forever"].ToString(), out playTime);

                                                // Add game to backlog
                                                Backlog newGame = new Backlog();
                                                newGame.GameId = dbGame.Id;
                                                newGame.PlayTime = playTime;
                                                newGame.UserName = User.Identity.Name;

                                                Backlog userGame = db.Backlogs.Add(newGame);
                                            }
                                            else // If game exists in backlog already, update playtime
                                            {
                                                // Get game playtime
                                                int playTime = 0;
                                                Int32.TryParse(games[i]["playtime_forever"].ToString(), out playTime);

                                                backlogEntry.PlayTime = playTime;
                                            }
                                        }
                                    }
                                }
                            }

                            db.SaveChanges();
                        }

                        return Json("{\"success\": true}", JsonRequestBehavior.AllowGet);
                    }
                }
            }

            return Json("{\"success\": false}", JsonRequestBehavior.AllowGet);
        }
    }
}