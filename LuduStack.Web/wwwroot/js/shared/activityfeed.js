var FEEDTYPE = {
    HOME: 1,
    USER: 2,
    GAME: 3,
    ARTICLES: 4
};

var ACTIVITYFEED = (function () {
    "use strict";
    var selectors = {};
    var selectorText = {};

    var feedType;
    var feedId;

    var oldestGuid;
    var oldestDate;

    var embedo;

    function init(divActivityFeed, type, id) {
        feedType = type;
        feedId = id;

        selectors.divActivityFeed = divActivityFeed;
        selectorText.btnMorePosts = '#btnMorePosts';
        selectorText.btnDeletePost = '.btnDeletePost';
        selectors.btnInteractionShare = '.btn-interaction-share';
        selectors.sharePopup = '.share-popup';
        selectors.videoClickArea = '.video-thumbnail';

        bindAll();
    }

    function bindAll() {
        bindMorePosts();
        bindDeletePost();
        bindOEmbedClick();
    }

    function bindMorePosts() {
        $('body').on('click', selectorText.btnMorePosts, function () {
            var btn = $(this);
            oldestGuid = btn.data('oldestid');
            oldestDate = btn.data('oldestdate');

            MAINMODULE.Common.DisableButton(btn);

            ACTIVITYFEED.Methods.LoadActivityFeed(false);
        });
    }

    function bindDeletePost() {
        $('body').on('click', selectorText.btnDeletePost, function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            var msg = btn.data('confirmationmessage');
            var confirmationTitle = btn.data('confirmationtitle');
            var confirmationButtonText = btn.data('confirmationbuttontext');
            var cancelButtonText = btn.data('cancelbuttontext');

            ALERTSYSTEM.ShowConfirmMessage(confirmationTitle, msg, confirmationButtonText, cancelButtonText, function () {
                $.ajax({
                    url: '/content/' + id,
                    type: 'DELETE'
                }).done(function (response) {
                    if (response.success) {
                        var elementToDelete = btn.closest('.box-content');
                        elementToDelete.remove();

                        if (response.message) {
                            ALERTSYSTEM.ShowSuccessMessage(response.message);
                        }
                    }
                    else {
                        ALERTSYSTEM.ShowWarningMessage(response.message);
                    }
                });
            });

            return false;
        });
    }

    function bindOEmbedClick() {
        $('body').on('click', selectors.videoClickArea, function (e) {
            e.preventDefault();

            loadSingleOembed(this);

            return false;
        });
    }

    function loadActivityFeed(first, callback) {
        if (first !== false) {
            selectors.divActivityFeed.append(MAINMODULE.Default.Spinner);
        }

        var url = "/content/feed?load=1";

        if (feedId && feedType === FEEDTYPE.USER) {
            url += '&userId=' + feedId;
        }
        if (feedId && feedType === FEEDTYPE.GAME) {
            url += '&gameId=' + feedId;
        }
        if (feedType === FEEDTYPE.ARTICLES) {
            url += '&articlesOnly=True';
        }

        if (oldestGuid) {
            url += '&oldestId=' + oldestGuid;
        }
        if (oldestDate) {
            url += '&oldestDate=' + oldestDate;
        }

        MAINMODULE.Ajax.GetHtml(url).then((response) => {
            if (oldestDate !== undefined && oldestGuid !== undefined) {
                selectors.divActivityFeed.find('.spinner').remove();
                $(selectorText.btnMorePosts).parent().remove();
                selectors.divActivityFeed.append(response);
            }
            else {
                selectors.divActivityFeed.html(response);
            }

            CONTENTACTIONS.BindShareContent();
            if (callback) {
                callback();
            }
        });
    }

    function initializeEmbed() {
        if (!embedo) {
            embedo = new Embedo({
                youtube: true,
                facebook: {
                    appId: $('meta[property="fb:app_id"]').attr('content'), // Enable facebook SDK
                    version: 'v3.2',
                    width: "100%"
                }
            });
        }
    }

    function loadSingleOembed(element) {
        var obj = $(element);
        var wrapper = obj.closest('.videoWrapper');
        var oembed = wrapper.find('oembed');
        var videoUrl = wrapper.data('url');

        if (typeof Embedo === 'function') {
            initializeEmbed();

            if (wrapper.hasClass('loaded')) {
                return;
            }
            else {
                obj.hide();

                var w = wrapper.width();
                var h = w * 9 / 16;

                embedo.load(oembed.get(0), videoUrl, {
                    width: w,
                    height: h,
                    centerize: true,
                    strict: false
                })
                    .done(function () {
                        wrapper.addClass('loaded');
                    });
            }
        }
    }

    return {
        Init: init,
        Methods: {
            LoadActivityFeed: loadActivityFeed
        },
        Events: {
        }
    };
}());