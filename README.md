# GameLogger

![](https://i.imgur.com/tJO0M5T.jpg)

GameLogger is a game management platform that allows users to track statistics about the games they are playing and get notifications related to their games as well. Game length data is based on scraped data from HowLongToBeat.com. The project is written in C# with ASP.NET MVC.

A user's game library can be imported from their Steam accounts since the Steam API is used but users can also manually insert games outside of their Steam library if desired. The IGDB API is used to provide further information about every game in a user's library so that a user can determine what they want to play. There is a game scheduler that is integrated with Google Calendar and users can also opt-in to text message notifications provided by the Twilio API.

[gamelogger.us](http://gamelogger.us)
