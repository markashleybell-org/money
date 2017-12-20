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
    var _accountList;
    var _addEntryButtons;
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
        _accountList = $('.panel-group');
        _addEntryButtons = $('.btn-add-entry');
        _modal = $('#add-entry-modal');
        _modalTitle = _modal.find('.modal-title');
        _modalContent = _modal.find('.modal-body');
        _accountList.on('show.bs.collapse hide.bs.collapse', function (e) {
            var icon = $('#' + e.target.id.replace('categories', 'heading')).find('.account-heading-view-toggle > .fa');
            if (e.type == 'show') {
                icon.removeClass('fa-chevron-down').addClass('fa-chevron-up');
            }
            else {
                icon.removeClass('fa-chevron-up').addClass('fa-chevron-down');
            }
        });
        _addEntryButtons.on('click', function (e) {
            e.preventDefault();
            var button = $(e.target);
            var accountID = button.data('accountid');
            var accountName = button.data('accountname');
            _modalTitle.html(accountName);
            _showLoader();
            _xhr(Method.GET, addEntryUrl + '/' + accountID, {}, function (html) {
                _modalContent.html(html);
                _modal.modal('show');
                // _modal.find('.date-picker').datepicker({ format: 'dd/mm/yyyy' });
                _hideLoader();
            });
        });
        $(document).on('submit', '#add-entry-form', function (e) {
            e.preventDefault();
            _showLoader();
            var form = $(e.target);
            var accountSummary = $('#account-' + form.find('[name=AccountID]').val());
            _xhr(Method.POST, addEntryUrl, $(e.target).serialize(), function (response) {
                _hideLoader();
                if ($.trim(response) === 'INVALID') {
                    alert('Form Invalid');
                }
                else {
                    accountSummary.html(response);
                    _modal.modal('hide');
                }
            });
        });
        $(document).on('focus', '#Amount', function (e) {
            var input = $(e.target);
            if (parseFloat(input.val()) == 0)
                input.val('');
        });
        $(document).on('click', '.btn-date-preset', function (e) {
            e.preventDefault();
            $('#Date').val($(e.target).data('date'));
        });
    };
})(money || (money = {}));
$(function () {
    money.init(_ADD_ENTRY_URL);
});
//# sourceMappingURL=site.js.map