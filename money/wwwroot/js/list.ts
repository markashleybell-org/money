import Sortable from 'sortablejs';

function persistOrder(persistUrl: string): (sortable: Sortable) => void {
    return function (sortable) {
        var order = sortable.toArray().map(id => parseInt(id, 10));

        const options = {
            method: 'POST',
            body: JSON.stringify(order),
            headers: {
                'Accept': 'application/json'
            }
        };

        fetch(persistUrl, options)
            .then(_ => {
                console.log('OK');
            }).catch(error => {
                console.log(error)
            });
    }
}

document.querySelectorAll('.sortable-rows').forEach((el: HTMLElement) => {
    Sortable.create(el, {
        draggable: 'tr',
        direction: 'vertical',
        ghostClass: 'drag-ghost',
        handle: '.drag-handle',
        store: {
            get: null,
            set: persistOrder(el.getAttribute('data-persist-url'))
        }
    });
});
