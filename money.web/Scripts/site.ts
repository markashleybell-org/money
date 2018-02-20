/// <reference types="jquery" />
/// <reference types="bootstrap-datepicker" />

declare var _ADD_ENTRY_URL: string;
declare var _NET_WORTH_URL: string;

enum Method {
    GET,
    POST
}

namespace money {
    const _loadIndicatorSelector = '.load-indicator > img';
    const _loaderHideClass = 'load-indicator-hidden';

    let _addEntryUrl: string;
    let _netWorthUrl: string;

    let _modal: JQuery;
    let _modalTitle: JQuery;
    let _modalContent: JQuery;
    let _netWorth: JQuery;

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

    export const init = (addEntryUrl: string, netWorthUrl: string): void => {
        $.ajaxSetup({
            cache: false
        });

        _addEntryUrl = addEntryUrl;
        _netWorthUrl = netWorthUrl;

        _modal = $('#add-entry-modal');
        _modalTitle = _modal.find('.modal-title');
        _modalContent = _modal.find('.modal-body');
        _netWorth = $('#net-worth');
        
        $(document).on('click', '.btn-add-entry', e => {
            e.preventDefault();

            let button = $(e.currentTarget);

            let accountID = parseInt(button.data('accountid'), 10);
            let accountName = button.data('accountname');

            // This bit of code differentiates between a click on the account-level add button
            // (which doesn't have a category ID data attribute) and the category-level add buttons
            // We only want to show the category dropdown if the account-level button has been
            // clicked, so there is a bit of nastiness here which works around the fact that
            // uncategorised categories are returned with an ID of 0 by the stored procedure

            let categoryID = button.attr('data-categoryid') ? parseInt(button.data('categoryid'), 10) : null;
            let categoryName = button.data('categoryname');

            let isCredit = button.attr('data-iscredit') ? button.data('iscredit') as boolean : null;

            let data = {
                accountID: accountID,
                categoryID: categoryID !== 0 ? categoryID : null,
                isCredit: isCredit,
                showCategorySelector: false,
                remaining: button.attr('data-remaining') ? parseFloat(button.attr('data-remaining')) : 0
            };

            _modalTitle.html(accountName + (categoryName ? ': ' + categoryName : ''));

            _showLoader();

            _xhr(Method.GET, _addEntryUrl, data, html => {
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
                    _netWorth.load(_netWorthUrl);
                    _modal.modal('hide');
                    setTimeout(() => $('.progress-bar').removeClass('updated'), 1000);
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

        $(document).on('click', '.btn-amount-preset', e => {
            e.preventDefault();
            $('#Amount').val($(e.currentTarget).data('amount'));
        });

        $(document).on('click', '.net-worth-expand', e => {
            e.preventDefault();
            $('.net-worth-balance-list').toggleClass('net-worth-balance-list-hidden');
        });
    }
}

$(() => {
    money.init(_ADD_ENTRY_URL, _NET_WORTH_URL);
});