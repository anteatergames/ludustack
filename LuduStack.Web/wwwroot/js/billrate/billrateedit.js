var BILLRATEEDIT = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var canInteract = false;
    var isNew = false;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.canInteract = '#caninteract';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.form = '#frmBillRateSave';
        selectors.userId = '#UserId';
        selectors.billRateType = '#BillRateType';
        selectors.artStyle = '#ArtStyle';
        selectors.soundStyle = '#SoundStyle';
        selectors.gameElement = '#GameElement';
        selectors.hourPrice = '#HourPrice';
        selectors.hourQuantity = '#HourQuantity';
        selectors.txtTotal = '#txtTotal';
        selectors.btnSave = '#btnSaveBillRate';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.form = $(selectors.form);
        objs.userId = $(selectors.userId);
        objs.billRateType = $(selectors.billRateType);
        objs.artStyle = $(selectors.artStyle);
        objs.soundStyle = $(selectors.soundStyle);
        objs.gameElement = $(selectors.gameElement);
        objs.hourPrice = $(selectors.hourPrice);
        objs.hourQuantity = $(selectors.hourQuantity);
        objs.txtTotal = $(selectors.txtTotal);
    }

    function init() {
        setSelectors();
        cacheObjs();

        typeChange(true, objs.billRateType.val());

        bindAll();

        canInteract = $(selectors.canInteract).val();
        isNew = window.location.href.indexOf('add') > -1;

        if (isNew) {
            console.log('new bill rate');
        }

        MAINMODULE.Common.BindPopOvers();

        calculateTotal();
    }

    function bindAll() {
        bindBtnSaveForm();
        bindTypeChange();
        bindHourPriceChange();
        bindHourQuantityChange();
    }

    function bindTypeChange() {
        objs.container.on('change', selectors.billRateType, function (e) {
            typeChange(false, e.target.value);
        });
    }

    function bindHourPriceChange() {
        objs.container.on('change', selectors.hourPrice, calculateTotal);
    }

    function bindHourQuantityChange() {
        objs.container.on('change', selectors.hourQuantity, calculateTotal);
    }

    function typeChange(initial, value) {
        $('.conditionalbytype[data-visiblewhentype!="' + value + '"]').hide();
        $('.conditionalbytype[data-visiblewhentype="' + value + '"]').show();

        var firstVisibleOption = $(selectors.gameElement + ' option.conditionalbytype[data-visiblewhentype="' + value + '"]:first');

        if (!initial && (isNew || objs.gameElement.val() !== firstVisibleOption.first().val())) {
            $(selectors.gameElement).val(firstVisibleOption.first().val());
        }

        calculateTotal();
    }

    function bindBtnSaveForm() {
        objs.container.on('click', selectors.btnSave, function () {
            var btn = $(this);
            var valid = objs.form.valid();

            if (valid && canInteract) {
                MAINMODULE.Common.DisableButton(btn);

                submitForm(btn);
            }
        });
    }

    function submitForm(btn) {
        var url = objs.form.attr('action');
        var data = objs.form.serializeObject();

        return $.post(url, data).done(function (response) {
            if (response.success === true) {
                MAINMODULE.Common.PostSaveCallback(response, btn);

                MAINMODULE.Ajax.HandleUrlResponse(response);
            }
            else {
                MAINMODULE.Ajax.HandleErrorResponse(response);
            }
        });
    }

    function calculateTotal() {
        var price = parseInt(objs.hourPrice.val() || 0);
        var quantity = parseInt(objs.hourQuantity.val() || 0);

        objs.txtTotal.text(price * quantity);
    }

    return {
        Init: init
    };
}());

$(function () {
    BILLRATEEDIT.Init();
});