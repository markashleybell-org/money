/// <reference types="jquery" />

namespace money {
    let _accountList: JQuery;

    export const init = (): void => {
        _accountList = $('.panel-group');

        _accountList.on('show.bs.collapse hide.bs.collapse', e => {
            var icon = $('#' + e.target.id.replace('categories', 'heading')).find('.account-heading-view-toggle > .fa');
            if (e.type == 'show') {
                icon.removeClass('fa-chevron-down').addClass('fa-chevron-up');
            } else {
                icon.removeClass('fa-chevron-up').addClass('fa-chevron-down');
            }
        });
    }
}

$(() => {
    money.init();
});