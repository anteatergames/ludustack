﻿var POLLS = (function () {
    "use strict";
    var selectors = {};

    function init() {
        setSelectors();

        bindAll();
    }

    function setSelectors() {
        selectors.divPostPoll = $('div#divPostPoll');
        selectors.options = $('input.polloptioninput');
    }

    function bindAll() {
        bindBtnAddOptionBtn();
        bindBtnRemoveOptionBtn();
        bindPollVote();
    }

    function bindBtnAddOptionBtn() {
        $('.content').on('click', '#btnAddOption', function (e) {
            var option = selectors.divPostPoll.children('.polloption:first');
            var newOption = option.clone();
            var input = newOption.children('.polloptioninput');
            input.val('');

            var btnRemoveOption = newOption.find('.btn-option-remove');
            $(btnRemoveOption).removeClass('invisible');

            newOption.insertBefore('#btnAddOption');
            newOption.addClass('polloptionextra');
            input.focus();
            POLLS.Events.PostAddOption();
        });
    }

    function bindBtnRemoveOptionBtn() {
        $('.content').on('click', '.btn-option-remove', function (e) {
            var option = $(this).closest('.polloption');

            option.remove();
            POLLS.Events.PostAddOption();
        });
    }

    function bindPollVote() {
        $('.content').on('click', 'ul.poll.canvote li input.poll-option', function (e) {
            var pollOptionCtrl = $(this);

            vote(pollOptionCtrl.val()).done(function (response) { voteCallback(response, pollOptionCtrl); });
        });
    }

    function clearOptions() {
        selectors.options.val('');
        $('.polloptionextra').remove();
    }

    function vote(pollOptionId) {
        return $.post("/interact/poll/vote", { pollOptionId: pollOptionId })
            .done(function (response) {
                if (response.success) {
                    MAINMODULE.Common.HandlePointsEarned(response);
                }
            });
    }
    function voteCallback(response, pollOptionCtrl) {
        if (response.success === true) {
            var poll = pollOptionCtrl.closest('ul.poll');
            var pollWrapper = poll.closest('.poll-wrapper');
            var pollVoteCount = pollWrapper.find('.poll-vote-count');
            var allOptions = poll.children('li');

            $(allOptions).each(function () {
                var li = $(this);
                li.removeClass('voted').addClass('notvoted');
                var percBack2 = li.children('span.perc-back');
                var percNumber2 = li.children('span.perc-number');
                var input2 = li.find('input');

                percBack2.css('width', '0');
                percNumber2.text('0.00%');
                input2.prop("checked", false);
            });

            var votedLi = $(pollOptionCtrl).parent('li');
            votedLi.removeClass('notvoted').addClass('voted');
            var input = votedLi.find('input');
            input.prop("checked", true);

            for (let optionReturned of response.value.optionResults) {
                var optionInput = poll.find(`input[value="${optionReturned.optionId}"]`);
                var optionLi = optionInput.parent('li');
                var percBack = optionLi.children('span.perc-back');
                var percNumber = optionLi.children('span.perc-number');

                percBack.css('width', optionReturned.percentage + '%');
                percNumber.text(optionReturned.percentage + '%');
            }

            pollVoteCount.text(response.value.totalVotes);
        }
    }

    return {
        Init: init,
        Methods: {
            ClearOptions: clearOptions
        },
        Events: {
            PostAddOption: function () { console.log('polls default PostAddOption'); }
        }
    };
}());

$(function () {
    POLLS.Init();
});