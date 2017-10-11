$(document).ready(function () {
    
    var filterId = getURLParameter('filterId');
    var taskId = getURLParameter('taskId');
    
    if (filterId == undefined) {
        loadTasks(6);
    }
    else {
        loadTasks(filterId, taskId);
    }

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

function loadTasks(filterId, taskId) {
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
            $(".list-group-item").removeClass("active");
            var el = $("#f" + filterId);
            if (el == undefined)
                return;

            var item = $(el);
            item.addClass("active");
        },
        success: function (data) {
            $("#taskList").html(data);

            if (taskId == undefined)
                return;

            var task = $("#" + taskId);
            if (!task.length) {
                addTasks(taskId);
                return;
            }

            processTaskClick(task);
            scrollToElement("#" + taskId);
        },
        error: function (xhr) {
            alert('error' + xhr);
        },
        complete: function () {
            $("#progress").hide();
        }
    });
}

var ready = true; //Assign the flag here
function addTasks(taskId) {
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
            if (data.indexOf("Нет данных для отображения") !== -1)
                return;
            $("#taskList").append(data);

            if (taskId == undefined)
                return;

            var task = $("#" + taskId);
            if (!task.length) {
                ready = true;
                addTasks(taskId);
                return;
            }

            processTaskClick(task);
            scrollToElement("#" + taskId);
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

function processTaskClick(el) {
    $(".task-node").removeClass("active");
    var task = $(el);
    task.addClass("active");
}

function getURLParameter(sParam) {

    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');

    for (var i = 0; i < sURLVariables.length; i++) {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] == sParam) {
            return sParameterName[1];
        }
    }
}

function scrollToElement(elementId) {
    $('html, body').animate({
        scrollTop: $(elementId).offset().top - 100
    }, 500);
}