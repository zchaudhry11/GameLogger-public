using GameLoggerIden.Models;
using GameLoggerIden.Util;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace GameLoggerIden.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        public ActionResult Index(CancellationToken cancellationToken)
        {
            // Google Authorization
            var result = new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).
                AuthorizeAsync(cancellationToken).Result;

            if (result.Credential != null)
            {
                string user = User.Identity.Name;

                CalendarService service = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = result.Credential,
                    ApplicationName = "GameLogger"
                });

                using (ScheduleDataContext db = new ScheduleDataContext())
                {
                    bool scheduleExists = db.Schedules.Any(m => m.UserName == user);
                    bool playedGameExists = db.PlayedGames.Any(m => m.UserName == user);
                    
                    // If no schedule exists then create one
                    if (!scheduleExists)
                    {
                        Schedule schedule = new Schedule();
                        schedule.UserName = user;

                        db.Schedules.Add(schedule);
                        db.SaveChanges();
                    }

                    // If user is playing a game then add it to the calendar
                    if (playedGameExists)
                    {
                        Schedule schedule = db.Schedules.First(m => m.UserName == user);
                        PlayedGame playedGame = db.PlayedGames.First(m => m.UserName == user);
                        int id = playedGame.GameId;
                        Game game = db.Games.First(m => m.Id == id);

                        // User has a game selected and a schedule set up so update their Google Calendar
                        if (schedule != null && playedGame != null)
                        {
                            AddCalendarEvent(service, schedule, game);
                        }
                    }
                    else // No game is being played so create an empty calendar
                    {
                        // Get gamelogger calendar or make one if it doesn't exist
                        IList<CalendarListEntry> calendars = service.CalendarList.List().Execute().Items;

                        // Delete previous calendar and create a new one
                        for (int i = 0; i < calendars.Count; i++)
                        {
                            if (calendars[i].Summary == "GameLogger")
                            {
                                service.Calendars.Delete(calendars[i].Id).Execute();
                            }
                        }

                        // Create new calendar
                        Calendar calendar = new Calendar();
                        calendar.Summary = "GameLogger";
                        calendar.TimeZone = "America/New_York";

                        var newCal = service.Calendars.Insert(calendar).Execute();
                        string id = newCal.Id;
                    }
                }

                return View();
            }
            else
            {
                return new RedirectResult(result.RedirectUri);
            }
        }

        /// <summary>
        /// Inserts a game into a person's Google Calendar.
        /// </summary>
        private void AddCalendarEvent(CalendarService service, Schedule schedule, Game game)
        {
            int gameId = game.Id;
            string user = User.Identity.Name;

            // Get gamelogger calendar or make one if it doesn't exist
            IList<CalendarListEntry> calendars = service.CalendarList.List().Execute().Items;

            // Delete previous calendar and create a new one
            for (int i = 0; i < calendars.Count; i++)
            {
                if (calendars[i].Summary == "GameLogger")
                {
                    service.Calendars.Delete(calendars[i].Id).Execute();
                }
            }

            // Create new calendar
            Calendar calendar = new Calendar();
            calendar.Summary = "GameLogger";
            calendar.TimeZone = "America/New_York";

            var newCal = service.Calendars.Insert(calendar).Execute();
            string calId = newCal.Id;

            // Insert event
            EventsResource eventRes = new EventsResource(service);

            Event calEvent = new Event();
            
            EventDateTime Start = new EventDateTime();
            EventDateTime End = new EventDateTime();

            double gameLength = game.Main_Story_Length;

            int currPlayTime = 0;

            // If game does not have a length specified in the database then assume it will take 12 hours to beat.
            if (gameLength <= 0)
            {
                gameLength = 12;
            }

            // Get user's current playtime on the game and subtract the game's length from the playtime
            using (BacklogDataContext blDb = new BacklogDataContext())
            {
                Backlog bl = blDb.Backlogs.First(m => m.GameId == gameId && m.UserName == user);

                if (bl != null)
                {
                    currPlayTime = ((int)bl.PlayTime/60);
                }
            }

            gameLength -= (currPlayTime);

            int daysToFinish = (int)Math.Ceiling(gameLength / 8);

            List<int> freeDays = new List<int>();

            // Get free days. TODO: make this less messy
            if (schedule.Monday == true) { freeDays.Add(1); }
            if (schedule.Tuesday == true) { freeDays.Add(2); }
            if (schedule.Wednesday == true) { freeDays.Add(3); }
            if (schedule.Thursday == true) { freeDays.Add(4); }
            if (schedule.Friday == true) { freeDays.Add(5); }
            if (schedule.Saturday == true) { freeDays.Add(6); }
            if (schedule.Sunday == true) { freeDays.Add(7); }

            int weekOffset = 0;

            // Find the next available day to play
            if (freeDays.Count > 0)
            {
                for (int x = 0; x < daysToFinish; x++)
                {
                    // Set start date. TODO: make this less messy
                    for (int i = 0; i < freeDays.Count; i++)
                    {
                        // Set event times
                        if (freeDays[i] == 1)
                        {
                            DateTime today = DateTime.Today;
                            int daysUntilNext = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
                            DateTime next = today.AddDays(daysUntilNext + (weekOffset*7));
                            Start.Date = next.ToString("yyyy-MM-dd");
                            End.Date = Start.Date;
                            daysToFinish--;
                        }
                        else if (freeDays[i] == 2)
                        {
                            DateTime today = DateTime.Today;
                            int daysUntilNext = ((int)DayOfWeek.Tuesday - (int)today.DayOfWeek + 7) % 7;
                            DateTime next = today.AddDays(daysUntilNext + (weekOffset * 7));
                            Start.Date = next.ToString("yyyy-MM-dd");
                            End.Date = Start.Date;
                            daysToFinish--;
                        }
                        else if (freeDays[i] == 3)
                        {
                            DateTime today = DateTime.Today;
                            int daysUntilNext = ((int)DayOfWeek.Wednesday - (int)today.DayOfWeek + 7) % 7;
                            DateTime next = today.AddDays(daysUntilNext + (weekOffset * 7));
                            Start.Date = next.ToString("yyyy-MM-dd");
                            End.Date = Start.Date;
                            daysToFinish--;
                        }
                        else if (freeDays[i] == 4)
                        {
                            DateTime today = DateTime.Today;
                            int daysUntilNext = ((int)DayOfWeek.Thursday - (int)today.DayOfWeek + 7) % 7;
                            DateTime next = today.AddDays(daysUntilNext + (weekOffset * 7));
                            Start.Date = next.ToString("yyyy-MM-dd");
                            End.Date = Start.Date;
                            daysToFinish--;
                        }
                        else if (freeDays[i] == 5)
                        {
                            DateTime today = DateTime.Today;
                            int daysUntilNext = ((int)DayOfWeek.Friday - (int)today.DayOfWeek + 7) % 7;
                            DateTime next = today.AddDays(daysUntilNext + (weekOffset * 7));
                            Start.Date = next.ToString("yyyy-MM-dd");
                            End.Date = Start.Date;
                            daysToFinish--;
                        }
                        else if (freeDays[i] == 6)
                        {
                            DateTime today = DateTime.Today;
                            int daysUntilNext = ((int)DayOfWeek.Saturday - (int)today.DayOfWeek + 7) % 7;
                            DateTime next = today.AddDays(daysUntilNext + (weekOffset * 7));
                            Start.Date = next.ToString("yyyy-MM-dd");
                            End.Date = Start.Date;
                            daysToFinish--;
                        }
                        else if (freeDays[i] == 7)
                        {
                            DateTime today = DateTime.Today;
                            int daysUntilNext = ((int)DayOfWeek.Sunday - (int)today.DayOfWeek + 7) % 7;
                            DateTime next = today.AddDays(daysUntilNext + (weekOffset * 7));
                            Start.Date = next.ToString("yyyy-MM-dd");
                            End.Date = Start.Date;
                            daysToFinish--;
                        }

                        // Set event information and insert
                        calEvent.Summary = "GameLogger";
                        calEvent.Start = Start;
                        calEvent.End = End;
                        calEvent.ColorId = "3";
                        calEvent.Description = "Playing " + game.Name;

                        // Insert event
                        eventRes.Insert(calEvent, calId).Execute();

                        if (daysToFinish <= 0)
                        {
                            break;
                        }
                    }

                    weekOffset++;
                }
            }
        }

        public ActionResult Schedule()
        {
            return View();
        }

        /// <summary>
        /// Retrieves a user's schedule of free days.
        /// </summary>
        [HttpGet]
        public JsonResult GetSchedule()
        {
            string jsonSchedule = "";

            using (ScheduleDataContext db = new ScheduleDataContext())
            {
                string user = User.Identity.Name;

                bool exists = db.Schedules.Any(m => m.UserName == user);

                // User has a schedule so retrieve it
                if (exists)
                {
                    Schedule schedule = db.Schedules.First(m => m.UserName == user);

                    if (schedule != null)
                    {
                        jsonSchedule += "{";
                        jsonSchedule += "\"monday\":" + "\"" + schedule.Monday + "\"" + ",";
                        jsonSchedule += "\"tuesday\":" + "\"" + schedule.Tuesday + "\"" + ",";
                        jsonSchedule += "\"wednesday\":" + "\"" + schedule.Wednesday + "\"" + ",";
                        jsonSchedule += "\"thursday\":" + "\"" + schedule.Thursday + "\"" + ",";
                        jsonSchedule += "\"friday\":" + "\"" + schedule.Friday + "\"" + ",";
                        jsonSchedule += "\"saturday\":" +  "\"" + schedule.Saturday + "\"" + ",";
                        jsonSchedule += "\"sunday\":" + "\"" + schedule.Sunday + "\"" + "}";
                    }
                    else
                    {
                        return Json("failed", JsonRequestBehavior.AllowGet);
                    }
                }
                else // User doesn't have a schedule so make a default one
                {
                    jsonSchedule += "{";
                    jsonSchedule += "\"monday\":" + "\"" + "True" + "\"" + ",";
                    jsonSchedule += "\"tuesday\":" + "\"" + "True" + "\"" + ",";
                    jsonSchedule += "\"wednesday\":" + "\"" + "True" + "\"" + ",";
                    jsonSchedule += "\"thursday\":" + "\"" + "True" + "\"" + ",";
                    jsonSchedule += "\"friday\":" + "\"" + "True" + "\"" + ",";
                    jsonSchedule += "\"saturday\":" + "\"" + "True" + "\"" + ",";
                    jsonSchedule += "\"sunday\":" + "\"" + "True" + "\"" + "}";
                }
            }

            return Json(jsonSchedule, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Updates a user's schedule of free days.
        /// </summary>
        [HttpPost]
        public JsonResult UpdateSchedule(string mon, string tue, string wed, string thu, string fri, string sat, string sun)
        {
            using (ScheduleDataContext db = new ScheduleDataContext())
            {
                string user = User.Identity.Name;

                bool exists = db.Schedules.Any(m => m.UserName == user);

                // If user has a schedule, update it
                if (exists)
                {
                    Schedule schedule = db.Schedules.First(m => m.UserName == user);

                    if (schedule != null)
                    {
                        db.Schedules.Remove(schedule);

                        Schedule updatedSchedule = new Schedule();
                        updatedSchedule.UserName = User.Identity.Name;
                        updatedSchedule.Monday = bool.Parse(mon);
                        updatedSchedule.Tuesday = bool.Parse(tue);
                        updatedSchedule.Wednesday = bool.Parse(wed);
                        updatedSchedule.Thursday = bool.Parse(thu);
                        updatedSchedule.Friday = bool.Parse(fri);
                        updatedSchedule.Saturday = bool.Parse(sat);
                        updatedSchedule.Sunday = bool.Parse(sun);

                        db.Schedules.Add(updatedSchedule);
                        db.SaveChanges();

                        return Json("Finished updating schedule.", JsonRequestBehavior.AllowGet);
                    }
                }
                else // If no schedule is found, create a new one
                {
                    Schedule updatedSchedule = new Schedule();
                    updatedSchedule.UserName = User.Identity.Name;
                    updatedSchedule.Monday = bool.Parse(mon);
                    updatedSchedule.Tuesday = bool.Parse(tue);
                    updatedSchedule.Wednesday = bool.Parse(wed);
                    updatedSchedule.Thursday = bool.Parse(thu);
                    updatedSchedule.Friday = bool.Parse(fri);
                    updatedSchedule.Saturday = bool.Parse(sat);
                    updatedSchedule.Sunday = bool.Parse(sun);

                    db.Schedules.Add(updatedSchedule);
                    db.SaveChanges();

                    return Json("Finished updating schedule.", JsonRequestBehavior.AllowGet);
                }
            }

            return Json("Failed to update schedules.", JsonRequestBehavior.AllowGet);
        }
    }
}