// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $('.complete-lesson-btn').on('click', function (e) {
        e.preventDefault();

        var lessonId = $(this).data('lesson-id');
        var token = $('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            url: '/Lessons/CompleteLesson/' + lessonId,
            type: 'POST',
            headers: {
                "RequestVerificationToken": token
            },
            success: function (response) {
                alert('Lesson completed successfully');
                location.reload();
            },
            error: function (xhr) {
                console.error(xhr.responseText);
                alert('An error occurred while completing the lesson.');
            }
        });
    });
});


