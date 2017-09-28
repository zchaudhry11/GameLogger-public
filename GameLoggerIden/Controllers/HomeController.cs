using GameLoggerIden.Models;
using GameLoggerIden.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Twilio.Rest.Api.V2010.Account;

namespace GameLoggerIden.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get user's phone number and send them a text about the game they're playing if they have notifications enabled.
        /// </summary>
        /// <param name="gameName">The game currently being played by the user.</param>
        [HttpPost]
        public MessageResource SendTextMessage(string gameName)
        {
            string user = User.Identity.Name;
            Phone phone = new Phone();
            using (PhoneDataContext db = new PhoneDataContext())
            {
                bool exists = db.Phones.Any(m => m.UserName == user);

                if (exists)
                {
                    phone = db.Phones.First(m => m.UserName == user);
                }
                else
                {
                    return null;
                }
            }

            // Text user if they have notifications enabled
            if (phone.ReceiveTexts)
            {
                RestClient rc = new RestClient();
                string to = "+1" + phone.PhoneNumber;

                string body = "This week you will be playing " + gameName + "!";

                return rc.SendMessage("+12562692445", to, body, null).Result;
            }

            return null;
        }
    }
}