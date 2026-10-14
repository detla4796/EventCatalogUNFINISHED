$(function () {
    var url = $('#manageTable').data('toggle-url');
    var token = $('#antiForgeryForm input[name="__RequestVerificationToken"]').val();
    var $msg = $('#manageMsg');

    $('.toggle-publish').on('click', function () {
        var $btn = $(this);
        $msg.text('');

        $.post(url, { id: $btn.data('id'), __RequestVerificationToken: token })
            .done(function (r) {
                if (r.ok) {
                    $btn.closest('tr').find('.status')
                        .text(r.isPublished ? 'Опубликовано' : 'Черновик');
                } else {
                    $msg.text(r.message);
                }
            })
            .fail(function () {
                $msg.text('Ошибка: не удалось изменить статус.');
            });
    });
});