/// <reference types="jquery" />
/// <reference types="bootstrap-datepicker" />
var Method;
(function (Method) {
    Method[Method["GET"] = 0] = "GET";
    Method[Method["POST"] = 1] = "POST";
})(Method || (Method = {}));
var money;
(function (money) {
    var _loadIndicatorSelector = '.load-indicator > img';
    var _loaderHideClass = 'load-indicator-hidden';
    var _addEntryUrl;
    var _modal;
    var _modalTitle;
    var _modalContent;
    var _defaultAjaxErrorCallback = function (request, status, error) { return console.log('XHR ERROR: ' + error + ', STATUS: ' + status); };
    var _xhr = function (method, url, data, successCallback, errorCallback) {
        var options = {
            type: Method[method],
            url: url,
            data: data,
            cache: false
        };
        $.ajax(options).done(successCallback).fail(errorCallback || _defaultAjaxErrorCallback);
    };
    var _showLoader = function () { return $(_loadIndicatorSelector).removeClass(_loaderHideClass); };
    var _hideLoader = function () { return $(_loadIndicatorSelector).addClass(_loaderHideClass); };
    money.init = function (addEntryUrl) {
        _addEntryUrl = addEntryUrl;
        _modal = $('#add-entry-modal');
        _modalTitle = _modal.find('.modal-title');
        _modalContent = _modal.find('.modal-body');
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
            var data = {
                categoryID: categoryID !== 0 ? categoryID : null,
                showCategorySelector: categoryID === null
            };
            _modalTitle.html(accountName + (categoryName ? ': ' + categoryName : ''));
            _showLoader();
            _xhr(Method.GET, _addEntryUrl + '/' + accountID, data, function (html) {
                _modalContent.html(html);
                _modal.modal('show');
                _hideLoader();
            });
        });
        $(document).on('submit', '#add-entry-form', function (e) {
            e.preventDefault();
            _showLoader();
            var form = $(e.currentTarget);
            _xhr(Method.POST, addEntryUrl, form.serialize(), function (response) {
                _hideLoader();
                if (!response.ok) {
                    alert('Form Invalid');
                }
                else {
                    response.updated.forEach(function (u) { return $('#account-' + u.id).html(u.html); });
                    _modal.modal('hide');
                    setTimeout(function () { return $('.updated').removeClass('updated-show'); }, 1000);
                }
            });
        });
        $(document).on('focus', '#Amount', function (e) {
            var input = $(e.currentTarget);
            if (parseFloat(input.val()) == 0)
                input.val('');
        });
        $(document).on('click', '.btn-date-preset', function (e) {
            e.preventDefault();
            $('#Date').val($(e.currentTarget).data('date'));
        });
    };
})(money || (money = {}));
$(function () {
    money.init(_ADD_ENTRY_URL);
});
//# sourceMappingURL=site.js.map