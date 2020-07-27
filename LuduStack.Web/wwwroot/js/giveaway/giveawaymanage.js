var GIVEAWAYMANAGE = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var canInteract = false;

    function setSelectors() {
        selectors.canInteract = '#caninteract';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.btnDeleteParticipant = '.btn-participant-delete';
        selectors.btnDeclareNotWinner = '.btn-participant-declarenotwinner';
        selectors.btnClearParticipants = '#btnClearParticipants';
        selectors.btnPickSingleWinner = '#btnPickSingleWinner';
        selectors.btnPickAllWinners = '#btnPickAllWinners';
        selectors.btnExportParticipantList = '#btnExportParticipantList';
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.containerDetails = $(selectors.containerDetails);
    }

    function init() {
        setSelectors();
        cacheObjs();

        canInteract = objs.container.find(selectors.canInteract).val();

        bindAll();

        FX.StartCountDown('#SecondsToEnd');
    }

    function bindAll() {
        bindBtnDeleteParticipant();
        bindBtnDeclareNotWinner();
        bindBtnClearParticipants();
        bindBtnPickSingleWinner();
        bindBtnPickAllWinners();
        bindBtnExportParticipantList();
    }

    function bindBtnExportParticipantList() {
        objs.container.on('click', selectors.btnExportParticipantList, function (e) {
            e.preventDefault();
            var btn = $(this);

            if (canInteract) {
                EXPORTHELPER.TableToExcel('tblParticipantsExport', 'Participants');
            }

            return false;
        });
    }

    function bindBtnDeleteParticipant() {
        objs.container.on('click', selectors.btnDeleteParticipant, function (e) {
            e.preventDefault();
            var btn = $(this);

            if (canInteract) {
                MAINMODULE.Common.DeleteEntity(btn);
            }

            return false;
        });
    }

    function bindBtnDeclareNotWinner() {
        objs.container.on('click', selectors.btnDeclareNotWinner, function (e) {
            e.preventDefault();
            var btn = $(this);

            if (canInteract) {
                MAINMODULE.Common.PostWithConfirmation(btn);
            }

            return false;
        });
    }

    function bindBtnClearParticipants() {
        objs.container.on('click', selectors.btnClearParticipants, function (e) {
            e.preventDefault();
            var btn = $(this);

            if (canInteract) {
                MAINMODULE.Common.DeleteEntity(btn);
            }

            return false;
        });
    }

    function bindBtnPickSingleWinner() {
        objs.container.on('click', selectors.btnPickSingleWinner, function (e) {
            e.preventDefault();
            var btn = $(this);

            if (canInteract) {
                MAINMODULE.Common.PostWithConfirmation(btn);
            }

            return false;
        });
    }

    function bindBtnPickAllWinners() {
        objs.container.on('click', selectors.btnPickAllWinners, function (e) {
            e.preventDefault();
            var btn = $(this);

            if (canInteract) {
                MAINMODULE.Common.PostWithConfirmation(btn);
            }

            return false;
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    GIVEAWAYMANAGE.Init();
});