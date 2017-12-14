/// <reference types="jquery" />

enum Method {
    GET,
    POST
}

namespace money {
    let _accountList: JQuery;
    let _addEntryButtons: JQuery;
    let _modal: JQuery;
    let _modalTitle: JQuery;
    let _modalContent: JQuery;

    let _defaultAjaxErrorCallback = (request: JQueryXHR, status: string, error: string): void => console.log('XHR ERROR: ' + error + ', STATUS: ' + status);

    let _xhr = (method: Method, url: string, data: any, successCallback?: (data: any) => void, errorCallback?: (xhr: JQueryXHR, status: string, error: string) => void) => {
        let options = {
            type: Method[method],
            url: url,
            data: data,
            cache: false
        };

        $.ajax(options).done(successCallback).fail(errorCallback || _defaultAjaxErrorCallback);
    }

    export const init = (): void => {
        _accountList = $('.panel-group');
        _addEntryButtons = $('.btn-add-entry');
        _modal = $('#add-entry-modal');
        _modalTitle = _modal.find('.modal-title');
        _modalContent = _modal.find('.modal-body');

        _accountList.on('show.bs.collapse hide.bs.collapse', e => {
            var icon = $('#' + e.target.id.replace('categories', 'heading')).find('.account-heading-view-toggle > .fa');
            if (e.type == 'show') {
                icon.removeClass('fa-chevron-down').addClass('fa-chevron-up');
            } else {
                icon.removeClass('fa-chevron-up').addClass('fa-chevron-down');
            }
        });

        _addEntryButtons.on('click', e => {
            e.preventDefault();

            let button = $(e.target);

            let accountID = button.data('accountid');
            let accountName = button.data('accountname');

            _modalTitle.html(accountName);

            _xhr(Method.GET, 'home/addentry/' + accountID, {}, html => {
                _modalContent.html(html);
                _modal.modal('show');
            });
        });
    }
}

$(() => {
    money.init();
});