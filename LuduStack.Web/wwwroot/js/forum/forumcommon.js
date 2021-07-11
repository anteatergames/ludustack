var FORUMCOMMON = (function () {
    "use strict";

    var selectors = {};
    var objs = {};
    var canInteract = false;

    function setSelectors() {
        selectors.controlsidebar = '.control-sidebar';
        selectors.urls = '#urls';
        selectors.container = '#featurecontainer';
        selectors.postItem = '.postitem';
        selectors.btnDelete = '.btn-entity-delete';
        selectors.btnVote = '.btn-vote';
        selectors.txtScore = '.txtScore';
    }

    function cacheObjs() {
        objs.controlsidebar = $(selectors.controlsidebar);
        objs.container = $(selectors.container);
        objs.urls = $(selectors.urls);
    }

    function init() {
        setSelectors();
        cacheObjs();

        canInteract = MAINMODULE.CanInteract();

        bindAll();
    }

    function bindAll() {
        bindDelete();
        bindBtnVote();
    }

    function bindDelete() {
        objs.container.on('click', selectors.btnDelete, function (e) {
            e.preventDefault();

            var btn = $(this);

            if (canInteract && !btn.hasClass('disabled')) {
                MAINMODULE.Common.DeleteEntity(btn, FORUMCOMMON.Callback.DeleteEntity);
            }

            return false;
        });
    }

    function bindBtnVote() {
        objs.container.on('click', selectors.btnVote, function () {
            var btn = $(this);
            var colorClass = btn.data('colorclass');
            var item = btn.closest(selectors.postItem);
            var postId = item.data('postid');
            var vote = btn.data('vote');
            var sameVote = item.data('currentuservote') === vote;

            if (canInteract === true && !sameVote) {
                var url = objs.urls.data('urlVote');

                return $.post(url, { postId, vote }).then(function (response) {
                    if (response.success === true) {
                        item.data('currentuservote', vote);
                        var txtScore = item.find(selectors.txtScore);
                        txtScore.fadeOut('fast', () => {
                            txtScore.html(response.value).fadeIn();
                        });

                        var allVoteButtons = item.find(selectors.btnVote);

                        allVoteButtons.each((index, element) => {
                            var thisBtn = $(element);
                            var colorClass2 = thisBtn.data('colorclass');
                            thisBtn.removeClass(colorClass2);
                        });
                        btn.addClass(colorClass);
                    }
                    else {
                        MAINMODULE.Ajax.HandleErrorResponse(response);
                    }
                });
            }
        });
    }

    return {
        Init: init,
        CanInteract: canInteract,
        Callback: {
            DeleteEntity: function () {
                console.log('empty FORUMCOMMON.Callback.DeleteEntity');
            }
        }
    };
}());

$(function () {
    FORUMCOMMON.Init();
});