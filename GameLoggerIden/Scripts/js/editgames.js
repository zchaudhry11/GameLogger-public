var lastSearchLetter = "";
var currSearchLetter = "";

$(document).ready(function () {
    // Initialize list of games and update autocomplete box
    var init = new Array(1);

    $("#gameName").autocomplete({
        source: init,
        autoFocus: true,
        minLength: 1,
        delay: 500,
        search: function (event, ui) {
            currSearchLetter = $("#gameName").val()[0];

            // If the input search term changed first letters then get new games from database
            if (currSearchLetter != lastSearchLetter) {
                getGameByLetter(currSearchLetter);
            }
        }
    });

    $("#editGameButton").click(function (event) {
        event.preventDefault();
    });
});

function getGameByLetter(gameLetter) {
    $.ajax({
        url: "/Backlog/GetGamesList",
        type: "GET",
        data: {
            letter: gameLetter
        },
        dataType: "JSON",
        success: function (res) {
            // Update the last letter that was searched
            lastSearchLetter = gameLetter;

            // Parse games list
            var updatedGames = [];
            var gamesJson = JSON.parse(res);

            for (var i = 0; i < gamesJson.length; i++) {
                updatedGames.push(gamesJson[i])
            }
            $("#gameName").autocomplete({
                source: updatedGames
            });
        },
        error: function (res) {
            console.log(res);
        }
    });
}

function addGame() {
    var game = $("#gameName").val();
    var completed = $("#completed").prop('checked');
    var playTime = $("#playTime").val();

    $.ajax({
        url: "/Backlog/Add",
        type: "POST",
        data: {
            gameName: game,
            completed: completed,
            playTime: playTime
        },
        dataType: "JSON",
        beforeSend: function () {
            $("#results").html("");
        },
        success: function (res) {
            var jsonRes = JSON.parse(res);

            if (jsonRes.success) {
                $("#results").html("Successfully added game to backlog.");
                $("#results").css("color", "green");
            } else {
                $("#results").html("Invalid game. Could not add to backlog.");
                $("#results").css("color", "red");
            }
        },
        error: function (res) {
            $("#results").html("Could not update backlog.");
            $("#results").css("color", "red");
        }
    });
}