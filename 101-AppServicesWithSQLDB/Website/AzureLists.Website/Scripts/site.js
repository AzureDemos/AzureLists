$(document).ready(function () {
    if ($(window).width() < 768) {
        $('#wrapper').addClass('sidebar-small');
    }


    $('.sidebarCollapse').on('click', function () {
        $('#wrapper').toggleClass('sidebar-small');
    });

    //$('.task').on('click', function () {
    //    alert("task");
    //});
    $('.btn-task-important').on('click', function (e) {
        var taskId = $(this).attr("data-task");
        var listId = $(this).attr("data-list");
        e.stopPropagation();
        e.preventDefault();
        var element = this;
        $.ajax({
            url: "/tasks/toggleImportant?listId=" + listId + "&taskId=" + taskId,
            type: 'GET',
            success: function (data) {
                if (data.success) {
                    $(element).toggleClass("btn-danger");
                    $(element).toggleClass("btn-default");
                }
            },
            error: function () {
                alert("Error Updating Task");
            }
        });
       
    });

    $('.btn-task-complete').on('click', function (e) {
        var taskId = $(this).attr("data-task");
        var listId = $(this).attr("data-list");
        e.stopPropagation();
        e.preventDefault();
        var element = this;
        $.ajax({
            url: "/tasks/toggleComplete?listId=" + listId + "&taskId=" + taskId,
            type: 'GET',
            success: function (data) {
                if (data.success) {
                    window.location.reload(false); 
                }
            },
            error: function () {
                alert("Error Updating Task");
            }
        });

    });
    $('.tasks-completed').hide();
    $('.show-completed').on('click', function (e) {
        $(".tasks-completed").slideToggle();
    });

 
    $('.hideTask').on('click', function () {
        $('#wrapper').addClass('edit-task-collapsed');
    });


    $('.edit-list').on('click', function () {
        $('.list-view').addClass('hide');
        $('.list-create').addClass('hide');
        $('.list-edit').removeClass('hide');

    });

    $('.create-list').on('click', function () {
        $('.list-view').addClass('hide');
        $('.list-edit').addClass('hide');
        $('.list-create').removeClass('hide');
    });

    $('.stop-edit-list').on('click', function () {
        $('.list-view').removeClass('hide');
        $('.list-edit').addClass('hide');
        $('.list-create').addClass('hide');
    });

    $("#create-task").click(function () {
        $("#create-task-form").submit();
    })

    resize();
});

function resize() {
    var heights = window.innerHeight;
    var newHeight = heights - 75;

    $(".content-body").css({ height: newHeight + "px" });
}

window.onresize = function () {
    resize();
};




$(function () {
    $("#datepicker").datepicker();
});

$(window).on('resize', function () {
    $('#wrapper').removeClass('sidebar-small');
    if ($(window).width() < 768) {
        $('#wrapper').addClass('sidebar-small');
    }
})