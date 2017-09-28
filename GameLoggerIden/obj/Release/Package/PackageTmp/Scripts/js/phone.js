$(document).ready(function () {
    getPhone();
});

function updatePhone() {
    var phone = $("#phoneNum").val();
    var texts = $("#ReceiveTexts").prop('checked');

    $.ajax({
        url: '/Manage/UpdatePhone',
        type: "POST",
        data: {
            phoneNum: phone,
            receiveTexts: texts
        },
        dataType: 'JSON',
        beforeSend: function () {
            $("#results").html("");
        },
        success: function (res) {
            console.log(res);

            var jsonRes = JSON.parse(res);

            if (jsonRes.success == true) {
                $("#results").html("Successfully updated phone number!");
                $("#results").css("color", "green");
            }
            else {
                $("#results").html("Invalid phone number.");
                $("#results").css("color", "red");
            }
        },
        error: function (res) {
            console.log(res);

            $("#results").html("Could not update phone number.");
            $("#results").css("color", "red");
        }
    });
}

function getPhone() {
    $.ajax({
        url: '/Manage/GetPhone',
        type: "GET",
        dataType: 'JSON',
        success: function (res) {
            console.log(res);

            var jsonRes = JSON.parse(res);

            if (jsonRes.success) {
                var phone = jsonRes.phone_number;
                var texts = jsonRes.receive_texts;

                $("#phoneNum").val(phone);
                $("#ReceiveTexts").prop('checked', texts);
            }
        },
        error: function (res) {
            console.log(res);

            $("#results").html("Could not update phone number.");
            $("#results").css("color", "red");
        }
    });
}