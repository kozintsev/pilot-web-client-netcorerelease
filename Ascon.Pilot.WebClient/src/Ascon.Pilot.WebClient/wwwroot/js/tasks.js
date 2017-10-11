function loadTasks(filterId) {
    $.ajax({
        url: '/Tasks/GetTasks',
        datatype: "json",
        data: { 'filterId': filterId },
        type: "post",
        contenttype: 'application/json; charset=utf-8',
        async: true,
        beforeSend: function () {
            $("#taskList").empty();
            $("#progress").show();
        },
        success: function (data) {
            $("#taskList").html(data);
        },
        error: function (xhr) {
            alert('error' + xhr);
        },
        complete: function () {
            $("#progress").hide();
        }
    });
}

$(document).ready(function () {
    loadTasks(6);

    var win = $(window);
    var doc = $(document);

    // Each time the user scrolls
    win.scroll(function () {
        // Vertical end reached?
        var height = doc.height() - win.height();
        var pos = win.scrollTop();
               
        if (pos >= height - 50 && pos <= height) {
            addTasks();
        }
    });
});

var ready = true; //Assign the flag here
function addTasks() {
    if (!ready)
        return;

    ready = false; //Set the flag here
    $.ajax({
        url: '/Tasks/GetNextTasks',
        datatype: "json",
        type: "post",
        contenttype: 'application/json; charset=utf-8',
        async: true,
        success: function (data) {
            $("#taskList").append(data);
        },
        error: function (xhr) {
            alert('error' + xhr);
        },
        complete: function () {
            $("#progress").hide();
            ready = true; //Reset the flag here
        },
    });
}