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

    let _showLoader = () => $(_loadIndicatorSelector).removeClass(_loaderHideClass);

    let _hideLoader = () => $(_loadIndicatorSelector).addClass(_loaderHideClass);

    export const init = (addEntryUrl: string): void => {
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

            _showLoader();

            _xhr(Method.GET, addEntryUrl + '/' + accountID, {}, html => {
                _modalContent.html(html);
                _modal.modal('show');
                // _modal.find('.date-picker').datepicker({ format: 'dd/mm/yyyy' });
                _hideLoader();
            });
        });

        $(document).on('submit', '#add-entry-form', e => {
            e.preventDefault();

            _showLoader();

            let form = $(e.target);
            let accountSummary = $('#account-' + form.find('[name=AccountID]').val());

            _xhr(Method.POST, addEntryUrl, $(e.target).serialize(), response => {
                _hideLoader();

                if ($.trim(response) === 'INVALID') {
                    alert('Form Invalid');
                } else {
                    accountSummary.html(response);
                    _modal.modal('hide');
                }
            });
        });

        $(document).on('focus', '#Amount', e => {
            var input = $(e.target);
            if (parseFloat(input.val() as string) == 0)
                input.val('');
        });

        $(document).on('click', '.btn-date-preset', e => {
            e.preventDefault();

            $('#Date').val($(e.target).data('date'));
        });
    }
}

$(() => {
    money.init(_ADD_ENTRY_URL);
});