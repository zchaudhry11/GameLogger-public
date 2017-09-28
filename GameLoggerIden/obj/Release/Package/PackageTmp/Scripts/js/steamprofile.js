$(document).ready(function () {
    getProfilePic();
});

function getProfilePic() {
    $.ajax({
        url: '/Steam/GetProfile',
        dataType: 'JSON',
        success: function (res) {
            // If user has a Steam profile linked
            if (res.length > 0) {
                console.log(res);
                var jsonProfile = JSON.parse(res);

                if (jsonProfile.response != null) {
                    if (jsonProfile.response.players.length > 0) {
                        $.each(jsonProfile.response.players, function (key, value) {
                            if ($('.profile')) {
                                $('.profile').append("<img src='" + value.avatar + "'> <span>" + value.personaname + "</span>")
                            }
                            if ($('.profile1')) {
                                $('.profile1').append("<img src='" + value.avatarfull + "'>")
                            }
                            if ($('.username')) {
                                $('.username').append(value.personaname)
                            }

                            console.log(value)
                        });
                    }
                    else {
                        console.log("Link your steam profile!");
                    }
                }
            }
        },
        error: function (httpReq, status, exception) {
            console.log(status + " " + exception);
        }
    });
}