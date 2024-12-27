$(document).on('click', '.addPartialBtn', function () {
    let value = $(this).val();
    $.get('/Template/GetQuestion', { questionType: value })
        .done(function (data) {
            $('#partialContainer').append(data);
        });
});

$(document).on('click', '.delete-btn', function () {
    $(this).closest('.question-item').remove();
})  