/// <reference types="jquery" />
/// <reference types="bootstrap-datepicker" />

declare var _ADD_ENTRY_URL: string;

enum Method {
    GET,
    POST
}

namespace money {
    const _loadIndicatorSelector = '.load-indicator > img';
    const _loaderHideClass = 'load-indicator-hidden';

    let _addEntryUrl: string;
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

    let _showLoader = () => $(_loadIndicatorSelector).removeClass(_loaderHideClass);

    let _hideLoader = () => $(_loadIndicatorSelector).addClass(_loaderHideClass);

    export const init = (addEntryUrl: string): void => {
        _addEntryUrl = addEntryUrl;

        _modal = $('#add-entry-modal');
        _modalTitle = _modal.find('.modal-title');
        _modalContent = _modal.find('.modal-body');
        
        $(document).on('click', '.btn-add-entry', e => {
            e.preventDefault();

            let button = $(e.currentTarget);

            let accountID = button.data('accountid');
            let accountName = button.data('accountname');
            let categoryID = button.data('categoryid');
            let categoryName = button.data('categoryname');

            _modalTitle.html(accountName + (categoryName ? ': ' + categoryName : ''));

            _showLoader();

            _xhr(Method.GET, _addEntryUrl + '/' + accountID, { categoryID: categoryID }, html => {
                _modalContent.html(html);
                _modal.modal('show');
                _hideLoader();
            });
        });

        $(document).on('submit', '#add-entry-form', e => {
            e.preventDefault();

            _showLoader();

            let form = $(e.currentTarget);

            _xhr(Method.POST, addEntryUrl, form.serialize(), response => {
                _hideLoader();

                if (!response.ok) {
                    alert('Form Invalid');
                } else {
                    response.updated.forEach((u: any) => $('#account-' + u.id).html(u.html));
                    _modal.modal('hide');
                }
            });
        });

        $(document).on('focus', '#Amount', e => {
            var input = $(e.currentTarget);
            if (parseFloat(input.val() as string) == 0)
                input.val('');
        });

        $(document).on('click', '.btn-date-preset', e => {
            e.preventDefault();

            $('#Date').val($(e.currentTarget).data('date'));
        });
    }
}

$(() => {
    money.init(_ADD_ENTRY_URL);
});