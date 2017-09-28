$(document).ready(function () {
    initSchedule();
});

function initSchedule() {
    $.ajax({
        url: "/Calendar/GetSchedule",
        type: "GET",
        dataType: "JSON",
        success: function (res) {
            console.log(res);
            var jsonSchedule = JSON.parse(res);

            var mon = jsonSchedule.monday;
            var tue = jsonSchedule.tuesday;
            var wed = jsonSchedule.wednesday;
            var thu = jsonSchedule.thursday;
            var fri = jsonSchedule.friday;
            var sat = jsonSchedule.saturday;
            var sun = jsonSchedule.sunday;

            // Disable toggles if day is not free
            if (mon != "True") { $("#monday").bootstrapToggle("off"); }
            if (tue != "True") { $("#tuesday").bootstrapToggle("off"); }
            if (wed != "True") { $("#wednesday").bootstrapToggle("off"); }
            if (thu != "True") { $("#thursday").bootstrapToggle("off"); }
            if (fri != "True") { $("#friday").bootstrapToggle("off"); }
            if (sat != "True") { $("#saturday").bootstrapToggle("off"); }
            if (sun != "True") { $("#sunday").bootstrapToggle("off"); }
        },
        error: function (res) {
            console.log(res);
        }
    });

    $("#scheduleForm").submit(function (e) {
        e.preventDefault();
    });
}

function updateSchedule() {
    // Get day values
    var mon = $('#monday').prop('checked');
    var tue = $('#tuesday').prop('checked');
    var wed = $('#wednesday').prop('checked');
    var thu = $('#thursday').prop('checked');
    var fri = $('#friday').prop('checked');
    var sat = $('#saturday').prop('checked');
    var sun = $('#sunday').prop('checked');

    // Update database
    $.ajax({
        url: "/Calendar/UpdateSchedule",
        type: "POST",
        data: {
            mon: mon,
            tue: tue,
            wed: wed,
            thu: thu,
            fri: fri,
            sat: sat,
            sun: sun
        },
        dataType: "JSON",
        beforeSend: function () {
            $("#resultMessage").html("");
        },
        success: function (res) {
            console.log(res);

            // Success message
            $("#resultMessage").html("Schedule updated successfully!");
            $("#resultMessage").css("color", "green");
        },
        error: function (res) {
            console.log(res);

            // Failure message
            $("#resultMessage").html("Schedule could not be updated!");
            $("#resultMessage").css("color", "red");
        }
    });
}