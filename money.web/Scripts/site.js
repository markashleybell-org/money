/// <reference types="jquery" />
/// <reference types="bootstrap-datepicker" />
var Method;
(function (Method) {
    Method[Method["GET"] = 0] = "GET";
    Method[Method["POST"] = 1] = "POST";
})(Method || (Method = {}));
var money;
(function (money) {
    var loadIndicatorSelector = '.load-indicator > img';
    var loaderHideClass = 'load-indicator-hidden';
    var modal;
    var modalTitle;
    var modalContent;
    var netWorth;
    var defaultAjaxErrorCallback = function (request, status, error) {
        return alert('XHR ERROR: ' + error + ', STATUS: ' + status);
    };
    var xhr = function (method, url, data, successCallback, errorCallback) {
        var options = {
            type: Method[method],
            url: url,
            data: data,
            cache: false
        };
        $.ajax(options).done(successCallback).fail(errorCallback || defaultAjaxErrorCallback);
    };
    var showLoader = function () { return $(loadIndicatorSelector).removeClass(loaderHideClass); };
    var hideLoader = function () { return $(loadIndicatorSelector).addClass(loaderHideClass); };
    money.init = function (addEntryUrl, netWorthUrl) {
        $.ajaxSetup({
            cache: false
        });
        addEntryUrl = addEntryUrl;
        netWorthUrl = netWorthUrl;
        modal = $('#add-entry-modal');
        modalTitle = modal.find('.modal-title');
        modalContent = modal.find('.modal-body');
        netWorth = $('#net-worth');
        $(document).on('click', '.btn-add-entry', function (e) {
            e.preventDefault();
            var button = $(e.currentTarget);
            var accountID = parseInt(button.data('accountid'), 10);
            var accountName = button.data('accountname');
            // This bit of code differentiates between a click on the account-level add button
            // (which doesn't have a category ID data attribute) and the category-level add buttons
            // We only want to show the category dropdown if the account-level button has been
            // clicked, so there is a bit of nastiness here which works around the fact that
            // uncategorised categories are returned with an ID of 0 by the stored procedure
            var categoryID = button.attr('data-categoryid') ? parseInt(button.data('categoryid'), 10) : null;
            var categoryName = button.data('categoryname');
            var isCredit = button.attr('data-iscredit') ? button.data('iscredit') : null;
            var data = {
                accountID: accountID,
                categoryID: categoryID !== 0 ? categoryID : null,
                isCredit: isCredit,
                showCategorySelector: false,
                remaining: button.attr('data-remaining') ? parseFloat(button.attr('data-remaining')) : 0
            };
            modalTitle.html(accountName + (categoryName ? ': ' + categoryName : ''));
            showLoader();
            xhr(Method.GET, addEntryUrl, data, function (html) {
                modalContent.html(html);
                modal.modal('show');
                hideLoader();
            });
        });
        $(document).on('submit', '#add-entry-form', function (e) {
            e.preventDefault();
            showLoader();
            var form = $(e.currentTarget);
            xhr(Method.POST, addEntryUrl, form.serialize(), function (response) {
                hideLoader();
                if (!response.ok) {
                    alert('Form Invalid');
                }
                else {
                    response.updated.forEach(function (u) { return $('#account-' + u.id).html(u.html); });
                    netWorth.load(netWorthUrl);
                    modal.modal('hide');
                    setTimeout(function () { return $('.progress-bar').removeClass('updated'); }, 1000);
                }
            });
        });
        $(document).on('focus', '#Amount', function (e) {
            var input = $(e.currentTarget);
            if (parseFloat(input.val()) === 0) {
                input.val('');
            }
        });
        $(document).on('click', '.btn-date-preset', function (e) {
            e.preventDefault();
            $('#Date').val($(e.currentTarget).data('date'));
        });
        $(document).on('click', '.btn-amount-preset', function (e) {
            e.preventDefault();
            $('#Amount').val($(e.currentTarget).data('amount'));
        });
    };
})(money || (money = {}));
$(function () {
    money.init(ADD_ENTRY_URL, NET_WORTH_URL);
});
//# sourceMappingURL=site.js.map