/// <reference types="jquery" />
/// <reference types="bootstrap-datepicker" />

declare var ADD_ENTRY_URL: string;
declare var NET_WORTH_URL: string;

enum Method {
    GET,
    POST
}

type XHRErrorCallback = (request: JQueryXHR, status: string, error: string) => void;

namespace money {
    const loadIndicatorSelector = '.load-indicator > img';
    const loaderHideClass = 'load-indicator-hidden';

    let modal: JQuery;
    let modalTitle: JQuery;
    let modalContent: JQuery;
    let netWorth: JQuery;

    const defaultAjaxErrorCallback: XHRErrorCallback = (request, status, error) =>
        alert('XHR ERROR: ' + error + ', STATUS: ' + status);

    const xhr = (method: Method, url: string, data: any, successCallback?: (data: any) => void, errorCallback?: XHRErrorCallback) => {
        const options = {
            type: Method[method],
            url: url,
            data: data,
            cache: false
        };

        $.ajax(options).done(successCallback).fail(errorCallback || defaultAjaxErrorCallback);
    };

    const showLoader = () => $(loadIndicatorSelector).removeClass(loaderHideClass);

    const hideLoader = () => $(loadIndicatorSelector).addClass(loaderHideClass);

    export const init = (addEntryUrl: string, netWorthUrl: string): void => {
        $.ajaxSetup({
            cache: false
        });

        addEntryUrl = addEntryUrl;
        netWorthUrl = netWorthUrl;

        modal = $('#add-entry-modal');
        modalTitle = modal.find('.modal-title');
        modalContent = modal.find('.modal-body');
        netWorth = $('#net-worth');

        $(document).on('click', '.btn-add-entry', e => {
            e.preventDefault();

            const button = $(e.currentTarget);

            const accountID = parseInt(button.data('accountid'), 10);
            const accountName = button.data('accountname');

            // This bit of code differentiates between a click on the account-level add button
            // (which doesn't have a category ID data attribute) and the category-level add buttons
            // We only want to show the category dropdown if the account-level button has been
            // clicked, so there is a bit of nastiness here which works around the fact that
            // uncategorised categories are returned with an ID of 0 by the stored procedure

            const categoryID = button.attr('data-categoryid') ? parseInt(button.data('categoryid'), 10) : null;
            const categoryName = button.data('categoryname');

            const isCredit = button.attr('data-iscredit') ? button.data('iscredit') as boolean : null;

            const data = {
                accountID: accountID,
                categoryID: categoryID !== 0 ? categoryID : null,
                isCredit: isCredit,
                showCategorySelector: false,
                remaining: button.attr('data-remaining') ? parseFloat(button.attr('data-remaining')) : 0
            };

            modalTitle.html(accountName + (categoryName ? ': ' + categoryName : ''));

            showLoader();

            xhr(Method.GET, addEntryUrl, data, html => {
                modalContent.html(html);
                modal.modal('show');
                hideLoader();
            });
        });

        $(document).on('submit', '#add-entry-form', e => {
            e.preventDefault();

            showLoader();

            const form = $(e.currentTarget);

            xhr(Method.POST, addEntryUrl, form.serialize(), response => {
                hideLoader();

                if (!response.ok) {
                    alert('Form Invalid');
                } else {
                    response.updated.forEach((u: any) => $('#account-' + u.id).html(u.html));
                    netWorth.load(netWorthUrl);
                    modal.modal('hide');
                    setTimeout(() => $('.progress-bar').removeClass('updated'), 1000);
                }
            });
        });

        $(document).on('focus', '#Amount', e => {
            const input = $(e.currentTarget);
            if (parseFloat(input.val() as string) === 0) {
                input.val('');
            }
        });

        $(document).on('click', '.btn-date-preset', e => {
            e.preventDefault();
            $('#Date').val($(e.currentTarget).data('date'));
        });

        $(document).on('click', '.btn-amount-preset', e => {
            e.preventDefault();
            $('#Amount').val($(e.currentTarget).data('amount'));
        });
    };
}

$(() => {
    money.init(ADD_ENTRY_URL, NET_WORTH_URL);
});
