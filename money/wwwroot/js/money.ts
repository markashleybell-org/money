declare var ADD_ENTRY_URL: string;
declare var NET_WORTH_URL: string;

enum Method {
    GET,
    POST
}

type XHRErrorCallback = (request: JQueryXHR, status: string, error: string) => void;

const loadIndicatorSelector = '.spinner-border';
const loaderHideClass = 'spinner-border-hidden';

const modal = $('#add-entry-modal');
const modalTitle = modal.find('.modal-title');
const modalContent = modal.find('.modal-body');
const netWorth = $('#net-worth');

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

$.ajaxSetup({
    cache: false
});

modal.on('click', '.btn-primary', e => {
    e.preventDefault();

    showLoader();

    const form = modalContent.find('form');

    xhr(Method.POST, ADD_ENTRY_URL, form.serialize(), response => {
        hideLoader();

        if (!form.valid()) {
            return;
        }

        if (!response.ok) {
            alert(response.msg);
        } else {
            response.updated.forEach((u: any) => $('#account-' + u.id).html(u.html));
            netWorth.load(NET_WORTH_URL);
            modal.modal('hide');
            setTimeout(() => $('.progress-bar').removeClass('updated'), 1000);
        }
    });
});

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

    xhr(Method.GET, ADD_ENTRY_URL, data, html => {
        modalContent.html(html);
        $.validator.unobtrusive.parse(modalContent.find('form'));
        modal.modal('show');
        hideLoader();
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
