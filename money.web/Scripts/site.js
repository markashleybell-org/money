/// <reference types="jquery" />
var money;
(function (money) {
    var _accountList;
    money.init = function () {
        _accountList = $('.panel-group');
        _accountList.on('show.bs.collapse hide.bs.collapse', function (e) {
            var icon = $('#' + e.target.id.replace('categories', 'heading')).find('.account-heading-view-toggle > .fa');
            if (e.type == 'show') {
                icon.removeClass('fa-chevron-down').addClass('fa-chevron-up');
            }
            else {
                icon.removeClass('fa-chevron-up').addClass('fa-chevron-down');
            }
        });
    };
})(money || (money = {}));
$(function () {
    money.init();
});
//# sourceMappingURL=site.js.map