$(document).ready(function () {
    if ($(window).width() < 768) {
        $('#wrapper').addClass('sidebar-small');
    }


    $('.sidebarCollapse').on('click', function () {
        $('#wrapper').toggleClass('sidebar-small');
    });

    $('.task').on('click', function () {
        $('#wrapper').removeClass('edit-task-collapsed');
    });

    $('.hideTask').on('click', function () {
        $('#wrapper').addClass('edit-task-collapsed');
    });

});

$(window).on('resize', function () {
    $('#wrapper').removeClass('sidebar-small');
    if ($(window).width() < 768) {
        $('#wrapper').addClass('sidebar-small');
    }
})