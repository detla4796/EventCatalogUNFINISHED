$(function () {
    var $form = $('#filterForm');
    var $list = $('#eventList');

    function load() {
        $.get($list.data('url'), $form.serialize())
            .done(function (html) {
                $list.html(html);
            })
            .fail(function () {
                $list.html('<p class="text-danger">Не удалось загрузить список.</p>');
            });
    }

    $form.on('submit', function (e) {
        e.preventDefault();
        load();
    });
    $form.on('change', 'select', load);
    $form.on('input', 'input', load);
});