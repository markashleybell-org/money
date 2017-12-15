/// <reference types="jquery" />
/// <reference types="bootstrap-datepicker" />
var Method;
(function (Method) {
    Method[Method["GET"] = 0] = "GET";
    Method[Method["POST"] = 1] = "POST";
})(Method || (Method = {}));
var money;
(function (money) {
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
    money.init = function () {
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
            _xhr(Method.GET, 'home/addentry/' + accountID, {}, function (html) {
                _modalContent.html(html);
                _modal.modal('show');
                // _modal.find('.date-picker').datepicker({ format: 'dd/mm/yyyy' });
            });
        });
    };
})(money || (money = {}));
$(function () {
    money.init();
});
//# sourceMappingURL=site.js.map