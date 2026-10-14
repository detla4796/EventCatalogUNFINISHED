$(function () {
    $('#checkSeats').on('click', function () {
        var $box = $('#seatsResult').show().text('Проверяем...');

        $.getJSON($(this).data('url'))
            .done(function (r) {
                if (!r.ok) {
                    $box.text(r.message);
                    return;
                }
                $box.empty()
                    .append('Id: ' + r.id + '<br>Название: ')
                    .append(document.createTextNode(r.title))
                    .append('<br>Свободных мест: ' + r.freeSeats + '<br>Статус: ' + r.status);
            })
            .fail(function () {
                $box.text('Ошибка запроса.');
            });
    });
});