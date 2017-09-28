using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Calendar.v3;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameLoggerIden.Util
{
    public class AppFlowMetadata : FlowMetadata
    {
        private static readonly IAuthorizationCodeFlow flow =
            new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "",
                    ClientSecret = ""
                },
                Scopes = new[] { CalendarService.Scope.Calendar },
                DataStore = new FileDataStore("GoogleCalendar.Api.Auth.Store")
            });

        // use openid for auth
        public override string GetUserId(Controller controller)
        {
            var user = controller.Session["user"];
            if (user == null)
            {
                user = Guid.NewGuid();
                controller.Session["user"] = user;
            }
            return user.ToString();
        }

        public override IAuthorizationCodeFlow Flow
        {
            get { return flow; }
        }
    }
}