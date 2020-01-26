import Sortable from 'sortablejs';

function persistOrder(persistUrl: string): (sortable: Sortable) => void {
    return function (sortable) {
        var order = sortable.toArray().map(id => parseInt(id, 10));
        $.ajax({
            type: 'POST',
            url: persistUrl,
            data: { itemOrder: order },
            cache: false
        })
        .done(response => console.log(response))
        .fail((request: JQueryXHR, status: string, error: string) => console.log(status));
    }
}

document.querySelectorAll('.sortable-rows').forEach((el: HTMLElement) => {
    Sortable.create(el, {
        draggable: 'tr',
        direction: 'vertical',
        ghostClass: 'drag-ghost',
        store: {
            get: null,
            set: persistOrder(el.getAttribute('data-persist-url'))
        }
    });
});
