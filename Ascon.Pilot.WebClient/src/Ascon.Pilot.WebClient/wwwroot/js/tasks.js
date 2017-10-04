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
});