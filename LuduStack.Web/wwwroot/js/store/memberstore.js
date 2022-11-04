var MEMBERSTORE = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;

    var urlList;
    let urlPartnershipData;

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.containerList = '#containerlist';
        selectors.list = '#divList';
        selectors.btnSyncProduct = '.btn-sync-memberstore';
        selectors.spanFundsTotal = '#spanFundsTotal';
        selectors.spanFundsAvailable = '#spanFundsAvailable';
        selectors.spanFundsWithdrawn = '#spanFundsWithdrawn';
        selectors.btnHistory = '.btn-history';
        selectors.btnWithdraw = '.btn-withdraw';
        selectors.modalHistory = '#modalHistory';
        selectors.modalHistoryBody = '#modalHistory .modal-body';
        selectors.modalWithdraw = '#modalWithdraw';
        selectors.btnWithdrawConfirm = '#btnWithdrawConfirm';
        selectors.divWithdraw = '#divWithdraw';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.containerDetails = $(selectors.containerDetails);
        objs.containerList = $(selectors.containerList);
        objs.list = $(selectors.list);
        objs.spanFundsTotal = $(selectors.spanFundsTotal);
        objs.spanFundsAvailable = $(selectors.spanFundsAvailable);
        objs.spanFundsWithdrawn = $(selectors.spanFundsWithdrawn);
        objs.modalHistory = $(selectors.modalHistory);
        objs.modalHistoryBody = $(selectors.modalHistoryBody);
        objs.modalWithdraw = $(selectors.modalWithdraw);
        objs.divWithdraw = $(selectors.divWithdraw);
    }

    function init() {
        setSelectors();
        cacheObjs();

        urlList = objs.urls.data('urlList');
        urlPartnershipData = objs.urls.data('urlPartnershipData');

        loadPartnershipData(urlPartnershipData);

        loadItems(urlList);

        MEMBERSTORECOMMON.Callback.DeleteEntity = deleteCallback;

        canInteract = MAINMODULE.CanInteract();

        bindAll();
    }

    function bindAll() {
        bindSyncData();
        bindBtnHistory();
        bindBtnWithdraw();
        bindBtnWithdrawConfirm();
    }

    function bindSyncData() {
        objs.container.on('click', selectors.btnSyncProduct, function (e) {
            e.preventDefault();

            var btn = $(this);
            var url = btn.attr('href');

            if (canInteract && !btn.hasClass('disabled')) {
                MAINMODULE.Ajax.Post(url, null, null, syncCallback);
            }

            return false;
        });
    }

    function bindBtnHistory() {
        objs.container.on('click', selectors.btnHistory, function (e) {
            e.preventDefault();

            objs.modalHistory.modal('show');

            return false;
        });
    }

    function bindBtnWithdraw() {
        objs.container.on('click', selectors.btnWithdraw, function (e) {
            e.preventDefault();

            var btn = $(this);

            if (!btn.hasClass('disabled')) {
                var children = objs.divWithdraw.children();

                if (objs.divWithdraw.is(':visible')) {
                    children.fadeOut();

                    objs.divWithdraw.slideUp('slow');
                }
                else {
                    children.hide();
                    children.fadeIn('slow');

                    objs.divWithdraw.hide().removeClass('d-none').slideDown('slow');
                }
            }

            return false;
        });
    }

    function bindBtnWithdrawConfirm() {
        console.log($(selectors.btnWithdrawConfirm));
        objs.container.on('click', selectors.btnWithdrawConfirm, function (e) {
            e.preventDefault();

            var btn = $(this);

            console.log('clicked to confirm withdraw');

            return false;
        });
    }

    function loadItems(url) {
        MAINMODULE.Ajax.LoadHtml(url, objs.list);
    }

    function loadPartnershipData(url) {
        MAINMODULE.Ajax.Get(url, loadDataCallback);
    }

    function loadDataCallback(response) {
        if (response.success) {
            const data = response.value;

            if (data.fundsAvailable > 0) {
                $(selectors.btnWithdraw).removeClass('disabled');
            }

            objs.spanFundsTotal.text(MAINMODULE.Utils.ToLocaleNumber(data.fundsTotal));

            objs.spanFundsAvailable.text(MAINMODULE.Utils.ToLocaleNumber(data.fundsAvailable));

            objs.spanFundsWithdrawn.text(MAINMODULE.Utils.ToLocaleNumber(data.fundsWithdrawn));

            if (!!data.transactions && data.transactions.length > 0) {
                objs.modalHistoryBody.html('');
                for (var transaction of data.transactions) {
                    objs.modalHistoryBody.append(`<div class="row p-2 border-bottom rounded-0"><div class="col-12 col-md-4">${transaction.createDateText}</div><div class="col-12 col-sm-6 col-md-4">${transaction.typeText}</div><div class="col-12 col-sm-6 col-md-4">${transaction.value}</div></div>`)
                }
            }
        }
    }

    function syncCallback(response) {
        if (response.success) {
            loadPartnershipData(urlPartnershipData);

            loadItems(urlList);
        }
    }

    function deleteCallback(response) {
        if (response.success) {
            loadItems(urlList);
        }
    }

    return {
        Init: init
    };
}());

$(function () {
    MEMBERSTORE.Init();
});