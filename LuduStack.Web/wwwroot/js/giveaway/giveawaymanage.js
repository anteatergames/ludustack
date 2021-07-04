var GIVEAWAYMANAGE = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var canInteract = false;

    function setSelectors() {
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

        canInteract = MAINMODULE.CanInteract();

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

            if (canInteract) {
                EXPORTHELPER.TableToExcel('tblParticipantsExport', 'Participants');
            }

            return false;
        });
    }

    function bindBtnDeleteParticipant() {
        objs.container.on('click', selectors.btnDeleteParticipant, buttonDelete);
    }

    function bindBtnDeclareNotWinner() {
        objs.container.on('click', selectors.btnDeclareNotWinner, buttonPost);
    }

    function bindBtnClearParticipants() {
        objs.container.on('click', selectors.btnClearParticipants, buttonDelete);
    }

    function bindBtnPickSingleWinner() {
        objs.container.on('click', selectors.btnPickSingleWinner, buttonPost);
    }

    function bindBtnPickAllWinners() {
        objs.container.on('click', selectors.btnPickAllWinners, buttonPost);
    }

    function buttonDelete(e) {
        e.preventDefault();
        var btn = $(this);

        if (canInteract) {
            MAINMODULE.Common.DeleteEntity(btn);
        }

        return false;
    }

    function buttonPost(e) {
        e.preventDefault();
        var btn = $(this);

        if (canInteract) {
            MAINMODULE.Common.PostWithConfirmation(btn);
        }

        return false;
    }

    return {
        Init: init
    };
}());

$(function () {
    GIVEAWAYMANAGE.Init();
});