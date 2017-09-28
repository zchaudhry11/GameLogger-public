var selectedGameName = "";
var selectedGameId = -1;

// Use IGDB API to get information about the selected game.
function displayGameInfoModal(gameName, gameId) {
    // Set game name and ID
    selectedGameName = gameName;
    selectedGameId = gameId;

    var gameSearch = gameName.replace(/[:,^]/g, "").toLowerCase(); // Cleaned game name to search for

    openNav();

    $.ajax({
        url: "/Backlog/GetGameInfo",
        type: "GET",
        data: {
            gameName: gameName
        },
        dataType: "JSON",
        success: function (res) {
            var json = JSON.parse(res);

            // Loop through json result array
            for (var i = 0; i < json.length; i++) {
                var cleanedName = json[i].name.replace(/[:,^]/g, "").toLowerCase(); // Cleaned IGDB game name to search for

                // If the game was found from the IGDB results, update tile with game information
                if (cleanedName == gameSearch) {

                    // Game Name
                    $("#gameTitle").html(json[i].name);

                    // Game Summary
                    if (json[i].summary != null) {
                        $("#gameDesc").html(json[i].summary);
                    }
                    else {
                        $("#gameDesc").html("No game summary was found.");
                    }

                    // Game Rating, try to get aggregated rating, otherwise try player rating
                    if (json[i].aggregated_rating != null) {
                        var roundedRating = Math.round(json[i].aggregated_rating * 100) / 100;
                        $("#gameRating").html("<strong>Rating:</strong> " + roundedRating + "%");
                    }
                    else {
                        if (json[i].rating != null) {
                            var roundedRating = Math.round(json[i].rating * 100) / 100;
                            $("#gameRating").html("<strong>Rating:</strong> " + roundedRating + "%");
                        }
                        else {
                            $("#gameRating").html("<strong>Rating:</strong> " + "--");
                        }
                    }

                    // Game Release Date
                    if (json[i].release_dates != null) {
                        if (json[i].release_dates[0].y != null) {
                            $("#gameRelease").html("<strong>Release Date:</strong> " + json[i].release_dates[0].y);
                        }
                        else {
                            $("#gameRelease").html("<strong>Release Date:</strong> " + "--");
                        }
                    }
                    else {
                        $("#gameRelease").html("<strong>Release Date:</strong> " + "--");
                    }

                    // Game Screenshots
                    if (json[i].screenshots != null) {
                        $("#screenshots").html("");

                        console.log("NUM: " + json[i].screenshots.length);
                        var numScreenshots = json[i].screenshots.length;

                        for (var x = 0; x < numScreenshots; x++) {
                            var picUrl = "";

                            if (json[i].screenshots[x] != null) {
                                picUrl = json[i].screenshots[x].url;

                                picUrl = picUrl.substring(2, picUrl.length);
                                picUrl = picUrl.replace("t_thumb", "t_screenshot_huge");
                                picUrl = "https://" + picUrl;
                            }
                            var img = "<div class=\"col\" style=\"display: inline-block;\"><img src=\"" + picUrl + "\" height=\"500\" width=\"600\" /></li>";

                            $("#screenshots").append(img);
                        }
                    }
                    else {
                        $("#screenshots").html("No screenshots available.");
                    }

                    // Game Video
                    if (json[i].videos != null) {
                        var vidUrl = "https://www.youtube.com/embed/" + json[i].videos[0].video_id;

                        $("#gameTrailer").html("<iframe width=\"560\" height=\"315\" src=\"" + vidUrl + "\" frameborder=\"0\" allowfullscreen></iframe>");
                    }
                    else {
                        $("#gameTrailer").html("No trailer available.");
                    }

                    break;
                }
                else { // Couldn't find game
                    $("#gameTitle").html(gameName);
                    resetModal();
                }

            }
        },
        error: function (res) {
            console.log(res);
        }
    });
}

// Mark game as 'playing'
function playGame() {
    $.ajax({
        url: '/Backlog/PlayGame',
        data: {
            gameId: selectedGameId
        },
        dataType: 'JSON',
        success: function (res) {
            console.log(res);
            sendText(selectedGameName);
        },
        error: function (res) {
            console.log(res);
        }
    });
}

// Mark game as uncompleted
function completedGame() {
    $.ajax({
        url: '/Backlog/CompletedGame',
        type: "POST",
        data: {
            gameId: selectedGameId,
            completed: false
        },
        dataType: 'JSON',
        success: function (res) {
            console.log(res);
            // Reload page
            location.reload();
        },
        error: function (res) {
            console.log(res);
        }
    });
}

// Reset model content
function resetModal() {
    $("#gameDesc").html("No game summary was found.");
    $("#gameRating").html("<strong>Rating:</strong> " + "--");
    $("#gameRelease").html("<strong>Release Date:</strong> " + "--");
    $("#screenshots").html("No screenshots available.");
    $("#gameTrailer").html("No trailer available.");
}

// Control modal state
function openNav() {
    document.getElementById("gameModal").style.height = "100%";
}
function closeNav() {
    document.getElementById("gameModal").style.height = "0%";
}