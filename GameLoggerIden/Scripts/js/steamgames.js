function getSteamGames() {
    $.ajax({
        url: '/Steam/GetGames',
        dataType: 'JSON',
        beforeSend: function () {
            $('#loadingIndicator').css("display", "inline");
        },
        success: function (res) {
            // If user has a Steam profile linked
            console.log(res);
            var json = JSON.parse(res);

            if (json.success == false) {
                $("#result").html("You don't have a Steam account linked!");
                $("#result").css("color", "red");
            }
            else {
                $("#result").html("Steam account information updated successfully!");
                $("#result").css("color", "green");
            }
        },
        error: function (res) {
            console.log(res);
            $("#result").html("Could not update Steam account information!");
            $("#result").css("color", "red");
        },
        complete: function () {
            $('#loadingIndicator').css("display", "none");
        }
    });
}