﻿var JOBPOSITION = (function () {
    "use strict";

    var rootUrl = '/work/jobposition';
    var urlListDefault = rootUrl + '/list';
    var urlMyPositionStats = rootUrl + '/mypositionsstats';
    var urlMyApplications = rootUrl + '/myapplications';

    var selectors = {};
    var objs = {};
    var canInteract = false;
    var isIndex = false;
    var isNew = false;
    var isDetails = false;
    var isCompany = false;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.jobProfile = '#jobprofile';
        selectors.container = '#jobpositioncontainer';
        selectors.containerDetails = '#containerdetails';
        selectors.containerList = '#containerlist';
        selectors.list = '#divList';
        selectors.divListItem = '.jobposition-item';
        selectors.btnNewJobPosition = '.btn-jobposition-new';
        selectors.btnListMine = '#btn-jobposition-listmine';
        selectors.form = '#frmJobPositionSave';
        selectors.btnSave = '#btnPostJobPosition';
        selectors.btnEditJobPosition = '.btnEditJobPosition';
        selectors.btnDeleteJobPosition = '.btnDeleteJobPosition';
        selectors.btnApply = '#btnApply';
        selectors.chkRemote = '#Remote';
        selectors.location = '#Location';
        selectors.origin = '#Origin';
        selectors.url = '#Url';
        selectors.myPositionStats = '#divMyPositionStats';
        selectors.myApplications = '#divMyApplications';
        selectors.datepicker = '.datepicker';
        selectors.switchBenefit = '.switch-benefit';
        selectors.hdnBenefit = '.hdnBenefit';
        selectors.applicantRating = '.applicant-rating';
        selectors.btnShare = '.btn-share';
    }

    function cacheObjects() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
        objs.containerDetails = $(selectors.containerDetails);
        objs.containerList = $(selectors.containerList);
        objs.list = $(selectors.list);
        objs.myPositionStats = $(selectors.myPositionStats);
        objs.myApplications = $(selectors.myApplications);
        objs.btnApply = $(selectors.btnApply);
    }

    function cacheObjectsCreateEdit() {
        objs.location = $(selectors.location);
        objs.origin = $(selectors.origin);
        objs.url = $(selectors.url);
    }

    function setCreateEdit() {
        cacheObjectsCreateEdit();

        $(selectors.datepicker).datepicker();
    }

    function init() {
        setSelectors();
        cacheObjects();

        canInteract = MAINMODULE.CanInteract();
        isNew = COMMONEDIT.IsNew();
        isCompany = objs.container.find(selectors.jobProfile).val() === 'Company';
        isDetails = window.location.href.indexOf('details') > -1;
        isIndex = !isNew && !isDetails;

        bindAll();

        if (isIndex) {
            if (isCompany) {
                var url = objs.urls.data('urlMine');
                loadJobPositions(false, url);
                loadMyJobPositionStats(urlMyPositionStats);
            }
            else {
                loadJobPositions(false, urlListDefault);
                loadMyApplications(urlMyApplications);
            }
        }
        else if (isDetails) {
            bindDetails();
        }
    }

    function bindAll() {
        bindBtnNewJobPosition();
        bindBtnSaveForm();
        bindBtnApply();
        bindEditJobPosition();
        bindDeleteJobPosition();
        bindRemoteChange();
        bindSwitchBenefitChange();
    }

    function bindDetails() {
        bindRatings();
        bindShareButton();
    }

    function bindShareButton() {
        objs.container.on('click', selectors.btnShare, function (e) {
            e.preventDefault();
            var url = $(this).prop('href');
            var title = $(this).data('title');
            var provider = $(this).data('provider');

            url = encodeURI(url);

            if (provider === 'facebook') {
                FB.ui({
                    method: 'share',
                    href: url
                }, function (response) {
                    console.log(response);
                });
            }
            else if (provider === 'reddit') {
                url = 'https://www.reddit.com/submit?title=' + title + '&url=' + url;

                window.open(url);
            }
            else if (provider === 'twitter') {
                var text = $(this).data('text');
                url = 'https://www.twitter.com/intent/tweet?text=' + text + ' ' + url;

                window.open(url);
            }
            else {
                console.log(provider);
            }

            return false;
        });
    }

    function bindRatings() {
        $(selectors.applicantRating).rating({
            theme: 'krajee-fas',
            showClear: false,
            size: 'md',
            animate: false,
            step: 0.5,
            filledStar: '<i class="fas fa-gamepad tilt-20"></i>',
            emptyStar: '<i class="fas fa-gamepad tilt-20"></i>',
            starCaptions: {
                0.5: 'Padawan',
                1: '1',
                1.5: '1.5',
                2: '2',
                2.5: '2.5',
                3: '3',
                3.5: '3.5',
                4: '4',
                4.5: '4.5',
                5: 'Jedi'
            }
        });

        objs.container.on('rating:change', selectors.applicantRating, function (event, value, caption) {
            var url = $(this).data('url');

            var data = { score: value };

            genericPost(url, data);
        });
    }

    function bindBtnNewJobPosition() {
        objs.container.on('click', selectors.btnNewJobPosition, function () {
            var url = $(this).data('url');
            if (canInteract) {
                loadEditForm(url);
            }
        });
    }

    function bindBtnSaveForm() {
        objs.containerDetails.on('click', selectors.btnSave, function () {
            var btn = $(this);
            var valid = objs.form.valid();

            var origin = objs.origin.val();

            if (origin === 'external' && objs.url.val().length === 0) {
                valid = false;
                ALERTSYSTEM.ShowWarningMessage(MAINMODULE.Common.TranslatedMessages['msgUrlMissing']);
            }

            if (valid && canInteract && !btn.hasClass('disabled')) {
                MAINMODULE.Common.DisableButton(btn).ready(() => {
                    submitForm(btn);
                });
            }
        });
    }

    function bindEditJobPosition() {
        objs.container.on('click', selectors.btnEditJobPosition, function (e) {
            e.preventDefault();
            var url = $(this).data('url');

            if (canInteract) {
                loadEditForm(url);
            }
        });
    }

    function bindDeleteJobPosition() {
        objs.container.on('click', selectors.btnDeleteJobPosition, function (e) {
            e.preventDefault();

            var btn = $(this);
            var url = $(this).data('url');
            var msg = btn.data('confirmationmessage');
            var confirmationTitle = btn.data('confirmationtitle');
            var confirmationButtonText = btn.data('confirmationbuttontext');
            var cancelButtonText = btn.data('cancelbuttontext');

            ALERTSYSTEM.ShowConfirmMessage(confirmationTitle, msg, confirmationButtonText, cancelButtonText, function () {
                $.ajax({
                    url: url,
                    type: 'DELETE'
                }).done(function (response) {
                    deleteJobPositionCallback(response, btn);
                });
            });
        });
    }

    function deleteJobPositionCallback(response, btn) {
        if (response.success) {
            btn.closest(selectors.divListItem).remove();

            if (response.message) {
                ALERTSYSTEM.ShowSuccessMessage(response.message, function () {
                    if (response.url) {
                        MAINMODULE.Ajax.HandleUrlResponse(response);
                    }
                    else {
                        loadJobPositions(false, urlListDefault);
                        loadMyJobPositionStats(urlMyPositionStats);
                    }
                });
            }
            else {
                if (response.url) {
                    MAINMODULE.Ajax.HandleUrlResponse(response);
                }
                else {
                    loadJobPositions(false, urlListDefault);
                    loadMyJobPositionStats(urlMyPositionStats);
                }
            }
        }
        else {
            ALERTSYSTEM.ShowWarningMessage(response.message);
        }
    }

    function bindRemoteChange() {
        objs.containerDetails.on('change', selectors.chkRemote, function () {
            var isRemote = $(this).is(':checked');

            if (isRemote) {
                objs.location.val('').hide();
            }
            else {
                objs.location.val('').show();
            }
        });
    }

    function bindSwitchBenefitChange() {
        objs.containerDetails.on('change', selectors.switchBenefit, function () {
            var obj = $(this);
            var isChecked = obj.is(':checked');

            var hdn = obj.parent().find(selectors.hdnBenefit);

            if (isChecked) {
                hdn.val('True');
            }
            else {
                hdn.val('False');
            }
        });
    }

    function bindBtnApply() {
        objs.container.on('click', selectors.btnApply, function (e) {
            e.preventDefault();

            var btn = $(this);

            if (canInteract && !btn.hasClass('disabled')) {
                MAINMODULE.Common.DisableButton(btn).ready(() => {
                    var url = btn.data('url');

                    apply(url, function (response) {
                        MAINMODULE.Common.PostSaveCallback(response, btn);
                    });
                });
            }

            return false;
        });
    }

    function loadJobPositions(fromControlSidebar, url) {
        objs.list.html(MAINMODULE.Default.Spinner);
        objs.containerDetails.html('');
        objs.containerDetails.hide();

        MAINMODULE.Ajax.LoadHtml(url, objs.list).then(() => {
            cacheObjects();
        });
    }

    function loadMyJobPositionStats(url) {
        objs.myPositionStats.html(MAINMODULE.Default.SpinnerTop);

        MAINMODULE.Ajax.LoadHtml(url, objs.myPositionStats);
    }

    function loadMyApplications(url) {
        if (canInteract) {
            objs.myApplications.html(MAINMODULE.Default.SpinnerTop);

            MAINMODULE.Ajax.LoadHtml(url, objs.myApplications);
        }
    }

    function apply(url, callback) {
        genericPost(url, null, callback);
    }

    function loadEditForm(url) {
        objs.containerDetails.html(MAINMODULE.Default.Spinner);
        objs.containerList.hide();

        MAINMODULE.Ajax.LoadHtml(url, objs.containerDetails).then(() => {
            objs.containerDetails.show();
            objs.form = $(selectors.form);

            $.validator.unobtrusive.parse(selectors.form);
            setCreateEdit();
        });
    }

    function submitForm(btn, callback) {
        var url = objs.form.attr('action');

        var data = objs.form.serialize();

        $.post(url, data).done(function (response) {
            if (response.success === true) {
                MAINMODULE.Common.PostSaveCallback(response, btn);

                if (callback) {
                    callback();
                }

                MAINMODULE.Ajax.HandleUrlResponse(response);
            }
            else {
                MAINMODULE.Ajax.HandleErrorResponse(response);
            }
        });
    }

    function genericPost(url, data, callback) {
        $.post(url, data).done(function (response) {
            if (response.success === true) {
                if (callback) {
                    callback(response);
                }

                MAINMODULE.Ajax.HandleUrlResponse(response);
            }
            else {
                ALERTSYSTEM.ShowWarningMessage(response.message);
            }
        });
    }

    return {
        Init: init
    };
}());

$(function () {
    JOBPOSITION.Init();
});