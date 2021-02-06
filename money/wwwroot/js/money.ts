declare var ADD_ENTRY_URL: string;
declare var NET_WORTH_URL: string;
declare var DESCRIPTION_DELIMITER_REGEX: RegExp;

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

export const xhr = (method: Method, url: string, data: any, successCallback?: (data: any) => void, errorCallback?: XHRErrorCallback) => {
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

function setTransactionTypeInfo(select: HTMLSelectElement) {
    const selectedOption = select.selectedOptions.item(0);
    modal.find('.entry-modal-type-info').html(selectedOption.getAttribute('data-info'));
}

modal.on('change', 'select[name=Type]', e => {
    const select = e.target as HTMLSelectElement;
    setTransactionTypeInfo(select);
});

if (window.matchMedia("(min-width: 900px)").matches) {
    // Only do this at desktop widths, because mobile keyboards are a massive pain...
    // And yes, this is *very* basic... but until there's actually a good, reliable
    // way to detect what device is being used, this will do.

    modal.on('shown.bs.modal', () => {
        modal.find('input[name=Amount]').focus();
    });
}

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

    const title = accountName + (categoryName ? ': ' + categoryName : '');

    modalTitle.html(title);

    showLoader();

    xhr(Method.GET, ADD_ENTRY_URL, data, html => {
        modalContent.html(html);

        const form = modalContent.find('form');

        $.validator.unobtrusive.parse(form);

        const typeSelect = form.find('select[name=Type]');

        if (typeSelect.length) {
            typeSelect.find('option').get().forEach(el => {
                const info = DESCRIPTION_DELIMITER_REGEX.exec(el.innerText);

                if (info) {
                    el.innerText = el.innerText.replace(DESCRIPTION_DELIMITER_REGEX, '');
                    el.setAttribute('data-info', info[1]);
                }
            });

            setTransactionTypeInfo(typeSelect.get(0) as HTMLSelectElement);
        }

        modal.modal('show');

        hideLoader();
    });
});

$(document).on('click', '.btn-date-preset', e => {
    e.preventDefault();
    $('#Date').val($(e.currentTarget).data('date'));
});

$(document).on('click', '.btn-amount-preset', e => {
    e.preventDefault();

    const button = $(e.currentTarget);
    const amountInput = modal.find('#Amount');

    const amount = parseFloat(button.data('amount'));

    if (button.hasClass('btn-amount-preset-all')) {
        amountInput.val(amount.toFixed(2));
    } else {
        const currentAmount = parseFloat((amountInput.val() as string) || '0');
        const newValue = (currentAmount + amount).toFixed(2);

        amountInput.val(newValue);
    }
});
