import Modal from 'bootstrap/js/dist/modal';
import { DOM, dom } from 'mab-dom';

declare var ADD_ENTRY_URL: string;
declare var NET_WORTH_URL: string;
declare var DESCRIPTION_DELIMITER_REGEX: RegExp;

const loadIndicatorSelector = '.spinner-border';
const loaderHideClass = 'spinner-border-hidden';

const modal = dom('#add-entry-modal');
const modalTitle = modal.find('.modal-title');
const modalContent = modal.find('.modal-body');

const bsModal = modal.get() ? new Modal(modal.get()) : null;

const netWorth = dom('#net-worth');

const showLoader = () => dom(loadIndicatorSelector).removeClass(loaderHideClass);

const hideLoader = () => dom(loadIndicatorSelector).addClass(loaderHideClass);

dom('.needs-validation').on('submit', e => {
    const form = e.targetElement as HTMLFormElement;

    if (!form.checkValidity()) {
        e.preventDefault()
        e.stopPropagation()
    }

    form.classList.add('was-validated');
});

modal.onchild('.btn-primary', 'click',  e => {
    e.preventDefault();

    showLoader();

    const form = modalContent.find('form').get() as HTMLFormElement;

    if (!form.checkValidity()) {
        return;
    }

    const options = {
        method: 'POST',
        body: new FormData(form)
    };

    fetch(ADD_ENTRY_URL, options)
        .then(async response => {
            const json = await response.json();
            hideLoader();
            if (!json.ok) {
                alert(json.msg);
            } else {
                json.updated.forEach((u: any) => dom('#account-' + u.id).html(u.html));

                fetch(NET_WORTH_URL, { method: 'GET' })
                    .then(async response => {
                        const html = await response.text();

                        netWorth.html(html);

                        bsModal.hide();
                        setTimeout(() => dom('.progress-bar').removeClass('updated'), 1000);

                    }).catch(error => {
                        console.log(error);
                    });
            }
        }).catch(error => {
            console.log(error);
        });
});

function setTransactionTypeInfo(select: HTMLSelectElement) {
    const selectedOption = select.selectedOptions.item(0);
    modal.find('.entry-modal-type-info').html(selectedOption.getAttribute('data-info'));
}

modal.onchild('select[name=Type]', 'change', e => {
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

dom('body').onchild('.btn-add-entry', 'click', e => {
    e.preventDefault();

    const button = dom(e.targetElement);

    const buttonEl = button.get();

    const accountID = parseInt(button.data('accountid'), 10);
    const accountName = button.data('accountname');

    // This bit of code differentiates between a click on the account-level add button
    // (which doesn't have a category ID data attribute) and the category-level add buttons
    // We only want to show the category dropdown if the account-level button has been
    // clicked, so there is a bit of nastiness here which works around the fact that
    // uncategorised categories are returned with an ID of 0 by the stored procedure

    const categoryID = buttonEl.hasAttribute('data-categoryid') ? parseInt(button.data('categoryid'), 10) : null;
    const categoryName = button.data('categoryname');

    const isCredit = buttonEl.hasAttribute('data-iscredit') ? button.data('iscredit') === 'true' : null;

    const data = {
        accountID: accountID,
        categoryID: categoryID !== 0 ? categoryID : null,
        isCredit: isCredit,
        showCategorySelector: false,
        remaining: buttonEl.hasAttribute('data-remaining') ? parseFloat(button.data('data-remaining')) : 0
    };

    const title = accountName + (categoryName ? ': ' + categoryName : '');

    modalTitle.html(title);

    showLoader();

    const options = {
        method: 'GET'
    };

    const queryString = Object.entries(data)
        .filter(e => e[1])
        .map(e => `${e[0]}=${e[1]}`)
        .join('&');

    fetch(ADD_ENTRY_URL + '?' + queryString, options)
        .then(async response => {
            const html = await response.text();

            modalContent.html(html);

            const form = modalContent.find('form');

            const typeSelect = form.find('select[name=Type]');

            typeSelect.each(ts => {
                ts.find('option').each(option => {
                    const el = option.get();

                    const info = DESCRIPTION_DELIMITER_REGEX.exec(el.innerText);

                    if (info) {
                        el.innerText = el.innerText.replace(DESCRIPTION_DELIMITER_REGEX, '');
                        el.setAttribute('data-info', info[1]);
                    }
                });

                setTransactionTypeInfo(typeSelect.get(0) as HTMLSelectElement);
            });

            bsModal.show();
            hideLoader();
        }).catch(error => {
            console.log(error);
        });
});


dom('body').onchild('.btn-date-preset', 'click', e => {
    e.preventDefault();
    dom('#Date').val(dom(e.targetElement).data('date'));
});

dom('body').onchild('.btn-amount-preset', 'click', e => {
    e.preventDefault();

    const button = dom(e.targetElement);
    const amountInput = modal.find('#Amount');

    const amount = parseFloat(button.data('amount'));

    if (button.get().classList.contains('btn-amount-preset-all')) {
        amountInput.val(amount.toFixed(2));
    } else {
        const currentAmount = parseFloat((amountInput.val() as string) || '0');
        const newValue = (currentAmount + amount).toFixed(2);

        amountInput.val(newValue);
    }
});
